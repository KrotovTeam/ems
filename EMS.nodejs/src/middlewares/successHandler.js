'use strict';


module.exports = (req, res, next) => {
    // если поле result не заполнено вернем NotFound - 404
    if (!req.result) {
        return res.status(404).send();
    }

    // вернем стандартный ответ с результатом выполнения операции
    res.send({
        code: 200,
        message: '',
        data: req.result
    });

    next();
};
