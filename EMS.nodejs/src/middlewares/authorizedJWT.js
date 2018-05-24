'use strict';

const boom = require('boom');
const {errors} = require('./../../utils');

module.exports = (req, res, next) => {
    // если не передан JWT-токен, то выдаем ошибку авторизации
    if (!req.jwtToken) {
        throw boom.unauthorized('');
    }

    next();
};
