var dgram = require('dgram');
var Client = require('./Client').Client;
var Meier = require('./Meier').Meier;

console.log("press Ctrl-C to stop");

var socket = dgram.createSocket("udp4");

socket.on("listening", function () {
	var address = socket.address();
	console.log("server listening " + address.address + ":" + address.port);

	var client = new Client("JavaScript Starter Bot", socket);

	var meier = new Meier(client);
	socket.on("message", function (msg, rinfo) {

		meier.runMessage(msg);
	});

	client.register();

});

socket.bind(9999);