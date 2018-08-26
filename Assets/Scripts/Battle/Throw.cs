﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public GameObject itemPrefab;
    public GameObject bunPrefab;
    public List<Sprite> itemSprites;
    private  SpriteRenderer spriteR;
    [SerializeField, Range(-5, 5)]
    private int startingHeight = -3;
    [SerializeField, Range(0, 0.5f)]
    private float throwDelay = 0.25f;
    [SerializeField, Range(0, 5f)]
    private float targetYPosition = 3.5f;
    //This state should be fetched from the game manager or something, just here for testing
    private bool throwBun = false;

    void Start() {
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            // Call ThrowItem on a slight delay to give a sense of a wind up before the throw happens.
            Invoke("ThrowItem", throwDelay);
        }
    }

    private void ThrowItem() {
        Vector2 targetPosition = new Vector2(transform.position.x, targetYPosition);
        Vector2 pos = new Vector2(targetPosition.x, startingHeight);
        GameObject item = GetItemToThrow(pos);
        item.GetComponent<BattleItemController>().setTargetPosition(targetPosition);
    }

    private void SetRandomSprite(GameObject item) {
        Sprite randomSprite = itemSprites[Random.Range(0, itemSprites.Count)];
        item.GetComponent<SpriteRenderer>().sprite = randomSprite;
    }

    private GameObject GetItemToThrow(Vector2 position) {
        if(throwBun) {
            return Instantiate(bunPrefab, position, Quaternion.identity);
        } else {
            GameObject itemInstance = Instantiate(itemPrefab, position, Quaternion.identity);
            SetRandomSprite(itemInstance);
            return itemInstance;
        }
    }
}
