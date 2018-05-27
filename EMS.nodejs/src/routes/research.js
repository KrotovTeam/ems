const express = require('express');
const router = express.Router();
const {Users, Requests} = require('../db/index');
const boom = require('boom');
const {validate} = require('../../utils');
const createResearch = require('../schemes/createResearch');
const fs = require('fs');
const {promisify} = require('util');
const exists = promisify(fs.exists);
const readFile = promisify(fs.readFile);

router.get('/', (req, res) => {
    res.render('research');
});

router.get('/:id/details', async (req, res, next) => {
    const id = req.params.id;

    const request = await Requests.findOne({where: {id}});
    if (!request) {
        throw boom.notFound('Информация об исследование не найдена')
    }


    if(request.phenomenonResultFolder){
        const JSONFilePath = `${request.phenomenonResultFolder}\\dynamicGeoPoints.json`;
        if((await exists(JSONFilePath))){
            const data = await readFile(JSONFilePath, 'utf8');
            request.dataValues.phenomenonPoints = JSON.parse(data);
        }
    }

    req.result = request;
    next();
});

router.get('/list', async (req, res, next) => {
    const username = res.locals.user.username;
    // TODO небезопасный метод
    const user = await Users.findOne({where: {username}});
    const skip = req.query.skip ? parseInt(req.query.skip) : 0;
    const take = req.query.take ? parseInt(req.query.take) : 10;
    if (!user) {
        throw boom.notFound('user no found');
    }

    const items = await Requests.findAndCountAll({
        where: {userId: user.userId},
        attributes: ['id', 'researchName', 'createdAt'],
        order: [['createdAt', 'DESC']],
        limit: take,
        offset: skip
    });


    req.result = {
        items: items.rows,
        total: items.count
    };

    next();
});

router.post('/', async (req, res, next) => {
    validate(req.data, createResearch);

    const username = res.locals.user.username;
    // TODO небезопасный метод
    const user = Users.findOne({where: {username}});

    if (!user) {
        throw boom.notFound('user no found');
    }

    const request = await Requests.create({
        phenomenonName: req.data.phenomenonName,
        status: req.data.phenomenonName ? req.data.phenomenonName : 0,
        result: {},
        resultPath: ''
    });
    await user.addRequest(request);


    req.result = {
        success: true
    };
    next();
});

module.exports = router;