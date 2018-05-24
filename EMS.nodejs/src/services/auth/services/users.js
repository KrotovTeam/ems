'use strict';

const boom = require('boom');
const bcrypt = require('bcryptjs');

// подключаем функции для упрощения работы
// initLoggedClient - выполняет создание клиента микросервиса
// validate - выполняет проверку входных параметров на соответствие описанной JSON-схеме
const {validate, errors} = require('./../../../../utils');

// подключаем схемы валидации
const authenticateScheme = require('./../schemes/authenticateScheme');
// подключаем контроллер для работы с БД и кэшем
const UsersController = require('./../controllers/users');

module.exports = {
    /**
     * Ф-я авторизации пользователя по логину и паролю
     * @param {Object} req Объект, содержащий данные запроса
     * @return {Object}
     */
    authenticate: async req => {
        validate(req, authenticateScheme);

        const profile = await UsersController.getProfile(req.username);
        if (!bcrypt.compareSync(req.password, profile.password)) {
            throw boom.notFound(errors.notFound.USER_NOT_FOUND.code);
        }

        return await UsersController.generateToken(profile);
    },
    /**
     * Ф-я выполняет обновление токена доступа
     * @param {Object} req Объект, содержащий данные запроса
     * @return {Promise}
     */
    refreshToken: async req => await UsersController.refreshToken(req.token),
    /**
     * Ф-я выполняет выход пользователя из системы (kill session)
     * @param {Object} req Объект, содержащий данные запроса
     * @return {Promise}
     */
    logout: async req => await UsersController.logout(req.token),
    /**
     * Ф-я выполняет проверку прав доступа пользователя
     * @param {Object} req Объект, содержащий данные запроса
     * @return {Promise}
     */
    checkAccess: async req => await UsersController.checkAccess(req.token, req._action),
};
