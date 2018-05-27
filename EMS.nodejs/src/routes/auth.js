const router = require('express').Router();

const authorizationService = require('../services/auth/services/users');

router.post('/authenticate', async (req, res) => {

    let user = null;

    try {
        user = await authorizationService.authenticate({
            username: req.body.username,
            password: req.body.password
        });
    } catch (err) {

    }


    if (user) {
        user.username = req.body.username;
        req.session.user = user;
        res.redirect('/');
    } else {
        res.error('Неверный логин или пароль');
        res.redirect('/login');
    }
});
router.post('/registration', async (req, res) => {

    let user = null;

    try {
        user = await authorizationService.registration({
            username: req.body.username,
            password: req.body.password
        });
    } catch (err) {

    }


    if (user) {
        user.username = req.body.username;
        req.session.user = user;
        res.redirect('/');
    } else {
        res.error('Пользователь с таким именем уже существует!!!');
        res.redirect('/registration');
    }
});
router.post('/refreshToken', async (req, res, next) => {
    req.result = await authorizationService.refreshToken({
        token: req.body.token
    });

    next();
});
router.post('/logout', async (req, res) => {
    req.session.destroy(err => {
        if (err) throw err;
        res.redirect('/')
    });
});


module.exports = router;
