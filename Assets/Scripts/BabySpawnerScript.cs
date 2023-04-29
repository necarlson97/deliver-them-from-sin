using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySpawnerScript : MonoBehaviour {

    public GameObject babyPrefab;
    bool spawned = false;


    private void OnTriggerEnter2D(Collider2D other) {
        if (spawned) return;
        
        // TODO particle burst incase player can see it?
        if (other.gameObject.name == "Player") {
            // Spawn baby when player is near enough
            spawned = true;

            Instantiate(babyPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
