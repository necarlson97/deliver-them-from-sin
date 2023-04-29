using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour {
    void Update() {
        var ps = GameObject.FindObjectOfType<PlayerScript>();
        transform.position = ps.transform.position;
    }
}
