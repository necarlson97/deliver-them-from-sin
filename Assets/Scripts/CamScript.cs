using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamScript : MonoBehaviour {

    // Where should the camera be when the player gets up to speed?
    Vector3 fast = new Vector3(20, 0, -10);
    Vector3 start;

    float fastZoom = 20;
    float startZoom = 5;

    public float ratio;

    void Start() {
        start = transform.localPosition;
        // startZoom = GetComponent<Camera>().orthographicSize;
    }

    void FixedUpdate() {
        PlayerSpeed();
        CloseToStart();
        CloseToDemon();
    }

    void PlayerSpeed() {
        // Move to show more forward space as player moves forward
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        var rc = ps.GetComponent<Rigidbody2D>();

        var speedRatio = rc.velocity.x / ps.walkSpeed;

        // Slow to speed up, fast to slow down
        if (ratio < speedRatio) {
            ratio = Mathf.Lerp(ratio, speedRatio, 0.02f);
        } else {
            ratio = Mathf.Lerp(ratio, speedRatio, 0.1f);
        }
    
        ratio = Mathf.Min(1, Mathf.Max(0, ratio));

        transform.localPosition = Vector3.Lerp(start, fast, ratio);
    }

    void CloseToStart() {
        // zoom out as player walks away from origin
        var distance = transform.position.magnitude / 100f;
        distance = Mathf.Min(1, Mathf.Max(0, distance));
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(startZoom, fastZoom, distance);
    }

    void CloseToDemon() {
        var ds = GameObject.FindObjectOfType<DemonScript>();
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        var distance = Vector2.Distance(ds.transform.position, ps.transform.position);

        distance = distance / 40f;
        distance = Mathf.Min(1, Mathf.Max(0, distance));
        var sr = GetComponentInChildren<SpriteRenderer>();
        var tmp = sr.color;
        tmp.a = 1f - distance;
        sr.color = tmp;

        // TODO camera shake as well
    }
}
