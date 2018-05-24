const decompress = require('decompress');
const config = require('config');


decompress(`${config.downloadFolderPath}\\ura.tar.gz`, `${config.unzipFolderPath}`).then(files => {
    console.log('done!');
});

module.exports = {
    decompress: nameFile => {
        return decompress(`${config.downloadFolderPath}\\${nameFile}`, `${config.unzipFolderPath}\\${nameFile}`).then(files => {
            console.log('done!');
        });
    }
};