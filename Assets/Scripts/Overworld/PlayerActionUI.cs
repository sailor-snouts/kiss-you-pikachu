﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActionUI : MonoBehaviour {
    [SerializeField]
    private float iconCheckRadius = 1f;
    [SerializeField]
    private float actionCheckMagnitude = 0.8f;
    private PlayerMovementController movement;
    private Animator animator;
    private SpriteRenderer sprite;
    private List<GameObject> actionable;
    private Inventory inventory;

    void Start()
    {
        this.movement = this.gameObject.GetComponentInParent<PlayerMovementController>();
        this.animator = this.gameObject.GetComponentInParent<Animator>();
        this.inventory = FindObjectOfType<Inventory>();

        this.sprite = this.gameObject.GetComponent<SpriteRenderer>();
        this.actionable = new List<GameObject>();
    }

    void Update()
    {
        if(!this.movement.IsLocked() && this.movement.canLock() && Input.GetButtonDown("Fire1"))
        {
            this.TriggerAction();
        }
    }

    void FixedUpdate()
    {
        this.DisplayTipIcon();
        this.actionable.Clear();
        Vector2 checkDirection;
        switch (this.movement.GetDirection())
        {
            case PlayerAnimation.ANIMATION_WALK_UP:
                checkDirection = new Vector2(0, 1);
                break;
            case PlayerAnimation.ANIMATION_WALK_RIGHT:
                checkDirection = new Vector2(1, 0);
                break;
            case PlayerAnimation.ANIMATION_WALK_DOWN:
                checkDirection = new Vector2(0, -1);
                break;
            case PlayerAnimation.ANIMATION_WALK_LEFT:
                checkDirection = new Vector2(-1, 0);
                break;
            default:
                return;
        }
        Transform parent = this.transform.parent;
        Debug.DrawLine(parent.position, (Vector2) parent.position + checkDirection.normalized * this.actionCheckMagnitude, Color.red);
        RaycastHit2D[] hitColliders = Physics2D.RaycastAll(parent.position, checkDirection.normalized, this.actionCheckMagnitude);
        foreach (RaycastHit2D col in hitColliders)
        {
            GameObject obj = col.transform.gameObject;
            if (obj.GetComponent<Actionable>() || obj.GetComponent<DialogueTrigger>() || obj.GetComponent<Questionaire>() || obj.GetComponent<ChefController>())
            {
                this.actionable.Add(obj);
            }
        }
    }

    void TriggerAction()
    {
        foreach (GameObject obj in this.actionable)
        {
            Debug.Log("Logging interaction with " + obj.tag);

            switch (obj.tag)
            {
                case "Porkachu":
                    PorkachuController porkachu = obj.GetComponent<PorkachuController>();
                    porkachu.Pickup(inventory);
                    break;
                case "NPC":
                case "Tree":
                case "Menu":
                case "Wastebasket":
                    DialogueTrigger dialogue = obj.GetComponent<DialogueTrigger>();
                    dialogue.TriggerDialogue(this.movement);
                    break;
                case "ItemPickup":
                    dialogue = obj.GetComponent<DialogueTrigger>();
                    dialogue.TriggerDialogue(this.movement);
                    PickupController pickup = obj.GetComponent<PickupController>();
                    pickup.Pickup(inventory);
                    break;
                case "Question":
                    // @TODO Move this add Key to an action post-dialogue
                    inventory.AddKey(KeyType.RESTAURANT);
                    Questionaire dialogueQuestion = obj.GetComponent<Questionaire>();
                    dialogueQuestion.Ask(this.movement, this.inventory);
                    break;
                case "Door":
                    Door door = obj.GetComponent<Door>();
                    door.TakeAction(this.movement, this.inventory);
                    break;
                case "Chef":
                    ChefController chef = obj.GetComponent<ChefController>();
                    chef.StartEncounter();
                    break;
                default: 
                    break;
            }
        }
    }

    void DisplayTipIcon()
    {
        this.sprite.enabled = this.actionable.Count > 0;
    }
}
