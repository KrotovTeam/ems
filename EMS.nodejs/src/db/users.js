'use strict';

const Sequelize = require('sequelize');

module.exports = db => {
    return db.define('users', {
        userId: {
            primaryKey: true,
            type: Sequelize.INTEGER,
            autoIncrement: true
        },
        username: {type: Sequelize.STRING},
        password: {type: Sequelize.STRING},
        email: {type: Sequelize.STRING},
        firstName: {type: Sequelize.STRING},
        secondName: {type: Sequelize.STRING},
        lastName: {type: Sequelize.STRING}
    });
};
