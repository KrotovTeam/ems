'use strict';

const config = require('config');
const Sequelize = require('sequelize');

const Op = Sequelize.Op;

// выполняем создание объекта для работы с БД
const db = new Sequelize(config.db.options);

// Хранит список администраторов платформы
let Users = require('./users')(db);
let Requests = require('./requests')(db);

const initialize = force => {
    db.authenticate()
        .then(async () => {
            console.log('Connection has been established successfully.');
            // выполняет создание таблиц, если их не существует или если признак force=true
            Users = await Users.sync({force});
            const bcrypt = require('bcryptjs');
            await Users.create({
                userId: 1,
                username: 'user1',
                password: bcrypt.hashSync('12345', config.auth.salt),
                email: 'user1@gmail.com'
            });
            Requests = await Requests.sync({force});
            Users.hasMany(Requests, {foreignKey: 'userId'});
            console.log();
        })
        .catch(err => {
            console.error('Unable to connect to the database:', err);
            throw err;
        });
};

if (module.parent) {
    initialize(false);
    module.exports = {
        Op,
        Users,
        Requests
    };
} else {
    initialize(true);
}
