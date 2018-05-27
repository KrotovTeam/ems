const express = require('express');
const router = express.Router();

router.get('/', (req, res) => {
    //console.log(JSON.parse(req.cookies.user));

    const username = req.cookies.user ? JSON.parse(req.cookies.user).username : null;

    res.render('index', {username: username});
});

router.get('/index', (req, res) => {
    const username = req.cookies.user ? JSON.parse(req.cookies.user).username : null;
    res.render('index', {username: username});
});

module.exports = router;