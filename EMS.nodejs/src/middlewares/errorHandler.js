'use strict';
const {errors} = require('./../../utils');
module.exports = (err, req, res, next) => {
    // если у ошибки есть признак isBoom,
    // значит у нас есть информация об ошибке
    // и мы можем послать клиенту сообщение об ошибке
    if (err.isBoom) {
        // если код ошибки 422 - валидационная ошибка
        if (err.output && err.output.payload && err.output.payload.statusCode === 422) {
            return res.status(400).send({
                code: errors.badData.VALIDATION_ERROR.code,
                message: '',
                data: {
                    error: err.message,
                    call: err.callpoint,
                    stack: err.stack
                }
            });
        }
        // если код ошибки 555 - значит это внутренняя ошибка микросервисов
        if (err.output && err.output.payload && err.output.payload.statusCode === 555) {

            if (err.output.payload.message === 'Invalid JSON RPC response: undefined') {
                return res.status(500).send({
                    code: errors.internal.INVALID_JSON_RPC.code,
                    message: '',
                    data: {
                        error: err.message,
                        call: err.callpoint,
                        stack: err.stack
                    }
                });
            }
            return res.status(500).send({
                code: 500,
                message: '',
                data: {
                    error: err.message,
                    call: err.callpoint,
                    stack: err.stack
                }
            });
        }
        // вернем стандартный ответ с информацией об ошибке
        // Если message код ошибки
        const codeMessage = parseInt(err.output.payload.message);

        if (isNaN(codeMessage)) {
            return res.status(err.output.statusCode).send({
                code: err.output.statusCode,
                message: '',
                data: {
                    error: err.message,
                    call: err.callpoint,
                    stack: err.stack
                }
            });
        }

        return res.status(err.output.statusCode).send({
            code: codeMessage,
            message: '',
            data: {
                error: err.message,
                call: err.callpoint,
                stack: err.stack
            }
        });
    }
    // в противном случае - внутренняя ошибка сервера и код 500
    if (err instanceof Error) {
        return res.status(500).send({
            code: 500,
            message: '',
            data: {
                error: err.message,
                call: err.callpoint,
                stack: err.stack
            }
        });
    }

    return res.status(500).send({
        code: 500,
        message: '',
        data: {}
    });
};
