using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamesScript : MonoBehaviour {
    void LateUpdate() {
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        transform.position = new Vector3(
            ps.transform.position.x, transform.position.y, transform.position.z);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            other.GetComponent<PlayerScript>().Kill();
        }
    }
}
