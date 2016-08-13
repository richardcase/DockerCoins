var moment = require('moment');
var redis = require("redis");
var request = require('request-promise');

var client = redis.createClient(6379, 'redis');

client.on("error", function (err) {
    console.log("Error " + err);
});

function getRandomBytes() {
    const options = {
        method: 'GET',
        uri: 'http://rng/32'
    }

    request(options)
        .then(function(response) {
           return response; 
        })
        .catch(function(err) {
            console.error(err);
        });
    
}

function hashBytes(data) {
    const options = {
        method: 'POST',
        uri: 'http://hasher/',
        body: data
    };

    request(options)
        .then(function(response) {
            return response;
        })
        .catch(function(err) {
            console.error(err);
        })
}

function work_loop() {
    var interval = 1;
    var deadline = 0;
    var loops_done = 0
    client.set('hashes', 0);
    while (true) {
        var current = moment().unix();
        if (current > deadline) {
            console.log("%s units of work done, updating hash counter", loops_done);
            client.incr('hashes', loops_done);
            loops_done = 0;
            deadline = moment().add(1, 'seconds').unix();
        }
        work_once();
        loops_done += 1;
    }
}

function work_once() {
    console.debug("Doing one unit of work");
    //TODO: sleep potentially
    var random_bytes = getRandomBytes();
    var hex_hash = hashBytes(random_bytes);
    if (hex_hash.startsWith('0')) {
        console.log("No coin found");
        return;
    }
    console.log("Coin found {0}", hex_hash);
    created = client.hmset('wallet', hex_hash, random_bytes);
    // TODO: missed bit here
}

client.on('connect', function() {
    console.log('connected');
    while (true) {
        work_loop();
    }
});



client.set("string key", "string val", redis.print);
client.get("missingkey", function(err, reply) {
    // reply is null when the key is missing
    console.log(reply);
})

client.set('key1', 10, function() {
    client.incr('key1', function(err, reply) {
        console.log(reply); // 11
    });
});