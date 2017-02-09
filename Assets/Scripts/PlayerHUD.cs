using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

	public Transform cloneIcons;

	private PlayerController player;

	void Awake() {
		player = GetComponentInParent<PlayerController> ();
	}

	void Start() {
		cloneIcons = transform.Find ("CloneIcons");
	}

	// Update is called once per frame
	void Update () {
		if (player.clonesLeft < cloneIcons.childCount) {
			Destroy(cloneIcons.GetChild(cloneIcons.childCount-1).gameObject);
		}
	}
}
