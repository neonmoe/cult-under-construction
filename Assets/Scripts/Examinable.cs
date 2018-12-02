using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examinable : MonoBehaviour {
	[Tooltip("An empty name will default to the Game Object name!")]
	public string Name;

	private void Start() {
		if (Name.Length == 0) {
			Name = gameObject.name;
		}
	}
}
