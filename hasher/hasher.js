'use strict';

const Hapi = require('hapi');
const crypto = require('crypto');

const server = new Hapi.Server();

server.connection({ port: 8001 });

server.route({
    method: 'POST',
    path: '/',
    handler: function (request, reply) {
        var body = request.payload;
        var hash = crypto.createHash('sha256').update(body).digest("hex");
        reply(hash);
    }
});

server.route({
    method: 'GET',
    path: '/',
    handler: function (request, reply) {
        reply('Hasher running on ' + server.info.uri);
    }
});

server.start((err) => {
    if (err) {
        throw err;
    }

    console.log('Server running at:' + server.info.uri);
});