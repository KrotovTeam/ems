const express = require('express');
const router = express.Router();
const researchService = require('../services/research');

const SATELLITE = {
    Landsat: 'Landsat_8_C1',
    Sentinel: 'Sentinel_2A',
    Modis: 'Modis_MOD09GQ',
    SRTM: 'SRTM'
};

const MONTH_ENUM = {
    'Январь': 1,
    'Февраль': 2,
    'Март': 3,
    'Апрель': 4,
    'Мая': 5,
    'Июнь': 6,
    'Июль': 7,
    'Август': 8,
    'Сентябрь': 9,
    'Октябрь': 10,
    'Ноябрь': 11,
    'Декабрь': 12
};

const RESEARCHES = {
    'Болезни лесных насаждений': 'FOREST_DISEASES',
    'Эрозия почвы': 'SOIL_EROSION',
    'Загрязнение почвы нефтепродуктами': 'SOIL_POLLUTION_BY_OIL_PRODUCTS',
    'Поверхностные свалки': 'SURFACE_DUMPS',
};

router.post('/', async (req, res, next) => {
    const username = res.locals.user.username;
    const dateStart = req.body.dateStart;
    const dateEnd = req.body.dateEnd;
    const research = RESEARCHES[req.body.research];
    const month = MONTH_ENUM[req.body.month];
    const countYears = parseInt(req.body.countYears);
    const lowerLeftLatitude = req.body.lowerLeftLatitude;
    const lowerLeftLongitude = req.body.lowerLeftLongitude;
    const upperRightLatitude = req.body.upperRightLatitude;
    const upperRightLongitude = req.body.upperRightLongitude;
    const coord = [lowerLeftLatitude, lowerLeftLongitude, upperRightLatitude, upperRightLongitude];
    const cloudMax = parseInt(req.body.cloudMax);

    req.result = await researchService.handleResearch(research, dateStart, dateEnd, countYears, coord, cloudMax, month, username);;
    next();
});

module.exports = router;