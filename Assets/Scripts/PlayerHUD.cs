using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

	private Transform cloneIcons;
	private Transform donutIcons;
	private PlayerController player;
	private int cloneCount;
	private int donutCount;

	void Awake() {
		player = GetComponentInParent<PlayerController> ();
	}

	void Start() {
		cloneIcons = transform.Find ("CloneIcons");
		donutIcons = transform.Find ("DonutIcons");
		// perhaps instantiate icons in script 
		cloneCount = cloneIcons.childCount;
		donutCount = donutIcons.childCount;
	}

	// Update is called once per frame
	void Update () {
		if (player.clonesLeft < cloneCount) {
			cloneIcons.GetChild (cloneCount - 1).gameObject.SetActive (false);
			cloneCount--;
		}
		if (player.donutsLeft < donutCount) {
			donutIcons.GetChild (donutCount - 1).gameObject.SetActive (false);
			donutCount--;
		}
	}
}
