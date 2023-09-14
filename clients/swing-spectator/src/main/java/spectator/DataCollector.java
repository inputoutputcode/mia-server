package spectator;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Map;

import udphelper.MessageListener;

public class DataCollector implements MessageListener {

	private static final Map<String,String> REASON_TEXTS = new HashMap<String, String>();
	
	static {
		REASON_TEXTS.put("SEE_BEFORE_FIRST_ROLL", "");
		REASON_TEXTS.put("LIED_ABOUT_MIA", "");
		REASON_TEXTS.put("ANNOUNCED_LOSING_DICE", "");
		REASON_TEXTS.put("DID_NOT_ANNOUNCE", "");
		REASON_TEXTS.put("DID_NOT_TAKE_TURN", "");
		REASON_TEXTS.put("INVALID_TURN", "");
		REASON_TEXTS.put("SEE_FAILED", "");
		REASON_TEXTS.put("CAUGHT_BLUFFING", "");
		REASON_TEXTS.put("MIA", "");
		REASON_TEXTS.put("NO_PLAYERS", "");
		REASON_TEXTS.put("ONLY_ONE_PLAYER", "");
	}
	
	private final RoundListener roundListener;
	private final Collection<ScoreListener> scoreListeners = new ArrayList<ScoreListener>();

	private StringBuilder currentRound = new StringBuilder();
	private int currentRoundNumber;
	
	public DataCollector(RoundListener roundListener) {
		this.roundListener = roundListener;
	}

	public void onMessage(String message) {
		String[] parts = message.split(";");
		if (parts[0].equals("ROUND STARTED")) {
			currentRoundNumber = Integer.parseInt(parts[1]);
			currentRound.setLength(0);
		} else if (parts[0].equals("PLAYER LOST") || parts[0].equals("ROUND CANCELED")) {
			appendFormattedMessage(parts);
			if (roundIsIncomplete()) return;
			roundListener.roundCompleted(currentRoundNumber, currentRound.toString());
		} else if (parts[0].equals("SCORE")) {
			handleScores(message);
		} else {
			appendFormattedMessage(parts);
		}
	}

	private void appendFormattedMessage(String[] messageParts) {
		String formatted = null;
		if (messageParts[0].equals("ROUND STARTED")) {
			String[] players = messageParts[1].split(",");
			formatted = "Player: " + germanJoin(players);
		} else if (messageParts[0].equals("ANNOUNCED")) {
			String player = messageParts[1];
			String dice = formatDice(messageParts[2]);
			formatted = player + ": " + dice;
		} else if (messageParts[0].equals("PLAYER LOST")) {
			String[] players = messageParts[1].split(",");
			String reason = formatReason(messageParts[2]);
			formatted = formatPlayerLost(players, reason);
		} else if (messageParts[0].equals("ACTUAL DICE")) {
			String dice = formatDice(messageParts[1]);
			formatted = "Dice: " + dice;
		} else if (messageParts[0].equals("PLAYER ROLLS")) {
			String player = messageParts[1];
			formatted = player + " rolls...";
		} else if (messageParts[0].equals("PLAYER WANTS TO SEE")) {
			String player = messageParts[1];
			formatted = player + " will see!";
		} else if (messageParts[0].equals("ROUND CANCELED")) {
			String reason = formatReason(messageParts[1]);
			formatted = "Round cancelled: " + reason;
		}
		if (formatted != null) {
			currentRound.append(formatted);
		} else {
			currentRound.append(messageParts[0]);
		}
		currentRound.append("\n");
	}

	private String formatReason(String reasonCode) {
		String result = REASON_TEXTS.get(reasonCode);
		if (result == null) result = reasonCode;
		return result;
	}

	private String formatDice(String diceString) {
		String[] dieStrings = diceString.split(",");
		int die1 = Integer.parseInt(dieStrings[0]);
		int die2 = Integer.parseInt(dieStrings[1]);
		if (die1 == 2 && die2 == 1) {
			return "Mia!";
		}
		if (die1 == die2) {
			switch (die1) {
				case 1: return "One double";
				case 2: return "Two double";
				case 3: return "Three double";
				case 4: return "Four double";
				case 5: return "Five double";
				case 6: return "Six double";
			}
		}
		return "" + die1 + die2;
	}

	private String formatPlayerLost(String[] players, String reason) {
		String result;
		if (players.length == 1) {
			result = players[0] + " lose";
		} else {
			result = textJoin(players) + " lose";
		}
		return result + ": " + reason;
	}

	private String textJoin(String[] parts) {
		if (parts.length == 0) {
			return "";
		}
		String result = parts[0];
		if (parts.length == 1) {
			return result;
		}
		for (int i = 1; i < parts.length - 1; i++) {
			result += ", " + parts[i];
		}
		return result + " and " + parts[parts.length - 1];
	}

	private void handleScores(String message) {
		if (roundIsIncomplete()) return;
		Scores scores = Scores.parse(message);
		for (ScoreListener listener : scoreListeners) {
			listener.scoresAfterRound(scores, currentRoundNumber);
		}
	}

	private boolean roundIsIncomplete() {
		return currentRoundNumber == 0;
	}

	public void addScoreListener(ScoreListener listener) {
		scoreListeners.add(listener);
	}

}
