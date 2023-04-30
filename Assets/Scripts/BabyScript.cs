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

        transform.Find("Collider").GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<SpriteRenderer>().enabled = false;

        var ds = GameObject.FindObjectOfType<DemonScript>();
        ds.Awoken();
        GetComponent<BabyAudioScript>().Pickup();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            PickedUp(other.gameObject);
        }
    }

    public void DelayedWhine() {
        Invoke("Whine", 1f);
    }

    public void Whine() {
        GetComponent<BabyAudioScript>().Whine();
    }
}
