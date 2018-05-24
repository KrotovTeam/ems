const express = require('express');
const app = express();
let path = require('path');
let cookieParser = require('cookie-parser');
const bodyParser = require('body-parser');
let exprressSession = require('express-session');
const jwt = require('./src/middlewares/jwt');
const authorizedJWT = require('./src/middlewares/authorizedJWT');
const successHandler = require('./src/middlewares/successHandler');
const errorHandler = require('./src/middlewares/errorHandler');
//const mm = require('../diplom/src/services/auth/services/users');

app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'src/views'));

app.use(bodyParser.json());
app.use(cookieParser());
app.use(exprressSession({
    secret: 'keyboard cat',
    saveUninitialized: true
}));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: true}));
app.use(express.static(path.join(__dirname, 'src/views'))); //Проброс всех ресурсов для сайта
app.use(express.static(path.join(__dirname, 'external'))); //Проброс всех ресурсов для сайта
app.set('view engine', 'ejs');

app.use('/', require('./src/routes/index'));
app.use('/auth', require('./src/routes/auth'));
app.use('/getImageExample', require('./src/routes/getImageExample'));
app.use('/map' /* jwt, authorizedJWT*/, require('./src/routes/map'));
app.use('/research' /* jwt, authorizedJWT*/, require('./src/routes/research'));

// подключаем обработчик успешных запросов (для форматирования вывода)
app.use(`/`, successHandler);

// подключаем обработчик ошибок
app.use(errorHandler);

app.listen(2002, () => {
    console.log('Example app listening on port 3001!');
});