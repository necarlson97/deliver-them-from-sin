using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassScript : MonoBehaviour {
    void Broken(GameObject player) {
        // Must be diving to break through
        if (!player.GetComponent<PlayerScript>().IsDiving()) {
            return;
        }
        GetComponent<Collider2D>().enabled = false;
        transform.Find("Collider").GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        var ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        Debug.Log("Broken");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            Broken(other.gameObject);
        }
    }
}
