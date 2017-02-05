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
	public Material localPlayerColor;
	public Material enemyPlayerColor;

	private Vector3 movement;
	private float turn;
	private Game gameScript;
	private Renderer rend;
	private MyHud myHud;

	void Awake() {
		gameScript = GameObject.Find("Game").GetComponent<Game>();
		rend = GetComponent<MeshRenderer> ();
		GameObject managerObj = GameObject.Find ("NetworkManager");
		myHud = managerObj.GetComponent<MyHud> ();
	}

	void Start() {
		GetComponent<Rigidbody> ().freezeRotation = true;
		if (isLocalPlayer) {
			rend.material = localPlayerColor;
		} else {
			rend.material = enemyPlayerColor;
		}
	}

	public override void OnStartLocalPlayer() {
		ShowPlayerIndicator();
		SetupCamera ();
	}

	void Update () {
		if (isLocalPlayer) {
			CursorVisibility ();
			if (!myHud.gamePaused) {
				GunControl ();
				MovementControl ();
				TurnControl ();
				SpawnControl ();
			}
		}
	}

	void ShowPlayerIndicator() {
		transform.GetChild(0).gameObject.SetActive(true);
	}

	void SetupCamera() {
		Camera.main.transform.position = transform.position
			- transform.forward * 10
			+ transform.up * 3;
		Camera.main.transform.rotation = transform.rotation;
		Camera.main.transform.parent = transform;
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
		if (Input.mousePosition.x < 0 
			|| Input.mousePosition.x > Screen.width
			|| Input.mousePosition.y > Screen.height
			|| Input.mousePosition.y < 0) {
			turn = 0;
		} else {
			turn = Input.GetAxis ("Mouse X") * turnSpeed;
			Cursor.visible = false;
		}
		transform.Rotate (0, turn, 0);
	}

	void SpawnControl() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdSpawnClone ();		
		}
	}

	void CursorVisibility() {
		if (myHud.gamePaused
			|| Input.mousePosition.x < Screen.width * .05
			|| Input.mousePosition.x > Screen.width * .95) {
			Cursor.visible = true;
		} else {
			Cursor.visible = false;
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

	[Command]
	public void CmdEndGame() {
		gameScript.EndGame ();
	}
}
