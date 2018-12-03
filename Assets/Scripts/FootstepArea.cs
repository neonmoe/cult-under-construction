using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundType))]
public class FootstepArea : MonoBehaviour {
	public GroundType Type;

	private void OnTriggerEnter(Collider other) {
		Player Player = other.GetComponent<Player>();
		if (Player != null) {
			Player.CurrentGroundType = Type;
		}
	}
}
