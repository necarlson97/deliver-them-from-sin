using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScript : MonoBehaviour {

    public Vector3 lastPos;
    
    void Start() {
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        lastPos = ps.transform.position;
    }

    
    void LateUpdate() {
        // TODO might be sloppy, but for now, use mow much they moved
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        var v = ps.transform.position.x - lastPos.x;

        // TODO could array but eh
        foreach (Transform t in transform.Find("Layer 1")) {
            Nudge(t, v * .8f);
            Wrap(t);
        }
        foreach (Transform t in transform.Find("Layer 2")) {
            Nudge(t, v * .5f);
            Wrap(t);
        }
        foreach (Transform t in transform.Find("Layer 3")) {
            Nudge(t, v * .3f);
            Wrap(t);
        }

        lastPos = ps.transform.position;
    }

    void Nudge(Transform t, float x) {
        // Move the background building a bit on the x axis
        t.position = t.position - new Vector3(x, 0, 0);
    }

    void Wrap(Transform t) {
        // If it went off the edge of the screen, wrap it back to the front
        var wrap = new Vector3(200, 0, 0);
        if (t.localPosition.x < -100) {
            t.position = t.position + wrap;
        } else if (t.localPosition.x > 100) {
            t.position = t.position - wrap;
        }
    }
}
