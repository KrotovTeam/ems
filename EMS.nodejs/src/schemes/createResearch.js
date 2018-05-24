module.exports = {
    type: 'object',
    properties: {
        phenomenonName: {
            type: 'string',
            required: true,
            minimum: 1
        },
        status: {
            type: 'integer',
            required: false,
            minimum: 0
        }
    }
};
