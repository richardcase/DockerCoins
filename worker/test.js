var redis = require("redis");
var client = redis.createClient(6379, 'localhost');

//client.set('hashes', 0);
//client.set('hashes', 10);
//client.set('hashes', 11);
//client.set('hashes', 12);
//client.set('hashes', 13);
//client.set('hashes', 14);

client.get('hashes', function(err, reply) {
    console.log('Num hashes in redis: %s',reply);
});



