using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadOverlayScript : MonoBehaviour {

    public void Start() {
        var ps = GameObject.FindObjectOfType<PlayerScript>();

        var t = transform.Find("canvas/Delivered").GetComponent<Text>();
        t.text = "Saved "+ps.babiesDelivered+" souls";
        t = transform.Find("canvas/Windows").GetComponent<Text>();
        t.text = "Broken " + ps.windowsBroken + " windows";
        t = transform.Find("canvas/Boxes").GetComponent<Text>();
        t.text = "Broken " + ps.windowsBroken + " boxes";
        t = transform.Find("canvas/Explored").GetComponent<Text>();
        var blockSize = 100;
        var total = 2600/blockSize;
        var explored = Mathf.Round(ps.transform.position.x / blockSize);
        t.text = "Explored " + explored + "/" + total + " city blocks";
    }
    public void Reload() {
        MenuScript.Reload();
    }

    public void Success() {
        var t = transform.Find("canvas/Explored").GetComponent<Text>();
        t.text = "Explored all of the city city blocks!";

        var ds = GameObject.FindObjectOfType<DemonScript>();
        t = transform.Find("canvas/Demon").GetComponent<Text>();
        t.text = "Demon stopped at "+ Mathf.Round(ds.transform.position.x);

        t = transform.Find("canvas/Dead").GetComponent<Text>();
        t.text = "Salvation!";
    }
}
