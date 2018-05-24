'use strict';

const config = require('config');
const jwt = require('jsonwebtoken');
const uuid = require('uuid/v4');
const boom = require('boom');
const redis = require('redis');

// подключаем объекты для работы с БД
const {Users} = require('./../../../db/index');
const {errors} = require('./../../../../utils');

// подключаем кэш для хранения сессий пользователей
const sessionCache = redis.createClient(config.redis);

module.exports = {
    /**
     * Ф-я выполняет поиск информации о пользователе по логину и паролю
     * @param {string} username Логин пользователя
     * @return {Object}
     */
    getProfile: async username=> {
        let profile = await Users.findOne({where: {username}});
        if (profile && profile.userId) {
            return profile;
        }
        throw boom.notFound(errors.notFound.USER_NOT_FOUND.code);
    },
    /**
     * Ф-я выполняет создание токена доступа по профилю и списку ролей
     * @param {string} profile Профиль пользователя
     * @return {{userId: string, token: string}}
     */
    generateToken: async profile => {
        // создаем новую сессию
        const sessionIdentifier = uuid();
        sessionCache.set(sessionIdentifier, JSON.stringify(profile), 'EX', config.session.expiresIn);

        return {
            token: jwt.sign({
                sessionIdentifier,
            }, config.jwt.secret, {expiresIn: config.jwt.expiresIn})
        };
    },
    /**
     * Ф-я выполняет обновление токена доступа
     * @param {string} token Токен доступа пользователя
     * @return {Promise}
     */
    refreshToken: async token => {
        return new Promise((resolve, reject) => {
            jwt.verify(token, config.jwt.secret, {ignoreExpiration: true}, (err, decoded) => {
                if (err) return reject(boom.forbidden());
                sessionCache.get(decoded.sessionIdentifier, (err, result) => {
                    if (err) return reject(err);
                    let profile = JSON.parse(result);
                    if (!profile) {
                        return reject(boom.forbidden());
                    }
                    // удаляем старую сессию пользователя
                    sessionCache.del(decoded.sessionIdentifier);
                    // создаем новую сессию
                    const sessionIdentifier = uuid();
                    sessionCache.set(sessionIdentifier, JSON.stringify(profile), 'EX', config.session.expiresIn);

                    resolve({
                        token: jwt.sign({
                            sessionIdentifier
                        }, config.jwt.secret, {expiresIn: config.jwt.expiresIn})
                    });
                });
            });
        });
    },
    /**
     * Ф-я выполняет выход пользователя из системы (kill session)
     * @param {string} token Токен доступа пользователя
     * @return {Promise}
     */
    logout: async token => {
        return new Promise((resolve, reject) => {
            jwt.verify(token, config.jwt.secret, (err, decoded) => {
                if (err) return reject(boom.unauthorized());
                sessionCache.get(decoded.sessionIdentifier, (err, result) => {
                    if (err) return reject(err);
                    let profile = JSON.parse(result);
                    if (profile) {
                        // удаляем сессию пользователя
                        sessionCache.del(decoded.sessionIdentifier);
                    }

                    resolve({success: true});
                });
            });
        });
    },
    /**
     * Ф-я проверяет имеет ли пользователь переданную роль
     * @param {string} token Токен доступа пользователя
     * @param {Object} role Роль, наличие которой необходимо проверить
     * @return {Promise}
     */
    isInRole: async (token, role) => {
        return new Promise((resolve, reject) => {
            jwt.verify(token, config.jwt.secret, (err, decoded) => {
                if (err) return reject(boom.unauthorized());
                sessionCache.get(decoded.sessionIdentifier, (err, result) => {
                    if (err) return reject(err);
                    let profile = JSON.parse(result);
                    if (!(profile && profile.userId)) {
                        return reject(boom.unauthorized());
                    }

                    resolve({success: profile.role && profile.role === role || false});
                });
            });
        });
    }
};

