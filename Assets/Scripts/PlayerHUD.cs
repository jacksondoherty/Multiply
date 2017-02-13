using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

	public Transform cloneIcons;

	private PlayerController player;
	private int iconCount;

	void Awake() {
		player = GetComponentInParent<PlayerController> ();
	}

	void Start() {
		cloneIcons = transform.Find ("CloneIcons");
		// perhaps instantiate icons in script 
		iconCount = cloneIcons.childCount;
	}

	// Update is called once per frame
	void Update () {
		if (player.clonesLeft < iconCount) {
			cloneIcons.GetChild (iconCount - 1).gameObject.SetActive (false);
			iconCount--;
		}
	}
}
