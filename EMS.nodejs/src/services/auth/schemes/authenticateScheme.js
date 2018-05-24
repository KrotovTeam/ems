module.exports = {
    type: 'object',
    properties: {
        username: {
            type: 'string',
            required: true,
            minimum: 1
        },
        password: {
            type: 'string',
            required: true,
            minimum: 1
        }
    }
};
