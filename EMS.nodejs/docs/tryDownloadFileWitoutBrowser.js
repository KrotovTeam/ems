const fs = require('fs');
const request = require('request');
const progress = require('request-progress');
const config = require('config');

const jar = request.jar();
const cookie = request.cookie(`PHPSESSID=${config.phpSessionId}`);
const url = 'https://earthexplorer.usgs.gov/download/12864/LC81780282015203LGN01/STANDARD/INVSVC';
jar.setCookie(cookie, url);
const headers = {
    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.167 Safari/537.36'
};

progress(
    request({
        url,
        jar,
        headers
    }))
    .on('progress', state => {
        // {
        //     percent: 0.5,               // Overall percent (between 0 to 1)
        //     speed: 554732,              // The download speed in bytes/sec
        //     size: {
        //         total: 90044871,        // The total payload size in bytes
        //         transferred: 27610959   // The transferred payload size in bytes
        //     },
        //     time: {
        //         elapsed: 36.235,        // The total elapsed seconds since the start (3 decimals)
        //         remaining: 81.403       // The remaining seconds to finish (3 decimals)
        //     }
        // }
        console.log('progress', state);
    })
    .on('error', err => {
        console.error(err);
    })
    .on('end', () => {
        console.log('downloadEnded!!!!')
    })
    .pipe(fs.createWriteStream('ura.tar.gz'));

module.exports = {
    downloadArchive: async (url, nameFile = 'ura.tar.gz', cbProgress) => {
        return await new Promise((resolve, reject)=>{
            progress(
                request({
                    url,
                    jar,
                    headers
                }))
                .on('progress', state => {
                    // {
                    //     percent: 0.5,               // Overall percent (between 0 to 1)
                    //     speed: 554732,              // The download speed in bytes/sec
                    //     size: {
                    //         total: 90044871,        // The total payload size in bytes
                    //         transferred: 27610959   // The transferred payload size in bytes
                    //     },
                    //     time: {
                    //         elapsed: 36.235,        // The total elapsed seconds since the start (3 decimals)
                    //         remaining: 81.403       // The remaining seconds to finish (3 decimals)
                    //     }
                    // }
                    console.log('progress', state.percent);
                    if(typeof cbProgress === 'function'){
                        cbProgress(state);
                    }
                })
                .on('error', err => {
                    return reject(err);
                })
                .on('end', () => {
                    resolve();
                })
                .pipe(fs.createWriteStream(nameFile));
        });
    }
};
