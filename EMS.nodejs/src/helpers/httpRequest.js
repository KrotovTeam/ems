const https = require('https');

module.exports = options => {
    return new Promise((resolve, reject) => {
        https.get(options, (res) => {
            if (res.statusCode !== 200) {
                reject();
            }

            let output = '';

            res.on('data', chunk => {
                output += chunk;
            });

            res.on('end', function () {
                let obj = JSON.parse(output);
                resolve(obj);
            });

        })
            .on('error', err => {
                reject(err);
            });
    });
};