var moment = require('moment');
var redis = require("redis");
var request = require('sync-request');


var client = redis.createClient(6379, 'redis');

client.on("error", function (err) {
    console.log("Error " + err);
});

function getRandomBytes() {
    var res = request('GET','http://rng:8002/32', {
        'headers' : {
            'content-type' : 'text/plain'
        }
    });
    return res.getBody().toString('utf-8');     
}

function hashBytes(data) {
    var res = request('POST', 'http://hasher:8001', {
        body: data,
        'headers' : {
            'content-type' : 'text/plain'
        }
    });
    return res.getBody().toString('utf-8');
}

function work_loop() {
    console.log("***Entering work loop");
    var interval = 1;
    var deadline = 0;
    var loops_done = 0
    while (true) {
        var current = moment().unix();
        console.log(deadline);
        if (current > deadline) {
            console.log("%s units of work done, updating hash counter", loops_done);
            client.set('hashes', loops_done, redis.print);
            loops_done = 0;
            deadline = moment().add(1, 'seconds').unix();
        }
        work_once();
        loops_done += 1;
    }
}

function work_once() {
    console.log("Doing one unit of work");
    //TODO: sleep potentially
    // time.sleep(0.1)
    var random_bytes = getRandomBytes();
    var hex_hash = hashBytes(random_bytes);
    if (hex_hash.startsWith('0')) {
        console.log("No coin found");
        return;
    }
    console.log("Coin found %s", hex_hash);
    //created = client.hmset('wallet', hex_hash, random_bytes);
    // TODO: missed bit here
}

client.on('connect', function() {
   console.log('******connected********');
   client.set('hashes', 0, redis.print);
   while (true) {
        work_loop();
  }
});