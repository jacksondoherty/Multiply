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
		nav.updateRotation = true;
		rend = GetComponent<Renderer> ();
	}

	void Start() {
		rend.material = creator.GetComponent<Renderer> ().material;
		InvokeRepeating ("FindTarget", 3f, 3f);
		InvokeRepeating ("Fire", 3f, 3f);
	}
	
	void Update () {
		if (target == null) {
			FindTarget ();
		} else {
			nav.SetDestination (target.position);
		}
	}

	void FindTarget() {
		// donut precedent
		GameObject[] donuts = GameObject.FindGameObjectsWithTag ("Donut");
		for (int i = 0; i < donuts.Length; i++) {
			if (!donuts [i].GetComponent<Donut>().creator.Equals (creator)) {
				target = donuts [i].GetComponent<Transform> ();
				return;
			}
		}

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; i++) {
			// is .Equals the best way to do this?
			if (!players [i].Equals (creator)) {
				target = players[i].GetComponent<Transform>();
				return;
			}
		}
	}

	void Fire() {
		if (!isServer) return;
		if (target.gameObject.name == "Donut") return;

		var bullet = (GameObject)Instantiate (bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
		NetworkServer.Spawn (bullet);
		Destroy(bullet, 10.0f);
	}
}
