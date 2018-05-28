const express = require('express');
const res = express.response;
res.message = function (msg, type){
    type = type || 'info';
    const sess = this.req.session;
    sess.messages = sess.messages || [];
    sess.messages.push({type: type, text: msg});
};
res.error = function(msg){
    return this.message(msg, 'alert alert-danger')
};

res.info = function(msg){
    return this.message(msg, 'alert alert-success')
};

module.exports = function(req, res, next){
    res.locals.messages = req.session.messages || [];
    res.locals.removeMessages = function(){
        req.session.messages = [];
    };
    next();
};