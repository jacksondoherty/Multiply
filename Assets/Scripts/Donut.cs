using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Donut : NetworkBehaviour {
	[HideInInspector]
	[SyncVar]
	public GameObject creator;
}
