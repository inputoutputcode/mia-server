var Meier;

Meier = function(client) {

	var token;
	var command;
	var splitted;

	var stats = {"games": 0};

	function runMessage(msg){
		_splitMessage(msg.toString());

		switch(command) {
			case "ROUND STARTING":
				client.roundStarting(token);
				break;
			case "YOUR TURN":
				client.yourTurn(token, stats);
				break;
			case "PLAYER LOST":
				stats.games++;
				if(token.indexOf(",") < 0) {
					if(typeof stats[token] == "undefined")
						stats[token] = {"total":0};

					if(typeof stats[token][splitted[2]] == "undefined")
						stats[token][splitted[2]] = 0;

					stats[token][splitted[2]]++;
					stats[token]["total"]++;
				}
				console.log(stats);
				if(token == "JavaScript Starter Bot") {
					console.log(splitted[2]);
				}
				client.resetAnnounce();
				break;
			case "ROLLED":
				console.log("Having: " + splitted[1]);
				client.announce(splitted[2],splitted[1]);
				break;
			case "ANNOUNCED":
				console.log("Setting announcement to: " + splitted[2]);
				client.setAnnounce(splitted[1],splitted[2]);
				break;
		}
	}

	function _splitMessage(msg){
		splitted = msg.split(";");
		command = splitted[0];
		token = splitted[1];
	}

	return {
		runMessage: runMessage
	};
}

exports.Meier = Meier;