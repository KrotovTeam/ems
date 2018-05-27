const path = require('path');

module.exports = {
    phpSessionId: 'mlkhrrp1m9fm0f7tci905i29r5',
    downloadFolderPath: path.resolve('./external/download'),
    unzipFolderPath: path.resolve('./external/unzip'),
    exampleImageFolderPath: path.resolve('./external/image'),
    testMetaDataFolderPath: path.resolve('./external/testMetaData'),
    sortDownloadDataPath: path.resolve('./external/sortDownloadData'),
    resultUserPath:path.resolve('./external/resultUser'),
    amqp: {
        rabbitMQ: {
            url: 'amqp://localhost'
        },
        channels: {
            PHENOMENON_REQUEST: 'EMS.DeterminingPhenomenonService.Requests',
            PHENOMENON_RESULT: 'EMS.DeterminingPhenomenonService.Responses',
            CALIBRATE_REQUEST: 'EMS.DataNormalizationService.Requests',
            CALIBRATE_RESULT: 'EMS.DataNormalizationService.Responses',
            CHARACTERISTICS_REQUEST: 'EMS.CharacterizationService.Request',
            CHARACTERISTICS_RESULT: 'EMS.CharacterizationService.Responses',
            EMS_DATA_RELIEF_MODEL_SERVICE_REQUESTS: 'EMS.ReliefModelService.Requests',
            EMS_DATA_RELIEF_MODEL_SERVICE_RESPONSES: 'EMS.ReliefModelService.Responses',
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