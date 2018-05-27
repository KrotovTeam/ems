'use strict';

const Sequelize = require('sequelize');

module.exports = db => {
    return db.define('requests', {
        id: {
            type: Sequelize.UUID,
            primaryKey: true,
            defaultValue: Sequelize.UUIDV4
        },
        userId:{
            type: Sequelize.INTEGER
        },
        researchName: {type: Sequelize.STRING},
        status: {type: Sequelize.INTEGER},
        result: {type: Sequelize.JSON},
        resultPath: {type: Sequelize.STRING},
        coordinateUser: {type: Sequelize.JSON},
        coordinateScene: {type: Sequelize.JSON},
        miniImagePath:{type: Sequelize.JSON},
        numberYears: {type: Sequelize.INTEGER},
        pathsDownload: {type: Sequelize.JSON},
        linksDownload: {type: Sequelize.JSON},
        phenomenonResultFolder: {type: Sequelize.STRING},
        cloudMax:{type: Sequelize.INTEGER},
        month: {type: Sequelize.INTEGER},
    });
};
