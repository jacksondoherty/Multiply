using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class CloneController : NetworkBehaviour {

	[HideInInspector]
	public GameObject creator;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	private Transform target;
	private NavMeshAgent nav;

	void Start() {
		nav = GetComponent<NavMeshAgent> ();
		InvokeRepeating ("CmdFire", 5f, 5f); 
	}
	
	void Update () {
		if (target == null) {
			FindTarget ();
		} else {
			nav.SetDestination (target.position);
		}
	}

	// @todo: make target random, either clone or player
	// check if clone's creator is same or if is creator
	void FindTarget() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		bool targetFound = false;
		for (int i = 0; i < players.Length && !targetFound; i++) {
			if (!players [i].Equals (creator)) {
				targetFound = true;
				target = players[i].GetComponent<Transform>();
			}
		}
	}

	[Command]
	void CmdFire() {
		var bullet = (GameObject)Instantiate (bulletPrefab,
												bulletSpawn.position,
												bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
		NetworkServer.Spawn (bullet);

		// destroy bullet after 10 seconds
		Destroy(bullet, 10.0f);
	}
}
