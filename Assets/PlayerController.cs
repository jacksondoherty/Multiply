using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class PlayerController : NetworkBehaviour {

	public float movementSpeed;
	public float turnSpeed;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public GameObject clonePrefab;


	private Vector3 movement;
	private float turn;
	bool disableTurn = false;

	public override void OnStartLocalPlayer() {
		// change character color
		GetComponent<MeshRenderer>().material.color = Color.blue;

		// setup camera
		Camera.main.transform.position = transform.position
			- transform.forward * 10
			+ transform.up * 3;
		Camera.main.transform.rotation = transform.rotation;
		Camera.main.transform.parent = transform;

		Cursor.visible = false;
	}

	void Update () {
		if (isLocalPlayer) {
			GunControl ();
			MovementControl ();
			if (!disableTurn) {
				TurnControl ();
			}
			SpawnControl ();

			if (Input.GetKeyDown (KeyCode.C)) {
				Cursor.visible = !Cursor.visible;
			}
			if (Input.GetKeyDown (KeyCode.Y)) {
				disableTurn = !disableTurn;
			}
		}
	}
		
	void GunControl() {
		if (Input.GetMouseButtonDown(0)) {
			CmdFire ();
		}
	}

	void MovementControl() {
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		movement.Set(horizontal, 0, vertical);
		movement = movement.normalized * Time.deltaTime * movementSpeed;
		transform.Translate(movement);
	}

	void TurnControl() {
		if (Input.mousePosition.x < Screen.width * .01) {
			turn = -turnSpeed;
		} else if (Input.mousePosition.x > Screen.width * .99) {
			turn = turnSpeed;
		} else {
			turn = Input.GetAxis ("Mouse X") * turnSpeed;
		}
		transform.Rotate (0, turn, 0);
	}

	void SpawnControl() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdSpawnClone ();		
		}
	}

	[Command]
	void CmdSpawnClone() {
		GameObject clone = (GameObject)Instantiate (clonePrefab, 
			transform.position, 
			transform.rotation);
		CloneController cloneScript = clone.GetComponent<CloneController> ();
		cloneScript.creator = gameObject;
		NetworkServer.Spawn (clone);
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
