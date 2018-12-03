using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum ItemName {
	Unset, NiceFlower, AnotherNote, AshesOfHappyVillage, FireballBook
}

public class Examinable : MonoBehaviour {
	[Tooltip("An empty name will default to the Game Object name!")]
	public ItemName Name;

	public override string ToString() {
		switch (Name) {
		case ItemName.NiceFlower:
			return "Nice Flower";
		case ItemName.AnotherNote:
			return "Another Note";
		case ItemName.AshesOfHappyVillage:
			return "Ashes of Happy Village";
		case ItemName.FireballBook:
			return "On Fire";
		default:
			return gameObject.name;
		}
	}
}
