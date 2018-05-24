const path = require('path');

module.exports = {
    phpSessionId: '4ranp6p1e8ve0g9jdgctc5nvu2',
    downloadFolderPath: path.resolve('./external/download'),
    unzipFolderPath: path.resolve('./external/unzip'),
    exampleImageFolderPath: path.resolve('./external/image'),
    testMetaDataFolderPath: path.resolve('./external/testMetaData'),
    sortDownloadDataPath: path.resolve('./external/sortDownloadData'),
    amqp: {
        rabbitMQ: {
            url: 'amqp://localhost'
        },
        channels: {
            pending: 'PENDING',
            result: 'RESULT',
            error: 'ERROR'
        }
    },
    db: {
        options: {
            database: 'diplom',
            username: 'postgres',
            password: '12345',
            host: '127.0.0.1',
            port: 5432,
            dialect: 'postgres',
            logging: false
        }
    },
    jwt: {
        secret: 'q|e|"%]hlG*kef@@(m\'[PRO2pfHq*wn;+J\\d"<]L\\gLy@#jpE|\'@MLUY^zMS!N-',
        expiresIn: 3600
    },
    redis: {
        password: '12345',
        prefix: 'auth:session:'
    },
    session: {
        expiresIn: 28800
    },
    auth: {
        salt: 10
    }
};