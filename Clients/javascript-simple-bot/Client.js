var Client;

Client = function(name, socket) {

	var announcement;
	var announcer;

	var ranks = ["3,1", "3,2", "4,1", "4,2", "4,3", "5,1", "5,2", "5,3", "5,4", "6,1", "6,2", "6,3", "6,4", "6,5", "1,1", "2,2", "3,3", "4,4", "5,5", "6,6", "2,1"];

	var limit = 12;

	function register() {
		console.log("Registered");
		_sendMessage("REGISTER;"+name);
	}

	function roundStarting(token) {
		console.log("Joined game");
		_sendMessage("JOIN;" + token)
	}

	function yourTurn(token, stats) {
		console.log("My Turn");
		console.log("having announcement of " + announcement );
		console.log("Limit of: " + limit);

		var useLimit = limit;

		if(typeof stats[announcer] != "undefined") {
			if(typeof stats[announcer]["CAUGHT_BLUFFING"] != "undefined") {
				if(stats[announcer]["CAUGHT_BLUFFING"] / stats[announcer]["total"] * 100 > 10) {
					console.log("##############use different limit for bluffer " + announcer);
					useLimit  = limit - 2;
				}
			}
		}

		if(ranks.indexOf(announcement) < useLimit || announcement == null) {
			console.log("We roll");
			_sendMessage("ROLL;" + token);
		}else {
			console.log("We want to see");
			_sendMessage("SEE;" + token);
		}
	}

	function announce(token, dice) {
		console.log("announcing: " + dice);
		var announceDice = dice;
		if(ranks.indexOf(dice) <= ranks.indexOf(announcement)) {
			var announceDiceIndex = ranks.indexOf(announcement) + 2;
			announceDice = ranks[announceDiceIndex];
			console.log("We had " + dice + " but we now bluff " + announceDice)
		}
//		announcer =
		_sendMessage("ANNOUNCE;" + announceDice + ";" + token);
	}

	function setAnnounce(name, announce) {
		announcer = name;
		announcement = announce;
	}

	function resetAnnounce() {
		announcer = null;
		announcement = null;
	}

	function getAnnouncer() {
		return announcer;
	}

	function _sendMessage(message){
		var buffer = new Buffer(message);
		socket.send(buffer, 0, buffer.length, 9000, "192.168.0.45", function(err, bytes){});
	}

	return {
		register: register,
		roundStarting: roundStarting,
		yourTurn: yourTurn,
		announce: announce,
		setAnnounce: setAnnounce,
		resetAnnounce: resetAnnounce,
		getAnnouncer: getAnnouncer
	}
}

exports.Client = Client;