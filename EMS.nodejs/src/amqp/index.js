const events = require('events');
const eventEmitter = new events.EventEmitter();
const config = require('config');
const amqp = require('amqplib/callback_api');
const pendingChannel = config.amqp.channels.pending;
const resultChannel = config.amqp.channels.result;
const errorChannel = config.amqp.channels.error;

const RECONNECT_TIME = 7000;
let channel = null;

function createChannel(connection) {
    connection.createChannel((err, ch) => {
        channel = ch;
        ch.on('error', err => {
            logger.error('[AMQP] channel error', err.message);
        });
        ch.on('close', () => {
            logger.error('[AMQP] channel closed');
            createChannel(connection);
        });

        // создаем необходимые очереди
        ch.assertQueue(resultChannel, {durable: true});
        ch.assertQueue(errorChannel, {durable: true});
        ch.assertQueue(pendingChannel, {durable: true});
        ch.prefetch(1);
        // подписываемся на получения транзакциий, которые нужно выполнить
        ch.consume(pendingChannel, msg => {
            const message = JSON.parse(msg.content);
            if (message.operation) {
                console.log('Получили транзакцию, которую надо выполнить', message);
                eventEmitter.emit(message.operation, message, msg);
            }
        });
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
    /**
     * Ф-я публикует результат выполнения транзакции
     * @param {Object} data сообщение, которое публицируем
     * @return {Promise<boolean>}
     */
    push: async data => {
        if (!channel) {
            console.error(`Channel for connect ampq server not created`);
            return false;
        }
        try {
            console.log('Отправляем результат транзакции', data);
            return await channel
                .sendToQueue(resultChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
        } catch (err) {
            console.error(`Не удалось записать ${JSON.stringify(data)} в очередь ${resultChannel} ${err}`);
        }
        return false;
    },
    /**
     * Ф-я публикует транзакции, которые не смогли выполниться
     * @param {Object} data результат транзакции, которая не выполнилась
     * @return {Promise<*>}
     */
    pushError: async data => {
        if (!channel) {
            throw new Error('Channel for connect ampq server not created');
        }
        return await channel
            .sendToQueue(errorChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
    },
    /**
     * Ф-я подписки на очередь транзакциий, которые необходимо выполнить
     * @param {String} event название события подписки
     * @param {Function} cb ф-я обратного вызова, вызывается по наступлению события event
     */
    listenPending: (event, cb) => {
        eventEmitter.on(event, cb);
    },
    /**
     * Ф-я подтверждения сообщения
     * @param {Object} msg сообщение от aqmp сервера, которое необходимо подтвердить
     * @return {*}
     */
    confirmPendingTransaction: msg => {
        return channel.ack(msg);
    }
};