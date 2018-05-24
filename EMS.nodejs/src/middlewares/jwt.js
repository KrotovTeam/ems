'use strict';

module.exports = (req, res, next) => {
    if (req.method === 'OPTIONS' && req.headers.hasOwnProperty('access-control-request-headers')) {
        if (!!~req.headers['access-control-request-headers'].split(',').map(header => header.trim()).indexOf('authorization')) {
            return next();
        }
    }

    if (req.headers && req.headers.authorization) {
        let parts = req.headers.authorization.split(' ');
        if (parts.length === 2) {
            let scheme = parts[0];
            let credentials = parts[1];

            if (/^Bearer$/i.test(scheme)) {
                req.jwtToken = credentials;
            }
        }
    }

    next();
};
