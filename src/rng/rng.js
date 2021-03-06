'use strict';

const Hapi = require('hapi');
const crypto = require('crypto');

const server = new Hapi.Server();

server.connection({ port: 8002 });

server.route({
    method: 'GET',
    path: '/{numbytes}',
    handler: function (request, reply) {
        setTimeout(function() {
            // time.sleep(0.1)
            var num = parseInt(request.params.numbytes);
        
            crypto.randomBytes(num, function(err, buffer) {
                var token = buffer.toString('hex');
                reply(token);
            });
        }, 100);

        
    }
});

server.route({
    method: 'GET',
    path: '/',
    handler: function (request, reply) {
        reply('RNG service running on ' + server.info.uri);
    }
});

server.start((err) => {
    if (err) {
        throw err;
    }

    console.log('Server running at:' + server.info.uri);
});