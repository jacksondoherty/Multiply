using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Game : NetworkBehaviour {

	[SyncVar]
	public bool gameEnded = false;
	public Material playerOneColor;
	public Material playerTwoColor;

	[SyncVar]
	private GameObject playerOne;
	[SyncVar]
	private GameObject playerTwo;

	public void EnterGame(GameObject player) {
		if (!isServer) {
			return;
		}

		if (playerOne == null) {
			playerOne = player;
		} else if (playerTwo == null) {
			playerTwo = player;
		} else {
			//@todo: handle this when lobby is created
			print ("too many players in game");
		}
	}

	public Material GetColor(GameObject player) {
		// is .Equals the best way to do this?
		if (player.Equals(playerOne)) {
			return playerOneColor;
		// is .Equals the best way to do this?
		} else if (player.Equals(playerTwo)) {
			return playerTwoColor;
		} else {
			// should not happen with player limit (above)
			print ("cannot identify player");
			return null;
		}
	}

	public void EndGame() {
		if (!isServer) {
			return;
		}

		gameEnded = true;
	}
}
