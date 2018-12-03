using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GroundType {
	Stone, Grass
}

public enum TutorialStage {
	None, Pickup, Inventory0, PostInventory0, Inventory1, Done
}

public class Player : MonoBehaviour {
	public Transform PickupTarget;
	public float InteractionDistance = 1.0f; 

	public Transform CamTransform;
	public Bobber CamBobber;
	
	public CharacterController CharacterController;
	public float JumpForce = 1.0f;
	public float MovementSpeed = 1.0f;
	public float SprintingMultiplier = 1.5f;
	
	public Text InventoryDisplay;
	public Text ExaminationText;
	public Text PickupTutorial;
	public Text InventoryTutorial0;
	public Text InventoryTutorial1;

	public Notification MainNotification;

	public GameObject FootstepGrassSfxPrefab;
	public GameObject FootstepStoneSfxPrefab;
	public GroundType CurrentGroundType {
		get {
			return _CurrentGroundType;
		}
		set {
			_CurrentGroundType = value;
			switch (value) {
			case GroundType.Stone:
				CamBobber.BobSfx = FootstepStoneSfxPrefab;
				break;
			case GroundType.Grass:
				CamBobber.BobSfx = FootstepGrassSfxPrefab;
				break;
			}
		}
	}

	public bool LearnedFireball = false;
	public bool PickedupAshes = false;

	public GameObject HappyVillage;
	public GameObject AshesOfTheVillage;

	private GroundType _CurrentGroundType = GroundType.Stone;

	private float VelocityY = 0;
	private float Pitch = 0;

	private Pickup CurrentlyPickedUpObject;
	private List<GameObject> Inventory = new List<GameObject>();
	private int CurrentInventorySelectionIndex = 0;

	private float HoverTextAlpha = 0.0f;

	private TutorialStage CurrentTutorialStage = TutorialStage.None;

	private void Start() {
		Pitch = CamTransform.localEulerAngles.x;
		// TODO: Move cursor handling to pause menu or something
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Color Color = PickupTutorial.color;
		Color.a = 0;
		PickupTutorial.color = Color;

		Color = InventoryTutorial0.color;
		Color.a = 0;
		InventoryTutorial0.color = Color;

		Color = InventoryTutorial1.color;
		Color.a = 0;
		InventoryTutorial1.color = Color;
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
		Move = Move.normalized * MovementSpeed * (Input.GetButton("Sprint") ? SprintingMultiplier : 1);
		CamBobber.Bob = Move.magnitude > 0.05;
		CamBobber.BobFrequency = 2 * (Input.GetButton("Sprint") ? SprintingMultiplier : 1);
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
					int Index = Inventory.IndexOf(CurrentlyPickedUpObject.gameObject);
					if (CurrentInventorySelectionIndex >= Index && CurrentInventorySelectionIndex > 0) {
						CurrentInventorySelectionIndex--;
					}
					Inventory.RemoveAt(Index);
					if (LookedAtObject != null) {
						CurrentlyPickedUpObject.PutDown(hit.point, -hit.normal);
						CurrentlyPickedUpObject = null;
					} else {
						CurrentlyPickedUpObject.ThrowDown();
						CurrentlyPickedUpObject = null;
					}
				} else {
					if (LookedAtObject != null) {
						Pickup Target = LookedAtObject.GetComponent<Pickup>();
						if (Target != null) {
							ItemSounds Sfx = LookedAtObject.GetComponent<ItemSounds>();
							if (Sfx != null) Sfx.Open();
							Target.PickUp(PickupTarget);
							CurrentlyPickedUpObject = Target;
							Inventory.Add(Target.gameObject);
							CurrentInventorySelectionIndex = Inventory.Count - 1;
							if (CurrentTutorialStage == TutorialStage.Pickup) {
								CurrentTutorialStage = TutorialStage.PostInventory0;
							}
							Examinable Examinable = CurrentlyPickedUpObject.GetComponent<Examinable>();
							if (Examinable.Name == ItemName.AshesOfHappyVillage && !PickedupAshes) {
								MainNotification.Notify("<i>Oh! Seems like someone else already burnt the place down for you. How convenient.</i>", 5);
								PickedupAshes = true;
							}
						}
					}
				}
			}
		}
		// The "access inventory" action
		if (Input.GetButtonDown("Put Into / Pull From Bag")) {
			Pickup PreviousPickup = CurrentlyPickedUpObject;
			if (CurrentlyPickedUpObject != null) {
				ItemSounds Sfx = CurrentlyPickedUpObject.GetComponent<ItemSounds>();
				if (Sfx != null) Sfx.Close();
				Examinable Examinable = CurrentlyPickedUpObject.GetComponent<Examinable>();
				if (Examinable.Name == ItemName.FireballBook && !LearnedFireball) {
					MainNotification.Notify("<i>You now know how to cast Fireball!\n<size=20>(You know the title of the game. Pardon.)</size></i>", 5);
					LearnedFireball = true;
					HappyVillage.SetActive(false);
					AshesOfTheVillage.SetActive(true);
				}
				CurrentlyPickedUpObject.gameObject.SetActive(false);
				CurrentlyPickedUpObject = null;
			}
			if (Inventory.Count > 0 && CurrentInventorySelectionIndex < Inventory.Count) {
				GameObject Item = Inventory[CurrentInventorySelectionIndex];
				Pickup ItemPickup = Item.GetComponent<Pickup>();
				if (ItemPickup != PreviousPickup) {
					Item.SetActive(true);
					CurrentlyPickedUpObject = ItemPickup;
					ItemSounds Sfx = Item.GetComponent<ItemSounds>();
					if (Sfx != null) Sfx.Open();
				}
			}
			if (Inventory.Count > 1 && CurrentTutorialStage == TutorialStage.PostInventory0) {
				CurrentTutorialStage = TutorialStage.Inventory1;
			}
		}
		// The "look through inventory" action
		float Scroll = Input.GetAxis("Mouse ScrollWheel");
		if (Scroll != 0 && CurrentTutorialStage == TutorialStage.Inventory1) {
			CurrentTutorialStage = TutorialStage.Done;
		}
		if (Scroll < 0) {
			CurrentInventorySelectionIndex++;
			if (CurrentInventorySelectionIndex >= Inventory.Count) {
				CurrentInventorySelectionIndex = 0;
			}
		} else if (Scroll > 0) {
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
		string NewExaminationText = "";
		if (LookedAtObject != null) {
			Examinable Examinable = LookedAtObject.GetComponent<Examinable>();
			if (Examinable != null) {
				NewExaminationText = Examinable.ToString();
				if (CurrentTutorialStage == TutorialStage.None) {
					CurrentTutorialStage = TutorialStage.Pickup;
				}
			}
		}
		if (NewExaminationText == "") {
			HoverTextAlpha = 0.0f;
		} else {
			HoverTextAlpha = 1.0f;
		}
		Color ExaminationTextColor = ExaminationText.color;
		ExaminationTextColor.a = Mathf.Lerp(ExaminationTextColor.a, HoverTextAlpha, 20f * Time.deltaTime);
		ExaminationText.color = ExaminationTextColor;
		if (NewExaminationText != "") {
			ExaminationText.text = NewExaminationText;
		}

		// Update tutorial texts
		Color Color = PickupTutorial.color;
		Color.a = Mathf.Lerp(Color.a, CurrentTutorialStage == TutorialStage.Pickup ? 1 : 0, 5f * Time.deltaTime);
		PickupTutorial.color = Color;

		Color = InventoryTutorial0.color;
		Color.a = Mathf.Lerp(Color.a, CurrentTutorialStage == TutorialStage.Inventory0 ? 1 : 0, 5f * Time.deltaTime);
		InventoryTutorial0.color = Color;

		Color = InventoryTutorial1.color;
		Color.a = Mathf.Lerp(Color.a, CurrentTutorialStage == TutorialStage.Inventory1 ? 1 : 0, 5f * Time.deltaTime);
		InventoryTutorial1.color = Color;
	}
}

