const request = require('request');
const fs = require('fs');
const https = require('https');
const httpsReqest = require('./../../src/helpers/httpRequest');
const xmlString = require('xml2js').parseString;
const config = require('config');
let API_KEY = '';
getApiKey().then(apiKey => {
    API_KEY = apiKey;
    console.log('API key: ', API_KEY);
});


function _getTimestamp(year, month, date) {
    const data = new Date(year, month - 1, date);

    return data.getTime() / 100000;
}

function _getNormalDate(timestamp) {
    const data = new Date(timestamp * 100000);

    const year = data.getFullYear();
    const month = data.getMonth() + 1;
    const date = data.getDate();
    return `${year}-${month < 10 ? '0' + month : month}-${date < 10 ? '0' + date : date}`;
}

function generateSrringDate(year, month, date) {
    return `${year}-${month < 10 ? '0' + month : month}-${date < 10 ? '0' + date : date}`;
}

function _getMonth(dateString) {
    return dateString.split('-')[1];
}

function _getDay(dateString) {
    return dateString.split('-')[2];
}

function _getYear(dateString) {
    return dateString.split('-')[0];
}

function _changeMonth(dateString, month) {
    const arr = dateString.split('-');
    arr[1] = month < 10 ? '0' + month : month;
    return arr.reduce((a, b) => {
        return a + '-' + b;
    })
}

function _changeDay(dateString, day) {
    const arr = dateString.split('-');
    arr[2] = day < 10 ? '0' + day : day;
    return arr.reduce((a, b) => {
        return a + '-' + b;
    })
}

function _changeYear(dateString, year) {
    const arr = dateString.split('-');
    arr[1] = year;
    return arr.reduce((a, b) => {
        return a + '-' + b;
    })
}

function searchTimeRandgeDownload(countYears, month, endDate) {
    // Если за текущий год по месяцу можно взять результаты
    let startSearchYear = _getYear(endDate);
    if (new Date(endDate).getTime() > new Date(_changeMonth(endDate, month)).getTime()) {
        startSearchYear = _getYear(endDate);
    } else {
        startSearchYear = _getYear(endDate) - 1;
    }
    const arrYears = [startSearchYear];
    for (let i = 1; i < countYears; i++) {
        arrYears.push(startSearchYear - i);
    }

    return arrYears.map(year => {
        return {
            startDate: generateSrringDate(year, month - 1, 1),
            endDate: generateSrringDate(year, month, 28),
        }
    })
}

function allS(x1, y1, x2, y2, x3, y3, x4, y4) {
    let nx2 = x2 - x1, nx3 = x3 - x1, nx4 = x4 - x1, nx1 = 0,
        ny2 = y2 - y1, ny3 = y3 - y1, ny4 = y4 - y1, ny1 = 0;

    if (nx3 < 0) nx3 = 0;
    if (ny3 < 0) ny3 = 0;
    if (nx3 > nx2 || ny3 > ny2) {
        return 0;
    }
    let x, y = 0;
    if (nx4 <= nx2) {
        x = nx4 - nx3
    }
    else {
        x = nx2 - nx3
    }
    if (ny4 <= ny2) {
        y = ny4 - ny3
    }
    else {
        y = ny2 - ny3
    }

    return x * y;
}

function getArrayDate(timeStamp1, timeStamp2, count) {
    const difference = timeStamp2 - timeStamp1;
    const delta = Math.round(difference / (count - 1));
    const arr = [];
    arr.push(_getNormalDate(timeStamp1));
    let tempDate = timeStamp1;
    for (let i = 1; i < count - 1; i++) {
        tempDate = tempDate + delta;
        arr.push(_getNormalDate(tempDate));
    }
    arr.push(_getNormalDate(timeStamp2));
    return arr;
}

async function getApiKey() {
    const USER_NAME = 'facelinker';
    const PASSWORD = 'ILoveBasketball1';
    const urlGetApiKey = `https://earthexplorer.usgs.gov/inventory/json/v/1.4.0/login?jsonRequest={"username": "${USER_NAME}", "password":"${PASSWORD}"}`;
    const reqData = await httpsReqest(urlGetApiKey);
    return reqData.data;
}

async function getImageInfoErrayDate(satellite, arrayDate, coord, cloudMax = 100) {
    let objRequest = {
        datasetName: satellite,
        spatialFilter: {
            filterType: "mbr",
            lowerLeft: {
                latitude: coord[0],
                longitude: coord[1]
            },
            upperRight: {
                latitude: coord[2],
                longitude: coord[3]
            }
        },
        maxCloudCover: cloudMax,
        temporalFilter: {
            dateField: "search_date", //search_date standing_request_date
            // startDate: startData,
            // endDate: endData
        },
        maxResults: 100,
        startingNumber: 1,
        sortOrder: "ASC",
        apiKey: API_KEY,
        publicOnly: false
    };

    const resObj = {};

    for (let i = 0; i < arrayDate.length; i++) {
        const date = arrayDate[i];
        objRequest.temporalFilter.startDate = date.startDate;
        objRequest.temporalFilter.endDate = date.endDate;
        const urlSearh1 = 'https://earthexplorer.usgs.gov/inventory/json/v/1.4.0/search?jsonRequest=' + JSON.stringify(objRequest);
        const searchResponse = await httpsReqest(urlSearh1);
        resObj[date.endDate] = searchResponse.data.results;
    }

    return resObj;
}

async function getImageInfo(satellite, startData, endData, coord, cloudMax = 100) {
    let apiKey = await getApiKey();
    console.log('API key: ', apiKey);
    let objRequest = {
        datasetName: satellite,
        // datasetName: "Sentinel_2A",
        // datasetName: "Modis_MOD09GQ",
        // datasetName: "Modis_MOD09GA",
        // datasetName: "Modis_MYD09A1",
        spatialFilter: {
            filterType: "mbr",
            lowerLeft: {
                latitude: coord[0],
                longitude: coord[1]
            },
            upperRight: {
                latitude: coord[2],
                longitude: coord[3]
            }
        },
        maxCloudCover: cloudMax,
        temporalFilter: {
            dateField: "search_date", //search_date standing_request_date
            startDate: startData,
            endDate: endData
        },
        // additionalCriteria: {
        //     filterType: "and",
        //     childFilters: [
        //         {filterType: "between", fieldId: 10036, firstValue: "22", secondValue: "24"},
        //         {filterType: "between", fieldId: 10038, firstValue: "38", secondValue: "40"}
        //     ]
        // },
        maxResults: 100,
        startingNumber: 1,
        sortOrder: "ASC",
        apiKey: apiKey,
        publicOnly: false
    };

    const urlSearh1 = 'https://earthexplorer.usgs.gov/inventory/json/v/1.4.0/search?jsonRequest=' + JSON.stringify(objRequest);
    const searchResponse = await httpsReqest(urlSearh1);

    return searchResponse.data.results;
}


function _getElementArea(e, coordSceneUser) {
    const sceneCoord = e.sceneBounds.split(',');
    const x1 = parseFloat(sceneCoord[0]);
    const y1 = parseFloat(sceneCoord[1]);
    const x2 = parseFloat(sceneCoord[2]);
    const y2 = parseFloat(sceneCoord[3]);



    const areaScene = allS(x1, y1, x2, y2, x1, y1, x2, y2,);
    const all = allS(x1, y1, x2, y2, coordSceneUser[1], coordSceneUser[0], coordSceneUser[3], coordSceneUser[2]);
    console.log(areaScene, '_', all);
    return allS(x1, y1, x2, y2, coordSceneUser[1], coordSceneUser[0], coordSceneUser[3], coordSceneUser[2])
}

function _getWrs(displayId) {
    return displayId.split('_')[2];
}

function _getUTM(displayId) {
    let arrDisplayId = displayId.split('_')[1];

    for (let i = 0; i < arrDisplayId.length; i++) {
        if (arrDisplayId[i].length === 6 && arrDisplayId[i][0] === 'T') {
            return arrDisplayId[i];
        }
    }
    return arrDisplayId;

}

function getNeedWRSScene(resByMaxArea, res) {
    for (let i = 0; i < resByMaxArea.length; i++) {
        const maxWRS2String = _getWrs(resByMaxArea[i].displayId);
        let isFindScene = true;
        for (let j = 0; j < res.length; j++) {
            if (!checkWrs(maxWRS2String, res[j])) {
                isFindScene = false
            }
        }

        if (isFindScene) {
            return maxWRS2String
        }
    }
    return null;
}

function getNeedUTMScene(resByMaxArea, res) {
    for (let i = 0; i < resByMaxArea.length; i++) {
        const maxWRS2String = _getUTM(resByMaxArea[i].displayId);
        let isFindScene = true;
        for (let j = 0; j < res.length; j++) {
            if (!checkUTM(maxWRS2String, res[j])) {
                isFindScene = false
            }
        }

        if (isFindScene) {
            return maxWRS2String
        }
    }
    return null;
}

function checkWrs(WRS, arr) {
    let result = false;
    for (let i = 0; i < arr.length; i++) {
        if (_getWrs(arr[i].displayId) === WRS) {
            result = true;
        }
    }
    return result;
}

function checkUTM(UTM, arr) {
    let result = false;
    for (let i = 0; i < arr.length; i++) {
        if (_getUTM(arr[i].displayId) === UTM) {
            result = true;
        }
    }
    return result;
}


function getUniqSceneId(satellite) {
    switch (satellite) {
        case 'Sentinel_2A':
            return _getUTM;
        default:
            return _getWrs
    }
}

function getNeedScene(satellite) {
    switch (satellite) {
        case 'Sentinel_2A':
            return getNeedUTMScene;
        default:
            return getNeedWRSScene
    }
}

function checkSceneId(satellite) {
    switch (satellite) {
        case 'Sentinel_2A':
            return checkUTM;
        default:
            return checkWrs
    }
}


function check(coordScene, coordUser){
    const scene = {
        leftUpper : {
            lat: coordScene[0][1],
            lon: coordScene[0][0],
        },
        leftDown : {
            lat: coordScene[1][1],
            lon: coordScene[1][0],
        },
        rightUpper: {
            lat: coordScene[2][1],
            lon: coordScene[2][0],
        },
        rightDown : {
            lat:  coordScene[3][1],
            lon: coordScene[3][0],
        }
    };
    const user = {
        leftUpper : {
            lat: coordUser[2],
            lon: coordUser[1],
        },
        leftDown : {
            lat: coordUser[0],
            lon: coordUser[1],
        },
        rightUpper: {
            lat: coordUser[2],
            lon: coordUser[3],
        },
        rightDown : {
            lat: coordUser[0],
            lon: coordUser[3],
        }
    };
    // Для нижней левой точки находим 2 точки меньше по Lat
    let lat = user.leftDown.lat;
    let lon = user.leftDown.lon;
    let countTemp = 0;

    Object.keys(scene).forEach(e=>{
        const latS = scene[e].lat;
        const lonS = scene[e].lon;
        if(lat>latS){
            countTemp++;
        }

        if(lon>lonS){
            countTemp++;
        }
    });
    if(countTemp!==4){
        return false;
    }

    // Для нижней правой точки находим 2 точки меньше по Lat
    lat = user.rightDown.lat;
    lon = user.rightDown.lon;
    countTemp = 0;

    Object.keys(scene).forEach(e=>{
        const latS = scene[e].lat;
        const lonS = scene[e].lon;
        if(lat>latS){
            countTemp++;
        }
        if(lon<lonS){
            countTemp++;
        }
    });
    if(countTemp!==4){
        return false;
    }

    // Для верхней левой точки находим 2 точки меньше по Lat
    lat = user.rightUpper.lat;
    lon = user.rightUpper.lon;
    countTemp = 0;

    Object.keys(scene).forEach(e=>{
        const latS = scene[e].lat;
        const lonS = scene[e].lon;
        if(lat<latS){
            countTemp++;
        }
        if(lon>lonS){
            countTemp++;
        }
    });
    if(countTemp!==4){
        return false;
    }
    // Для верхней правой точки находим 2 точки меньше по Lat
    lat = user.leftUpper.lat;
    lon = user.leftUpper.lon;
    countTemp = 0;

    Object.keys(scene).forEach(e=>{
        const latS = scene[e].lat;
        const lonS = scene[e].lon;
        if(lat<latS){
            countTemp++;
        }
        if(lon<lonS){
            countTemp++;
        }
    });
    if(countTemp!==4){
        return false;
    }

    return true;
}
async function getDownloadLink(satellite, startData, endData, countYears = 2, coord = [59, 37, 59, 38], cloudMax = 100, month) {
    //const res = await getImageInfo(satellite, startData, endData, coord, cloudMax);
    const res = await getImageInfoErrayDate(satellite, searchTimeRandgeDownload(countYears, month, endData), coord, cloudMax);

    const keysDate = Object.keys(res);


    for(let i = 0; i<keysDate.length;i++){
        if(res[keysDate[i]].length===0){
            return [];
        }
    }

    console.log(res.length);

    // Формируем объект уникальных сцен для первого элемента, для пересичения
    const listUnikScene = {};

    keysDate.forEach(date=>{
        res[date].forEach(e => {
            const sceneId = getUniqSceneId(satellite)(e.displayId);
            if (!listUnikScene[sceneId]) {
                const initValue = {};
                for (let i = 0; i < keysDate.length; i++) {
                    initValue[keysDate[i]] = 0;
                }
                listUnikScene[sceneId] = {};
                const coordScene = e.spatialFootprint.coordinates[0].slice(1);
                listUnikScene[sceneId].coord = coordScene;
                listUnikScene[sceneId].years = initValue;
            }
        })
    });

    for (let i = 0; i < keysDate.length; i++) {
        const date = keysDate[i];
        const arr = res[date];
        for (let j = 0; j < arr.length; j++) {
            const sceneId = getUniqSceneId(satellite)(arr[j].displayId);
            if (listUnikScene[sceneId].years) {
                listUnikScene[sceneId].years[date]++;
            }
        }
    }

    // Сформируем массив индентификаторов сцен которые есть во всех годах
    const commonScenes = Object.keys(listUnikScene).filter(sceneID => {
        const dates = Object.keys(listUnikScene[sceneID].years);

        for (let i = 0; i < dates.length; i++) {
            if (listUnikScene[sceneID].years[dates[i]] === 0) {
                return false;
            }
        }
        return true;
    });

    console.log('commonScenes', commonScenes);


    // Отфильтруем сцены так что выбраная область не вылазила за пределы сцены
    const needScenes = commonScenes.filter(sceneId =>{
        return check(listUnikScene[sceneId].coord, coord);
    });

    if (needScenes.length === 0){
        return [];
    }


    let needIdScene = needScenes[0];


    // const resByMaxArea = res[Object.keys(res)[0]].sort((a, b) => {// Сортируем по убыванию размеру общей площади
    //     if (_getElementArea(a, coord) < _getElementArea(b, coord)) {
    //         return 1;
    //     }
    //     if (_getElementArea(a, coord) > _getElementArea(b, coord)) {
    //         return -1;
    //     }
    //     return 0;
    // });
    //
    //
    // let needIdScene = getNeedScene(satellite)(resByMaxArea, res);


    const needDownload = [];
    Object.keys(res).forEach(key => {
        const findElement = res[key].find(e => {
            if (getUniqSceneId(satellite)(e.displayId) === needIdScene) {
                return true;
            }
        });

        needDownload.push(findElement);

    });

    console.log(needDownload);

    // for (let i = 0; i < needDownload.length; i++) {
    //     if (!needDownload[i]) {
    //         return [];
    //     }
    // }

    const filterResByCount = needDownload;
    const resData = [];

    for (let i = 0; i < filterResByCount.length; i++) {
        const element = filterResByCount[i];

        const linkDownloadArchive = getDownloadArchive(element.entityId, element.metadataUrl);
        const imagePath = await downloadSaveImage(element.browseUrl);
        const modifiedDate = element.modifiedDate;
        // const metaData = await downloadSaveMetaData(element.fgdcMetadataUrl);
        console.log(linkDownloadArchive);
        resData.push({
            orig: element,
            linkDownloadArchive,
            imagePath,
            modifiedDate,
            uniqIdScene: needIdScene,
            satellite
        });
    }
    return resData;
}

function downloadSaveMetaData(downloadLink) {
    return new Promise((resolve, reject) => {
        const stream = request
            .get(downloadLink)
            .pipe(fs.createWriteStream(`${config.testMetaDataFolderPath}\\${_makeid()}.xml`));
        stream.on('finish', function () {
            fs.readFile(this.path, function (err, data) {
                xmlString(data, {explicitArray: false, explicitRoot: true}, (err, res) => {
                    console.log(res);
                })
            });

            resolve()
        });
    })
}

function downloadSaveImage(downloadLink) {
    return new Promise((resolve, reject) => {
        const stream = request
            .get(downloadLink)
            .pipe(fs.createWriteStream(`${config.exampleImageFolderPath}\\${_makeid()}.jpg`));
        stream.on('finish', function () {
            resolve(this.path)
        });
    })
}


function getDownloadArchive(entityId, metadataUrl) {
    const id = metadataUrl.slice(metadataUrl.indexOf('xml/') + 4, metadataUrl.lastIndexOf(entityId) - 1);
    const link = `https://earthexplorer.usgs.gov/download/${id}/${entityId}/STANDARD/INVSVC`;

    return link;
}

function _makeid() {
    let text = "";
    let possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (let i = 0; i < 5; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

module.exports = {
    getDownloadLink
};


