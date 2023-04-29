using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonScript : MonoBehaviour {


    float speed = 0;
    float speedMax = 4.5f;

    public void Awoken() {
        speed = speedMax;
    }

    void FixedUpdate() {
        var tp = transform.position;
        var direction = new Vector3(speed, 0, 0);
        transform.position = Vector3.Lerp(tp, tp + direction, Time.fixedDeltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            other.GetComponent<PlayerScript>().Kill();
        }
    }
}
