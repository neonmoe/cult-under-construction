using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
	private void Update() {
		Vector3 CameraPos = Camera.main.transform.position;
		transform.LookAt(CameraPos);
	}
}
