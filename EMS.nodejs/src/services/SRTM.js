// const http = require('https');
// const fs = require('fs');
// const file = fs.createWriteStream("file.zip");
// // const request = http.get(`https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/N00E072.hgt.zip`, function (response) {
// //     response.pipe(file);
// // });

function addZeroes(number, countZero) {
    let str = '';
    number += '';
    let l = number.length;
    while (l < countZero) {
        str += '0';
        l++;
    }
    return str + number;

}

function getSrtmNameFiles(y1, x1, y2, x2) {
    const latitudes = [];
    const longtitudes = [];


    const leftUpper = {
        Latitude: y1,
        Longitude: x1
    };
    const rigthLower = {
        Latitude: y2,
        Longitude: x2
    };

    const namesResult = [];
    for (let i = Math.trunc(leftUpper.Latitude); i >= Math.trunc(rigthLower.Latitude); i--) {
        latitudes.push(i);
    }

    for (let i = Math.trunc(leftUpper.Longitude); i <= Math.trunc(rigthLower.Longitude); i++) {
        longtitudes.push(i);
    }


    for (let y = 0; y < latitudes.length; y++) {
        let latitudeLetter = latitudes[y] > 0
            ? "N"
            : "S";
        for (let x = 0; x < longtitudes.length; x++) {
            let longitudeLetter = longtitudes[x] > 0
                ? "E"
                : "W";

            let name = `${latitudeLetter}${addZeroes(Math.abs(latitudes[y]), 2)}${longitudeLetter}${addZeroes(Math.abs(longtitudes[x]), 3)}.hgt.zip`;
            namesResult.push(name);
        }
    }
    return namesResult;
}


function getDownloadLink(satellite, coord) {
    const res = getSrtmNameFiles(coord[2], coord[1], coord[0], coord[3]);

    return res.map(name => {
        return {
            satellite,
            linkDownloadArchive: `https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/${name}`,
            nameFile: name
        }
    });
}


//console.log(getSrtmNameFiles());
module.exports = {
    getDownloadLink
};