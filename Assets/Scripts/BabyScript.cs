using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyScript : MonoBehaviour {
    void PickedUp(GameObject player) {
        var ps = transform.parent.Find("God Rays").GetComponent<ParticleSystem>();
        ps.Stop();

        transform.parent = player.transform;
        transform.localPosition = new Vector3(-.4f, .4f, 5);
        GetComponent<SpriteRenderer>().flipX = true;

        // GetComponent<Collider2D>().enabled = false;
        transform.Find("Collider").GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().simulated = false;

        var ds = GameObject.FindObjectOfType<DemonScript>();
        ds.Awoken();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            PickedUp(other.gameObject);
        }
    }
}
