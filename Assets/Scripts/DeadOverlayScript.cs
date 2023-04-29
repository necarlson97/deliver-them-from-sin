using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadOverlayScript : MonoBehaviour {

    public void Start() {
        var ps = GameObject.FindObjectOfType<PlayerScript>();

        var t = transform.Find("canvas/Delivered").GetComponent<Text>();
        t.text = "Saved "+ps.babiesDelivered+" souls";
        t = transform.Find("canvas/Broken").GetComponent<Text>();
        t.text = "Broken " + ps.windowsBroken + " windows";
        t = transform.Find("canvas/Explored").GetComponent<Text>();
        t.text = "Explored X/X city blocks";
    }
    public void Reload() {
        MenuScript.Reload();
    }
}
