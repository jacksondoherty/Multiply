using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class CloneController : NetworkBehaviour {

	[HideInInspector]
	[SyncVar]
	public GameObject creator;

	public float bulletSpeed;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	private Transform target;
	private NavMeshAgent nav;
	private Renderer rend;

	void Awake() {
		nav = GetComponent<NavMeshAgent> ();
		nav.stoppingDistance = 8;
		rend = GetComponent<Renderer> ();
	}

	void Start() {
		rend.material = creator.GetComponent<Renderer> ().material;
		InvokeRepeating ("Fire", 5f, 5f);
	}
	
	void Update () {
		if (target == null) {
			FindTarget ();
		} else {
			nav.SetDestination (target.position);
		}
	}

	void FindTarget() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		bool targetFound = false;
		for (int i = 0; i < players.Length && !targetFound; i++) {
			// is .Equals the best way to do this?
			if (!players [i].Equals (creator)) {
				targetFound = true;
				target = players[i].GetComponent<Transform>();
			}
		}
	}

	void Fire() {
		var bullet = (GameObject)Instantiate (bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
	}

	/*
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
	*/
}
