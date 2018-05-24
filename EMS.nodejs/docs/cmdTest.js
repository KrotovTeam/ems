const { exec } = require('child_process');

exec('start https://earthexplorer.usgs.gov/download/12864/LC81810192017037LGN00/STANDARD/INVSVC', (error, stdout, stderr) => {
    if (error) {
        console.error(`exec error: ${error}`);
        return;
    }
    console.log(`stdout: ${stdout}`);
    console.log(`stderr: ${stderr}`);
});