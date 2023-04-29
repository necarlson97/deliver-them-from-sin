using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxItemScript : MonoBehaviour {
    
    void OnBecameInvisible() {
        Debug.Log("invisible");
        // Wrap elements that go off screen
        var wrap = new Vector3(100, 0, 0);
        if (transform.localPosition.x > 0) {
            wrap *= -1;
        }
        transform.position = transform.position + wrap;
    }
}
