using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorkScript : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            var ps = other.GetComponent<PlayerScript>();
            ps.Win();
            var ds = GameObject.FindObjectOfType<DemonScript>();
            ds.Stopped();
        }
    }
}
