using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryScript : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            var ds = GameObject.FindObjectOfType<DemonScript>();
            ds.Stopped();
        }
    }
}
