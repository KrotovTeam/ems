'use strict';

const boom = require('boom');
const Validator = require('jsonschema').Validator;
const validator = new Validator();
const stackTrace = require('stack-trace');


const internals = {
    validate: (data, scheme, logger = null) => {
        const funcCall = stackTrace.get()[1].getFunctionName();

        if (!data) {
            const error = boom.badData('Ошибка валидации: входные параметры пустые.');
            if (logger) {
                logger.push({
                    cmd: 'error',
                    message: `${logger.owner}: ошибка валидации в функции '${funcCall}': ${error.message}.`,
                    source: logger.owner,
                    destination: null,
                    pattern: null,
                    func: funcCall,
                    data: data,
                    err: error
                });
            }
            throw error;
        }

        const resultValidate = validator.validate(data, scheme);

        if (resultValidate.errors.length !== 0) {
            let errorArray = resultValidate.errors.map(e => {
                return e.stack.replace(`${resultValidate.propertyPath}.`, '');
            });
            const error = boom.badData('Ошибка валидации: ' + new Error(errorArray));
            if (logger) {
                logger.push({
                    cmd: 'error',
                    message: `${logger.owner}: ошибка валидации в функции '${funcCall}': ${error.message}.`,
                    source: logger.owner,
                    destination: null,
                    pattern: null,
                    func: funcCall,
                    data: data,
                    err: error
                });
            }

            throw error;
        }
    },
};

const errors = {
    notFound: {
        // abs
        USER_PRODUCTS_NOT_RECEIVED_PROFILE_NOT_FOUND: {
            code: 40400,
            message: 'Продукты пользователя по аккаунту не были получены: профиль не найден'
        },
        INFORMATION_CARD_NOT_FOUND: {
            code: 40401,
            message: 'Информация по карте не найдена'
        },
        CLIENT_ACCOUNT_INFORMATION_NOT_FOUND: {
            code: 40402,
            message: 'Информация о счете клиента не найдена'
        },
        OPERATION_FAILED_NOT_UPDATE_STATE_IN_ACCOUNT: {
            code: 40403,
            message: 'Ошибка операции: не возможно обновить state у счета'
        },
        PROFILE_NOT_FOUND: {
            code: 40404,
            message: 'Профиль не найден'
        },
        SUMMARY_REPORT_ON_BANK_FROM_ABS_ACCOUNT_CRS_NOT_FOUND: {
            code: 40405,
            message: 'Сводный отчет по банку из АБС (счет ЦРС) не найден'
        },
        SUMMARY_REPORT_ON_BANK_FROM_ABS_TRANSIT_ACCOUNT_NOT_FOUND: {
            code: 40406,
            message: 'Сводный отчет по банку из АБС (транзитный счет) не найден'
        },
        // account
        NOT_GET_INFORMATION_BY_WALLET: {
            code: 40407,
            message: 'Не удалось получить информацию по кошельку'
        },
        NOT_GET_LIST_IDENTIFIERS_BY_WALLET: {
            code: 40408,
            message: 'Не удалось получить список идентификаторов по кошельку'
        },
        NOT_GET_WALLET_BALANCE: {
            code: 40409,
            message: 'Не удалось получить информацию по балансу кошелька'
        },
        NOT_GET_LIMIT_OF_TRANSFERRED_TYPE: {
            code: 40410,
            message: 'Не удалось получить лимит переданного типа'
        },
        NOT_CHANGE_LIMIT_OF_TRANSFERRED_TYPE: {
            code: 40411,
            message: 'Не удалось изменить лимит переданного типа'
        },
        NOT_GET_FULL_INFORMATION_BY_WALLET: {
            code: 40412,
            message: 'Не удалось получить полную информацию по кошельку (вместе с балансом)'
        },
        NOT_ADD_NEW_ID_FOR_WALLET: {
            code: 40413,
            message: 'Не удалось добавить новый идентификатор для кошелька'
        },
        // amqp_sm_worker, currency, sm_consumer
        CURRENCY_NOT_FOUND: {
            code: 40414,
            message: 'Валюта не найдена.'
        },
        // auth
        USER_NOT_FOUND: {
            code: 40415,
            message: 'Пользователь не найден'
        },
        // bank
        BANK_NOT_FOUND_FAILED_GET_INFORMATION_ABOUT_BANK: {
            code: 40416,
            message: 'Не удалось получить информацию о банке по его адресу: банк не найден'
        },
        BANK_NOT_FOUND_FAILED_GET_AMOUNT_EMISSION: {
            code: 40417,
            message: 'Не удалось получить сумму эммисии по банку: банк не найден'
        },
        BANK_NOT_FOUND_FAILED_GET_BALANCE_OF_BANK_IN_CONTEXT_OF_OTHER_BANKS: {
            code: 40418,
            message: 'Не удалось получить баланс банка в разрезе других банков: банк не найден'
        },
        BANK_NOT_FOUND_NOT_GET_SUMMARY_REPORT_ON_BANK_FROM_ABS_ACCOUNT_CRS: {
            code: 40419,
            message: 'Не удалось получить сводный отчет по банку из АБС (счет ЦРС): банк не найден'
        },
        BANK_NOT_FOUND_NOT_GET_SUMMARY_REPORT_ON_BANK_FROM_ABS_TRANSIT_ACCOUNT: {
            code: 40420,
            message: 'Не удалось получить сводный отчет по банку из АБС (транзитный счет): банк не найден'
        },
        BANK_NOT_FOUND_NOT_GET_BANK_LIMIT_BY_REGULATOR: {
            code: 40421,
            message: 'Не удалось получить регулятором лимита банка: банк не найден'
        },
        BANK_NOT_FOUND_FAILED_CHANGE_CURRENT_LIMIT_BY_REGULATOR: {
            code: 40422,
            message: 'Не удалось изменить регулятором текущего лимита для банка: банк не найден'
        },
        // limit
        LIMIT_NOT_FOUND: {
            code: 40423,
            message: 'Лимит не найден'
        },
        // notification
        DEVICE_NOT_FOUND: {
            code: 40424,
            message: 'Устройство не найдено'
        },
        // profile
        NO_USER_CONNECTION_WITH_WALLET: {
            code: 40425,
            message: 'Не существует связи пользователя с кошельком'
        },
        // sm_consumer, sm_publisher
        ACCOUNT_WITH_SPECIFIED_IDENTIFIER_NOT_FOUND: {
            code: 40426,
            message: 'Аккаунт с указанным идентификатором не найден'
        },
        // transaction
        TRANSACTION_NOT_FOUND: {
            code: 40427,
            message: 'Транзакция не найдена.'
        },
        // invoice
        INVOICE_NOT_FOUND: {
            code: 40428,
            message: 'Счет не найден.'
        },
        INVOICE_TRANSACTION_NOT_FOUND: {
            code: 40429,
            message: 'Счет не найден по идентификатору транзакции'
        }
    },
    conflict: {
        // account,  transaction
        SIGNATURE_INVALID: {
            code: 40900,
            message: 'Подпись недействительна.'
        },
        OPERATION_LIMIT_EXCEEDED_EXCEPTION: {
            code: 40901,
            message: 'Превышен лимит проведения разовой операции.'
        },
        DAILY_LIMIT_EXCEEDED_EXCEPTION: {
            code: 40902,
            message: 'Превышен суточный лимит.'
        },
        MONTHLY_LIMIT_EXCEEDED_EXCEPTION: {
            code: 40903,
            message: 'Превышен месячный лимит.'
        }
    },
    unauthorized: {
        // gateway
        AUTHORIZATION_HEADER_NOT_PASSED: {
            code: 40100,
            message: 'Для доступа к запрашиваемому ресурсу требуется аутентификация. В заголовок необходимо включить поле authorization с требуемыми для аутентификации данными.'
        }
    },
    internal: {
        // sm_consumer
        FAILED_CREATE_ACCOUNT: {
            code: 50000,
            message: 'Не удалось создать счет'
        },
        FAILED_CREATE_BANK: {
            code: 50001,
            message: 'Не удалось создать банк'
        },
        FAILED_CREATE_CURRENCY: {
            code: 50002,
            message: 'Не удалось создать валюту'
        },
        INVALID_JSON_RPC: {
            code: 50003,
            message: 'Ошибка: нет подключения к masterChain'
        }
    },
    badRequest: {
        // transaction
        FAILED_CREATE_TRANSACTIONS_TO_TRANSFER_FUNDS: {
            code: 40000,
            message: 'Ошибка создания транзакции на перевод средств.'
        },
        BANK_OPERATION_NOT_AVAILABLE: {
            code: 40001,
            message: 'Банк: операция недоступна'
        },
        FAILED_CREATE_TRANSACTIONS_TO_ENDOW_FUNDS: {
            code: 40002,
            message: 'Ошибка создания транзакции на зачисление средств'
        },
        FAILED_CREATE_TRANSACTIONS_TO_WITHDRAW_FUNDS: {
            code: 40003,
            message: 'Ошибка создания транзакции на списание средств'
        },
    },
    badData: {
        VALIDATION_ERROR: {
            code: 42200,
            message: 'Неверный формат входных данных'
        }
    }
};

module.exports = {
    validate: internals.validate,
    errors
};
