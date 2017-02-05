using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Game : NetworkBehaviour {

	[SyncVar]
	public bool gameEnded = false;

	public void EndGame() {
		if (!isServer) {
			return;
		}
		gameEnded = true;
	}
}
