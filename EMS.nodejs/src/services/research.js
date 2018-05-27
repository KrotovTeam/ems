const researchController = require('../controllers/research');
const downloadScheduler = require('./downloadScheduller');
const amqp = require('./../amqp/index');
const uuid = require('uuid/v4');
const fs = require('fs');
const config = require('config');
const util = require('util');


const SERVICES = {
    SRTM: require('./SRTM'),
    USGS: require('./usgs')
};

const CHARACTERISTICS = {
    AREA_OF_DAMAGE: {
        title: 'площадь повреждений',
        satellite: 'LANDSAT',
        type: 1,
        folder: 'areaOfDamage'
    },
    DIGITAL_RELIEF_MODEL: {
        title: 'Цифоровая модель рельефа',
        satellite: 'SRTM',
        type: 2,
        folder: 'digitalReliefModel'
    }
};

const RESEARCHES = {
    FOREST_DISEASES: {
        name: 'Болезни лесных насаждений',
        satellites: ['LANDSAT'],
        characteristics: ['AREA_OF_DAMAGE', 'DIGITAL_RELIEF_MODEL'],
        type: 1
    },
    SOIL_EROSION: {
        name: 'Эрозия почвы',
        satellites: ['LANDSAT'],
        type: 2
    },
    SOIL_POLLUTION_BY_OIL_PRODUCTS: {
        name: 'Загрязнение почвы нефтепродуктами',
        satellites: ['LANDSAT'],
        type: 3
    },
    SURFACE_DUMPS: {
        name: 'Поверхностные свалки',
        satellites: ['LANDSAT'],
        type: 4
    }
};

const satellites = {
    LANDSAT: {
        name: 'Landsat_8_C1',
        service: 'USGS',
        type: 1
    },
    SENTINEL: {
        name: 'Sentinel_2A',
        service: 'USGS',
        type: 2
    },
    MODIS: {
        name: 'Modis_MOD09GQ',
        service: 'USGS',
        type: 3
    },
    SRTM: {
        name: 'SRTM',
        service: 'SRTM',
        type: 4
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
    CALIBRATION: {
        code: 3,
        message: 'Калибровка данных'
    },
    FIND_PHENOMENON: {
        code: 4,
        message: 'Поиск явления'
    },
    NO_FIND_PHENOMENON: {
        code: 5,
        message: 'Явление не найдено'
    },
    DOWNLOAD_DATA_FOR_CHARACTERISTICS: {
        code: 6,
        message: 'Загрузка дополнительных снимков для определения хар-к'
    },
    FIND_CHARACTERISTICS: {
        code: 7,
        message: 'Вычисление характеристик'
    },
    COMPLETED: {
        code: 8,
        message: 'Завершен'
    },
    ERROR_GET_PHOTOS: {
        code: 9,
        message: 'Ошибка при получение снимков'
    },
    UNKNOWN_ERROR: {
        code: 10,
        message: 'Неизвестная ошибка'
    }
};

async function handleResearch(research, startData, endData, countYears = 2, coord = [59, 37, 59, 38], cloudMax = 100, month, username) {
    const researchRes = await researchController.create(username, RESEARCHES[research].name, coord, [], countYears, [], [],
        [], cloudMax, month, 1);

    console.log(countYears);

    const requestId = researchRes.id;

    const userDir = await createUserResFolders(requestId);

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
                        console.log('Подходящие снимки не найдены!!!');
                        return await researchController.setStatus(researchRes.id, STATE.ERROR_GET_PHOTOS.code);
                    }
                    linksDownload = linksDownload.concat(srtmResult);

                    break;
                }
                case 'USGS': {
                    const usgsResult = await SERVICES[serviceName].getDownloadLink(satelliteName, startData, endData, countYears, coord, cloudMax, month);
                    if (usgsResult.length === 0) { //Если модуль не получил ссылки для скачивания
                        console.log('Подходящие снимки не найдены!!!');
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


        let arrayLandsat = null;
        try {
            arrayLandsat = await _downloadAsync(requestId, linksDownload);
        } catch (err) {
            return await researchController.setStatus(researchRes.id, STATE.ERROR_GET_PHOTOS.code);
        }
        // Файлы скачались безошибочно
        await researchController.setStatus(researchRes.id, STATE.CALIBRATION.code);
        await researchController.setPathsDownload(researchRes.id, arrayLandsat);

        // После получения снимков Landsat


        // Проверим есть ли откалиброванные данные для каждой папки
        for (let i = 0; i < arrayLandsat.length; i++) {
            const pathCheck = arrayLandsat[i];
            if (!(await checkExistFolder(`${pathCheck}\\NormalizationData`))) { // Если нет отклаброванный данных
                console.log('Калибруем данные для: pathCheck');
                await amqp.calibration({
                    folder: pathCheck,
                    satelliteType: satellites.LANDSAT.type
                });
                console.log('Калибровка прошла успаешно калибровка: ')
            }
        }
        // Начнем поиск явления
        await researchController.setStatus(researchRes.id, STATE.FIND_PHENOMENON.code);

        const pathPhenomenon = `${userDir}\\phenomenon`;
        const getPhenomenonResult = await amqp.getPhenomenon({
            resultFolder: pathPhenomenon,
            leftUpper: {
                latitude: coord[2],
                Longitude: coord[1]
            },
            rightLower: {
                latitude: coord[0],
                longitude: coord[3]
            },
            phenomenon: RESEARCHES[research].type,
            dataFolders: arrayLandsat
        });


        if(!getPhenomenonResult.isDetermined){
            return await researchController.setStatus(researchRes.id, STATE.NO_FIND_PHENOMENON.code);
        }

        await researchController.setPhenomenonResultFolder(researchRes.id, pathPhenomenon);
        await researchController.setStatus(researchRes.id, STATE.DOWNLOAD_DATA_FOR_CHARACTERISTICS.code);


        const needSatellitesForCharacteristics = {};

        const characteristics = RESEARCHES[research].characteristics;

        characteristics.forEach( async characteristicName => {
            const satellite = CHARACTERISTICS[characteristicName].satellite;

            // Создадим папки для хар-к
            await createCharacteristicFolder(userDir, CHARACTERISTICS[characteristicName].folder);
            needSatellitesForCharacteristics[satellite] = '';
        });
        // Скачаем данные для каждого спутника
        const needSatellites = Object.keys(needSatellitesForCharacteristics);

        for (let i = 0; i < needSatellites.length; i++) {
            const satellite = needSatellites[i];
            if (satellite === 'LANDSAT') { // Мы получали данные перед обнуружением явления
                needSatellitesForCharacteristics[satellite] = arrayLandsat[arrayLandsat.length - 1];
                continue;
            }
            const serviceName = satellites[satellite].service;
            const satelliteName = satellites[satellite].name;
            switch (serviceName) {
                case 'SRTM': {
                    const srtmResult = await SERVICES[serviceName].getDownloadLink(satelliteName, coord);
                    needSatellitesForCharacteristics[satellite] = (await _downloadAsync(uuid(), srtmResult))[0];

                    break;
                }
                case 'USGS': {
                    const usgsResult = await SERVICES[serviceName].getDownloadLink(satelliteName, startData, endData, 1, coord, cloudMax, month);
                    needSatellitesForCharacteristics[satellite] = (await _downloadAsync(uuid(), usgsResult))[0];

                    break;
                }
            }
        }


        await researchController.setStatus(researchRes.id, STATE.FIND_CHARACTERISTICS.code);
        const characteristicsResult = await amqp.getCharacteristics({
            phenomenonType: RESEARCHES[research].type,
            characteristics: characteristics.map(character => {
                const ch = CHARACTERISTICS[character];
                const sat = ch.satellite;
                let dataFolder = needSatellitesForCharacteristics[sat];
                if (character === 'AREA_OF_DAMAGE') {
                    dataFolder = `${userDir}\\phenomenon`
                }

                return {
                    satelliteType: satellites[sat].type,
                    dataFolder:dataFolder,
                    resultFolder: `${userDir}\\characteristics\\${ch.folder}`,
                    characteristicType: ch.type
                }
            }),
            leftUpper: {
                latitude: coord[2],
                Longitude: coord[1]
            },
            rightLower: {
                latitude: coord[0],
                longitude: coord[3]
            }
        });

        console.log(characteristicsResult);
        await researchController.setStatus(researchRes.id, STATE.COMPLETED.code);
    }, 10);


    return researchRes;
}


async function _downloadAsync(requestId, data) {
    return await new Promise((resolve, reject) => {
        downloadScheduler.addToQueDownload({requestId, data}, (err, arrayPath) => {
            if (err) {
                return reject(err);
            }
            resolve(arrayPath);

        });
    })
}

async function checkExistFolder(dir) {
    const exists = util.promisify(fs.exists);
    return await exists(dir);

}


async function createCharacteristicFolder(path, name){
    const dir = `${path}\\characteristics`;
    const mkdir = util.promisify(fs.mkdir);
    const exists = util.promisify(fs.exists);

    const tempPath = `${dir}\\${name}`;

    if (!(await exists(tempPath))) {
        await mkdir(tempPath);
    }

}

async function createUserResFolders(uuid) {
    const dir = `${config.resultUserPath}\\${uuid}`;
    const mkdir = util.promisify(fs.mkdir);
    const exists = util.promisify(fs.exists);

    if (!(await exists(dir))) {
        await mkdir(dir);
    }
    const phenomenonDir = `${dir}\\phenomenon`;
    const characteristicsDir = `${dir}\\characteristics`;

    if (!(await exists(phenomenonDir))) {
        await mkdir(phenomenonDir);
    }
    if (!(await exists(characteristicsDir))) {
        await mkdir(characteristicsDir);
    }

    return dir;
}

module.exports = {
    handleResearch
};