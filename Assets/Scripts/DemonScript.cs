using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonScript : MonoBehaviour {


    float speed = 0;
    float speedMax = 4.3f;

    public void Awoken() {
        if (speed > 0) return;
        speed = speedMax;
        var cs = GameObject.FindObjectOfType<CamScript>();
        cs.Shake(1.5f);
    }

    public void Stopped() {
        speed = 0;
    }

    void FixedUpdate() {
        if (speed == 0) return;
        
        var tp = transform.position;
        var direction = new Vector3(speed, 0, 0);
        transform.position = Vector3.Lerp(tp, tp + direction, Time.fixedDeltaTime * speed);

        // Cheaty little telleport
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        if (Mathf.Abs(transform.position.x-ps.transform.position.x) > 200) {
            transform.position = transform.position + new Vector3(10, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            other.GetComponent<PlayerScript>().Kill();
        }
    }
}
