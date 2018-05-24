const express = require('express');
const router = express.Router();
const {Users, Requests} = require('../db/index');
const boom = require('boom');
const {validate} = require('../../utils');
const createResearch = require('../schemes/createResearch');


router.get('/', (req, res) => {
    const username = req.cookies.user ? JSON.parse(req.cookies.user).username : null;
    res.render('research', {username});
});

router.get('/:id/details', async(req, res, next) => {
    const id = req.params.id;

    const request = await Requests.findOne({where: {id}});
    if (!request) {
        throw boom.notFound('Информация об исследование не найдена')
    }

    req.result = request;
    next();
});

router.get('/list', async (req, res, next) => {
    const username = req.cookies.user ? JSON.parse(req.cookies.user).username : null;
    // TODO небезопасный метод
    const user = await Users.findOne({ where: {username}});

    if (!user) {
        throw boom.notFound('user no found');
    }

    req.result = await user.getRequests({attributes: ['id', 'researchName', 'createdAt']});

    next();
});

router.post('/', async (req, res, next) => {
    validate(req.data, createResearch);

    const username = req.cookies.user ? JSON.parse(req.cookies.user).username : null;
    const usertoken = req.cookies.user ? JSON.parse(req.cookies.user).token : null;
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