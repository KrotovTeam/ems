const researchController = require('../controllers/research');
const downloadScheduler = require('./downloadScheduller');

const SERVICES = {
    SRTM: require('./SRTM'),
    USGS: require('./usgs')
};

const RESEARCHES = {
    FOREST_DISEASES: {
        name: 'Болезни лесных насаждений',
        satellites: ['LANDSAT', 'SENTINEL']
    },
    SOIL_EROSION: {
        name: 'Эрозия почвы',
        satellites: ['LANDSAT', 'SENTINEL']
    },
    SOIL_POLLUTION_BY_OIL_PRODUCTS: {
        name: 'Загрязнение почвы нефтепродуктами',
        satellites: ['LANDSAT', 'SENTINEL']
    },
    SURFACE_DUMPS: {
        name: 'Поверхностные свалки',
        satellites: ['LANDSAT', 'SENTINEL']
    },
};

const satellites = {
    LANDSAT: {
        name: 'Landsat_8_C1',
        service: 'USGS'
    },
    SENTINEL: {
        name: 'Sentinel_2A',
        service: 'USGS'
    },
    MODIS: {
        name: 'Modis_MOD09GQ',
        service: 'USGS'
    },
    SRTM: {
        name: 'SRTM',
        service: 'SRTM'
    },
};


const STATE = {
    CREATED: {
        code: 1,
        message: 'Создан. На обработке.'
    },
    DOWNLOADING: {
        code: 2,
        message: 'Загрузка снимков'
    },
    RESEARCH: {
        code: 3,
        message: 'Исследование сников'
    },
    HANDLE_RESULT: {
        code: 4,
        message: 'Обработка результатов'
    },
    COMPLETED: {
        code: 5,
        message: 'Завершен'
    },
    ERROR_GET_PHOTOS: {
        code: 6,
        message: 'Ошибка при получение снимков'
    },
    UNKNOWN_ERROR: {
        code: 7,
        message: 'Неизвестная ошибка'
    }

};


async function handleResearch(research, startData, endData, countYears = 2, coord = [59, 37, 59, 38], cloudMax = 100, month, username) {
    const researchRes = await researchController.create(username, RESEARCHES[research].name, coord, [], countYears, [], [],
        [], cloudMax, month, 1);

    const requestId = researchRes.id;

    setTimeout(async () => {
        // По названию исследования узнаём снимки каких спутников нам нужны и Какие сервисы их получают
        const satellitesHandle = RESEARCHES[research].satellites;
        // Для каждого спутника получаем ссылки для скачивания
        let linksDownload = [];

        for (let i = 0; i < satellitesHandle.length; i++) {
            const satellite = satellitesHandle[i];
            const satelliteName = satellites[satellite].name;

            // По названию спутника определим сервис который получает ссылки на снимким
            const serviceName = satellites[satellite].service;
            switch (serviceName) {
                case 'SRTM': {
                    const srtmResult = await SERVICES[serviceName].getDownloadLink(satelliteName, coord);
                    if (srtmResult.length === 0) {//Если модуль не получил ссылки для скачивания
                        return await researchController.setStatus(researchRes.id, STATE.ERROR_GET_PHOTOS.code);
                    }
                    linksDownload = linksDownload.concat(srtmResult);

                    break;
                }
                case 'USGS': {
                    const usgsResult = await SERVICES[serviceName].getDownloadLink(satelliteName, startData, endData, countYears, coord, cloudMax, month);
                    if (usgsResult.length === 0) { //Если модуль не получил ссылки для скачивания
                        return await researchController.setStatus(researchRes.id, STATE.ERROR_GET_PHOTOS.code);
                    }
                    linksDownload = linksDownload.concat(usgsResult);

                    break;
                }
            }
        }


        await researchController.setMiniImagePath(researchRes.id, linksDownload.filter(r => !!r.imagePath).map(r => r.imagePath));
        await researchController.setLinksDownload(researchRes.id, linksDownload.map(r => r.linkDownloadArchive));

        // Попытаемся скачать снимки
        await researchController.setStatus(researchRes.id, STATE.DOWNLOADING.code);


        downloadScheduler.addToQueDownload({
            requestId,
            data: linksDownload
        }, async (err, arrayPath) => {
            if (err) {
                await researchController.setStatus(researchRes.id, STATE.ERROR_GET_PHOTOS.code);
            }
            // Файлы скачались безошибочно
            await researchController.setStatus(researchRes.id, STATE.HANDLE_RESULT.code);
            await researchController.setPathsDownload(researchRes.id, arrayPath);
        });
    }, 10);


    return researchRes;
}


module.exports = {
    handleResearch
};