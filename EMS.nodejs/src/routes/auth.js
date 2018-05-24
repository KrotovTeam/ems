const router = require('express').Router();

const authorizationService = require('../services/auth/services/users');

router.post('/authenticate', async (req, res, next) => {
    req.result = await authorizationService.authenticate({
        username: req.body.username,
        password: req.body.password
    });
    req.result.username = req.body.username;

    next();
});

router.post('/refreshToken', async (req, res, next) => {
    req.result = await authorizationService.refreshToken({
        token: req.body.token
    });

    next();
});


module.exports = router;
