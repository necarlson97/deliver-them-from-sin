using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamScript : MonoBehaviour {

    // Where should the camera be when the player gets up to speed?
    Vector3 fast = new Vector3(20, 0, -10);
    Vector3 start;

    void Start() {
        start = transform.localPosition;
    }

    void Update() {
        // TODO would be cool to have it start zoomed in,
        // then zoom out as player walks away from origin
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        var rc = ps.GetComponent<Rigidbody2D>();

        var speedRatio = rc.velocity.x / ps.walkSpeed;
        if (speedRatio > .95f) speedRatio = 1;

        transform.localPosition = Vector3.Lerp(start, fast, speedRatio);

        Debug.Log(speedRatio);

        // TODO should it point backwards?
    }
}
