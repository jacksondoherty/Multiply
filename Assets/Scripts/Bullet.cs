using UnityEngine;
using System.Collections;


public class Bullet : MonoBehaviour {
	void OnTriggerEnter(Collider other)
	{
		var hit = other.gameObject;
		var health = hit.GetComponent<Health>();
		if (health != null)
		{
			health.TakeDamage(10);
		}

		Destroy(gameObject);
	}
}
