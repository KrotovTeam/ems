const events = require('events');
const eventEmitter = new events.EventEmitter();
const config = require('config');
const amqp = require('amqplib/callback_api');
const uuid = require('uuid/v4');
const phenomenonRequestChannel = config.amqp.channels.PHENOMENON_REQUEST;
const phenomenonResultChannel = config.amqp.channels.PHENOMENON_RESULT;
const calibrateRequestChannel = config.amqp.channels.CALIBRATE_REQUEST;
const calibrateResultChannel = config.amqp.channels.CALIBRATE_RESULT;
const characteristicsRequestChannel = config.amqp.channels.CHARACTERISTICS_REQUEST;
const characteristicsResultChannel = config.amqp.channels.CHARACTERISTICS_RESULT;

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
        ch.assertQueue(phenomenonRequestChannel, {durable: true, noAck: true});
        ch.assertQueue(phenomenonResultChannel, {durable: true, noAck: true});
        ch.assertQueue(calibrateRequestChannel, {durable: true, noAck: true});
        ch.assertQueue(calibrateResultChannel, {durable: true, noAck: true});
        ch.assertQueue(characteristicsRequestChannel, {durable: true, noAck: true});
        ch.assertQueue(characteristicsResultChannel, {durable: true, noAck: true});
        ch.assertQueue(errorChannel, {durable: true, noAck: true});

        // ch.prefetch(1);
        // подписываемся на получения транзакциий, которые нужно выполнить


        function listenResult(msg){
            const message = JSON.parse(msg.content);
            console.log(message);
            if (message.RequestId) {
                eventEmitter.emit(message.RequestId, message, msg);
            }
        }

        ch.consume(phenomenonResultChannel, listenResult);
        ch.consume(calibrateResultChannel, listenResult);
        ch.consume(characteristicsResultChannel, listenResult);
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



async function getPhenomenon(message){
    const requestId = uuid();
    const data = {
        messageType: ['urn:message:BusContracts:IDeterminingPhenomenonRequest'],
        message
    };
    message.RequestId = requestId;

    return new Promise(async (resolve, reject)=>{
        eventEmitter.on(requestId, (result, origMsg)=>{
            // confirm(origMsg);
            console.log(`получаем ответ на запрос ${requestId}: `, result);
            resolve(result);
        });
        console.log(`Отправляем запрос ${requestId}: `,data);
        await channel.sendToQueue(phenomenonRequestChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
    });
}

async function calibration(message){
    const requestId = uuid();
    const data = {
        messageType: ['urn:message:BusContracts:IDataNormalizationRequest'],
        message
    };
    message.RequestId = requestId;

    return new Promise(async (resolve, reject)=>{
        eventEmitter.on(requestId, (result, origMsg)=>{
            // confirm(origMsg);
            console.log(`получаем ответ на запрос ${requestId}: `, result);
            resolve(result);
        });
        console.log(`Отправляем запрос ${requestId}: `,data);
        await channel.sendToQueue(calibrateRequestChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
    });
}
async function getCharacteristics(message){
    const requestId = uuid();
    const data = {
        messageType: ['urn:message:BusContracts:IDataNormalizationRequest'],
        message
    };
    message.RequestId = requestId;

    return new Promise(async (resolve, reject)=>{
        eventEmitter.on(requestId, (result, origMsg)=>{
            // confirm(origMsg);
            console.log(`получаем ответ на запрос ${requestId}: `, result);
            resolve(result);
        });
        console.log(`Отправляем запрос ${requestId}: `,data);
        await channel.sendToQueue(characteristicsResultChannel, Buffer.from(JSON.stringify(data)), {persistent: true});
    });
}







async function test(){
    const resultPhenomen = await getPhenomenon({
        ResultFolder: '',
        LeftUpper: {
            Latitude: 1.23,
            Longitude:1.23
        },
        RightLower:{
            Latitude: 1.23,
            Longitude:1.23
        },
        DataFolders:[
            '',
            ''
        ]
    });
    console.log('Результат ---------------', JSON.stringify(resultPhenomen));

}

setTimeout(test, 2000);



module.exports = {
    getPhenomenon: getPhenomenon,
    calibration: calibration,
    getCharacteristics: getCharacteristics,
    pushToEmsDataNormalizationServiceChannel: async data=>{
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
    },
    listenEmsDataNormalizationServiceResponsesChannel: (event, cb) => {
        eventEmitter.on(event, cb);
    },
    confirm: msg => {
        return channel.ack(msg);
    }
};