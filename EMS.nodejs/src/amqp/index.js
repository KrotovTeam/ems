const events = require('events');
const eventEmitter = new events.EventEmitter();
const config = require('config');
const amqp = require('amqplib/callback_api');
const emsDataNormalizationServiceChannel = config.amqp.channels.EMS_DATA_NORMALIZATION_SERVICE;
const resultChannel = config.amqp.channels.result;
const errorChannel = config.amqp.channels.error;

const RECONNECT_TIME = 7000;
let channel = null;

function createChannel(connection) {
    connection.createChannel((err, ch) => {
        channel = ch;
        ch.on('error', err => {
            console.error('[AMQP] channel error', err.message);
        });
        ch.on('close', () => {
            console.error('[AMQP] channel closed');
            createChannel(connection);
        });

        // создаем необходимые очереди
        ch.assertQueue(resultChannel, {durable: true, noAck: true});
        ch.assertQueue(errorChannel, {durable: true, noAck: true});
        ch.assertQueue(emsDataNormalizationServiceChannel, {durable: true, noAck: true});
        // ch.prefetch(1);
        // подписываемся на получения транзакциий, которые нужно выполнить
        // ch.consume(emsDataNormalizationServiceChannel, msg => {
        //     const message = JSON.parse(msg.content);
        //     console.log();
        //     if (message.operation) {
        //         console.log('Получили транзакцию, которую надо выполнить', message);
        //         eventEmitter.emit(message.operation, message, msg);
        //     }
        // });
    });
}

/**
 * Функция подключения к rabbitMQ
 */
function connect() {
    amqp.connect(config.amqp.rabbitMQ.url + '?heartbeat=60', (err, conn) => {
        if (err) {
            return setTimeout(connect, RECONNECT_TIME);
        }
        // Подписываемся на событие ошибок
        conn.on('error', err => {
            if (err.message !== 'Connection closing') {
                console.error('[AMQP] conn error', err.message);
            }
        });
        // Подписываемся событие закрытия соединения
        conn.on('close', () => {
            return setTimeout(connect, RECONNECT_TIME);
        });
        // создаем канал
        createChannel(conn);
    });
}

connect();

module.exports = {
    pushToEmsDataNormalizationServiceChannel: async data=>{
        if (!channel) {
            console.error(`Channel for connect ampq server not created`);
            return false;
        }
        try {
            console.log('Отправляем результат транзакции', data);
            return await channel
                .sendToQueue(emsDataNormalizationServiceChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
        } catch (err) {
            console.error(`Не удалось записать ${JSON.stringify(data)} в очередь ${emsDataNormalizationServiceChannel} ${err}`);
        }
        return false;
    },
    listenPending: (event, cb) => {
        eventEmitter.on(event, cb);
    },
    confirmPendingTransaction: msg => {
        return channel.ack(msg);
    }
};