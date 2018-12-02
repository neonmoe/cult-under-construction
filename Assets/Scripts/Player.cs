using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public Transform PickupTarget;
	public float InteractionDistance = 1.0f; 

	public Transform CamTransform;
	public Bobber CamBobber;
	
	public CharacterController CharacterController;
	public float JumpForce = 1.0f;
	public float MovementSpeed = 1.0f;
	
	public Text InventoryDisplay;
	public Text ExaminationText;

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
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		GameObject LookedAtObject = null;
		RaycastHit hit;
		if (Physics.Raycast(CamTransform.position, CamTransform.forward, out hit, InteractionDistance)) {
			LookedAtObject = hit.collider.attachedRigidbody != null ? hit.collider.attachedRigidbody.gameObject : hit.collider.gameObject;
		}

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
		// The "pick up" action
		if (Input.GetButtonDown("Pick Up / Drop Item")) {
			if (LookedAtObject != null && LookedAtObject.GetComponent(typeof(IInteractable)) != null) {
				IInteractable Interactable = (IInteractable) LookedAtObject.GetComponent(typeof(IInteractable));
				Interactable.Interact(CurrentlyPickedUpObject != null ? CurrentlyPickedUpObject.gameObject : null);
			} else {
				if (CurrentlyPickedUpObject != null) {
					bool CloseEnough = (CurrentlyPickedUpObject.GetOriginalPosition() - CamTransform.position).magnitude < InteractionDistance * .5f;
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
					if (LookedAtObject != null) {
						Pickup Target = LookedAtObject.GetComponent<Pickup>();
						if (Target != null) {
							Target.PickUp(PickupTarget);
							CurrentlyPickedUpObject = Target;
							Inventory.Add(Target.gameObject);
							CurrentInventorySelectionIndex = Inventory.Count - 1;
						}
					}
				}
			}
		}
		// The "access inventory" action
		if (Input.GetButtonDown("Put Into / Pull From Bag")) {
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
		// The "look through inventory" action
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

		for (int i = 1; i <= 10; i++) {
			if (Input.GetKeyDown(KeyCode.Alpha1)) { CurrentInventorySelectionIndex = Mathf.Clamp(0, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha2)) { CurrentInventorySelectionIndex = Mathf.Clamp(1, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha3)) { CurrentInventorySelectionIndex = Mathf.Clamp(2, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha4)) { CurrentInventorySelectionIndex = Mathf.Clamp(3, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha5)) { CurrentInventorySelectionIndex = Mathf.Clamp(4, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha6)) { CurrentInventorySelectionIndex = Mathf.Clamp(5, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha7)) { CurrentInventorySelectionIndex = Mathf.Clamp(6, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha8)) { CurrentInventorySelectionIndex = Mathf.Clamp(7, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha9)) { CurrentInventorySelectionIndex = Mathf.Clamp(8, 0, Inventory.Count); }
			if (Input.GetKeyDown(KeyCode.Alpha0)) { CurrentInventorySelectionIndex = Mathf.Clamp(9, 0, Inventory.Count); }
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

		// Update examination text
		ExaminationText.text = "";
		if (LookedAtObject != null) {
			Examinable Examinable = LookedAtObject.GetComponent<Examinable>();
			if (Examinable != null) {
				ExaminationText.text = Examinable.Name;
			}
		}
	}
}

