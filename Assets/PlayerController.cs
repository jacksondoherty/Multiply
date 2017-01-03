using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class PlayerController : NetworkBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	public override void OnStartLocalPlayer() {
		GetComponent<MeshRenderer>().material.color = Color.blue;

		if (isLocalPlayer) {
			// set position of camera and set as child of this
			Camera.main.transform.position = transform.position
											- transform.forward * 10
											+ transform.up * 3;
			Camera.main.transform.parent = transform;
		}

	}

	void Update () {
		// keep first -> so only local player executes code below
		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdFire ();
		}

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
	}

	[Command]
	void CmdFire() {
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate (
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

		NetworkServer.Spawn (bullet);

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}
}
