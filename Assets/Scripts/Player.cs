using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public Transform PickupTarget;
	public float PickupDistance = 1.0f; 
	public Transform CamTransform;
	public Bobber CamBobber;
	public CharacterController CharacterController;
	public float JumpForce = 1.0f;
	public float MovementSpeed = 1.0f;
	public Text InventoryDisplay;

	private float VelocityY = 0;
	private float Pitch = 0;
	private Pickup CurrentlyPickedUpObject;
	private List<GameObject> Inventory = new List<GameObject>();
	private int CurrentInventorySelectionIndex = 0;

	private void Start() {
		Pitch = CamTransform.localEulerAngles.x;
		// TODO: Move cursor handling to pause menu or something
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	private void Update() {
		// Camera control
		Vector2 MouseLook = Config.GetMouseLook();
		Pitch = Mathf.Clamp(Pitch + MouseLook.y, -89f, 89f);
		Vector3 NewEulers = CamTransform.localEulerAngles;
		NewEulers.x = Pitch;
		NewEulers.y += MouseLook.x;
		CamTransform.localEulerAngles = NewEulers;

		// Character movement
		Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Move = CamTransform.TransformDirection(Move);
		Move.y = 0;
		Move = Move.normalized * MovementSpeed;
		CamBobber.Bob = Move.magnitude > 0.05;
		if (CharacterController.isGrounded) {
			if (Input.GetButtonDown("Jump")) {
				VelocityY = JumpForce;
			} else {
				VelocityY = 0;
			}
		} else {
			VelocityY += Physics.gravity.y * Time.deltaTime;			
		}
		Move.y = VelocityY;
		CharacterController.Move(Move * Time.deltaTime);

		// Pickups
		if (Input.GetButtonDown("Fire1")) {
			if (CurrentlyPickedUpObject != null) {
				bool CloseEnough = (CurrentlyPickedUpObject.GetOriginalPosition() - CamTransform.position).magnitude < PickupDistance * .5f;
				int Index = Inventory.IndexOf(CurrentlyPickedUpObject.gameObject);
				if (CurrentInventorySelectionIndex >= Index && CurrentInventorySelectionIndex > 0) {
					CurrentInventorySelectionIndex--;
				}
				Inventory.RemoveAt(Index);
				if (CloseEnough) {
					CurrentlyPickedUpObject.PutDown();
					CurrentlyPickedUpObject = null;
				} else {
					CurrentlyPickedUpObject.ThrowDown();
					CurrentlyPickedUpObject = null;
				}
			} else {
				RaycastHit hit;
				if (Physics.Raycast(CamTransform.position, CamTransform.forward, out hit, PickupDistance)) {
					Pickup Target = hit.collider.GetComponent<Pickup>();
					if (Target != null) {
						Target.PickUp(PickupTarget);
						CurrentlyPickedUpObject = Target;
						Inventory.Add(Target.gameObject);
						CurrentInventorySelectionIndex = Inventory.Count - 1;
					}
				}
			}
		}
		if (Input.GetButtonDown("Fire2")) {
			Pickup PreviousPickup = CurrentlyPickedUpObject;
			if (CurrentlyPickedUpObject != null) {
				CurrentlyPickedUpObject.gameObject.SetActive(false);
				CurrentlyPickedUpObject = null;
			}
			if (Inventory.Count > 0 && CurrentInventorySelectionIndex < Inventory.Count) {
				GameObject Item = Inventory[CurrentInventorySelectionIndex];
				Pickup ItemPickup = Item.GetComponent<Pickup>();
				if (ItemPickup != PreviousPickup) {
					Item.SetActive(true);
					CurrentlyPickedUpObject = ItemPickup;
				}
			}
		}
		float Scroll = Input.GetAxis("Mouse ScrollWheel");
		if (Scroll > 0) {
			CurrentInventorySelectionIndex++;
			if (CurrentInventorySelectionIndex >= Inventory.Count) {
				CurrentInventorySelectionIndex = 0;
			}
		} else if (Scroll < 0) {
			CurrentInventorySelectionIndex--;
			if (CurrentInventorySelectionIndex < 0) {
				CurrentInventorySelectionIndex = Inventory.Count - 1;
			}
		}

		// Update inventory text
		string InventorySummary = "";
		if (Inventory.Count > 0) {
			InventorySummary += "Inventory:\n";
			for (int i = 0; i < Inventory.Count; i++) {
				GameObject Item = Inventory[i];
				Pickup ItemPickup = Item.GetComponent<Pickup>();
				InventorySummary += (i + 1) + ". ";
				if (CurrentlyPickedUpObject == ItemPickup) {
					InventorySummary += "<b>" + Item.name + "</b>";
				} else {
					InventorySummary += Item.name;
				}
				if (i == CurrentInventorySelectionIndex) {
					if (CurrentlyPickedUpObject != ItemPickup) {
						InventorySummary += " (Right-click to equip)";
					} else {
						InventorySummary += " (Right-click to unequip)";
					}
				}
				InventorySummary += "\n";
			}
		}
		InventoryDisplay.text = InventorySummary;
	}
}

