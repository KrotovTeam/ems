const events = require('events');
const eventEmitter = new events.EventEmitter();
const schedule = require('node-schedule');
const downloadLandsat = require('./downloadLandsat');
const config = require('config');
const unzip = require('./unzip');
const FGDC = require('./FGDC');

const fs = require('fs');
const util = require('util');


const needDownload = [];
let isBusy = false;
// every 10 seconds
schedule.scheduleJob('*/10 * * * * *', async () => {
    if (!isBusy && needDownload.length > 0) {
        isBusy = true;

        let downloadRes = null;

        try {
            downloadRes = await Promise.all(needDownload[0].data.map(getDownloadPathImage));
        } catch (e) {
            console.error(e, 'Не удалось скачать снимки');
        }

        eventEmitter.emit(needDownload[0].requestId, null, downloadRes);

        console.log(downloadRes);
        needDownload.shift();
        isBusy = false;
    }
});




async function getDownloadPathImage(downloadInfo){
    switch (downloadInfo.satellite){
        case 'Landsat_8_C1':{
            const resCheck = await checkAndGetExistPath(`${downloadInfo.satellite}\\${downloadInfo.orig.acquisitionDate}\\${downloadInfo.uniqIdScene}`);
            const nameFile =`${downloadInfo.orig.displayId}.tar.gz`;
            if (!resCheck.status) {// Если нет, то качаем
                await downloadLandsat.downloadArchive(downloadInfo.linkDownloadArchive, config.downloadFolderPath, nameFile);
                const pathToArchive = `${config.downloadFolderPath}\\${nameFile}`;
                await unzip.decompress(pathToArchive, resCheck.path);
                await convertFGDCToJSon(resCheck.path);
            }
            return resCheck.path;
        }
        case 'Sentinel_2A':{
            const resCheck = await checkAndGetExistPath(`${downloadInfo.satellite}\\${downloadInfo.orig.acquisitionDate}\\${downloadInfo.uniqIdScene}`);
            const nameFile =`${downloadInfo.orig.displayId}.tar.gz`;
            if (!resCheck.status) {// Если нет, то качаем
                await downloadLandsat.downloadArchive(downloadInfo.linkDownloadArchive, config.downloadFolderPath, nameFile);
                const pathToArchive = `${config.downloadFolderPath}\\${nameFile}`;
                await unzip.decompress(pathToArchive, resCheck.path);
            }
            return resCheck.path;
        }
        case 'SRTM':{
            const resCheck = await checkAndGetExistPath(`${downloadInfo.satellite}\\${downloadInfo.nameFile}`, true);
            const nameFile = downloadInfo.nameFile;
            if (!resCheck.status) {// Если нет, то качаем
                await downloadLandsat.downloadArchive(downloadInfo.linkDownloadArchive, config.downloadFolderPath, nameFile);
                const pathToArchive = `${config.downloadFolderPath}\\${nameFile}`;
                await unzip.decompress(pathToArchive, resCheck.path);
            }
            return resCheck.path;
        }
    }
    return [];
}

async function checkAndGetExistPath(pathDistanation, checkFile = false) {
    const exists = util.promisify(fs.exists);
    const mkdir = util.promisify(fs.mkdir);
    const readdir = util.promisify(fs.readdir);

    let pathDistanationArray = pathDistanation.split('\\');
    let nameFile='';
    if(checkFile){
        nameFile = pathDistanationArray[pathDistanationArray.length-1];
        pathDistanationArray=  pathDistanationArray.slice(0, pathDistanationArray.length-1)
    }

    let statusExist = true;
    let tempPath = config.sortDownloadDataPath;

    for(let i = 0;i<pathDistanationArray.length;i++){
        tempPath = tempPath + '\\' + pathDistanationArray[i];
        const existPath = await exists(tempPath);
        if (!existPath) {
            if(i===pathDistanationArray.length-1){
                statusExist = false;
            }
            await mkdir(tempPath);
        }
    }

    let files = await readdir(tempPath);
    if(!checkFile){

        if (files.length === 0) {
            statusExist = false;
        }
    }else{

        if (!files.includes(nameFile)) {
            statusExist = false;
        }
    }




    return {
        status: statusExist,
        path: tempPath
    };
}

async function convertFGDCToJSon(pathFolder) {
    const readdir = util.promisify(fs.readdir);
    let files = await readdir(pathFolder);
    files = files.filter(file => file.indexOf('.txt') !== -1);

    return await Promise.all(files.map(file => FGDC.parseWithJSON(pathFolder, file)));
}


async function moveFiles(pathDistanation, pathSource) {
    const readdir = util.promisify(fs.readdir);
    const rename = util.promisify(fs.rename);

    let files = await readdir(pathSource);
    await files.map(file => rename(`${pathSource}\\${file}`, `${pathDistanation}\\${file}`));
}


module.exports = {
    addToQueDownload(obj, cb) {
        needDownload.push(obj);
        eventEmitter.on(obj.requestId, cb)
    }
};


