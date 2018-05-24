const decompress = require('decompress');
const config = require('config');


// decompress(`${config.downloadFolderPath}\\ura.tar.gz`, `${config.unzipFolderPath}`).then(files => {
//     console.log('done!');
// });

module.exports = {
    decompress: (pathInput, pathOutput)=> {
        return decompress(pathInput, pathOutput)
    }
};