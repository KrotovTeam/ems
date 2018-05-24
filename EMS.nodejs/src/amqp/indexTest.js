const events = require('events');
const eventEmitter = new events.EventEmitter();
const config = require('config');
const amqp = require('amqplib/callback_api');
const emsDataNormalizationServiceRequestsChannel = config.amqp.channels.EMS_DATA_NORMALIZATION_SERVICE_REQUESTS;
const emsDataNormalizationServiceResponsesChannel = config.amqp.channels.EMS_DATA_NORMALIZATION_SERVICE_RESPONSES;
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
        ch.assertQueue(emsDataNormalizationServiceResponsesChannel, {durable: true, noAck: true});
        ch.assertQueue(errorChannel, {durable: true, noAck: true});
        ch.assertQueue(emsDataNormalizationServiceRequestsChannel, {durable: true, noAck: true});
        // ch.prefetch(1);
        // подписываемся на получения транзакциий, которые нужно выполнить
        ch.consume(emsDataNormalizationServiceResponsesChannel, msg => {
            const message = JSON.parse(msg.content);
            console.log('Получил: ', message);
            // if (message.operation) {
            //     eventEmitter.emit(message.operation, message, msg);
            // }
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


async function pushToEmsDataNormalizationServiceChannel(data){
    if (!channel) {
        console.error(`Channel for connect ampq server not created`);
        return false;
    }
    try {
        console.log('Отправляем результат транзакции', data);
        return await channel
            .sendToQueue(emsDataNormalizationServiceRequestsChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
    } catch (err) {
        console.error(`Не удалось записать ${JSON.stringify(data)} в очередь ${emsDataNormalizationServiceRequestsChannel} ${err}`);
    }
    return false;
}



const PATH = 'F:\\TEST\\';

setTimeout(async ()=>{
    await pushToEmsDataNormalizationServiceChannel({
        messageType: ['urn:message:BusContracts:IDataNormalizationRequest'],
        Folder: PATH,
        SatelliteType: 1
    });
}, 1000);

module.exports = {
    pushToEmsDataNormalizationServiceChannel:pushToEmsDataNormalizationServiceChannel,
    listenEmsDataNormalizationServiceResponsesChannel: (event, cb) => {
        eventEmitter.on(event, cb);
    },
    confirm: msg => {
        return channel.ack(msg);
    }
};