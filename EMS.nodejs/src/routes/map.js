const express = require('express');
const router = express.Router();

router.get('/', (req, res) => {
    const username = req.cookies.user ? JSON.parse(req.cookies.user).username : null;
    res.render('map', {username: username});
});
module.exports = router;