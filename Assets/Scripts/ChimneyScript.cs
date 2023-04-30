using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChimneyScript : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.name == "Player") {
            int deposited = other.GetComponent<PlayerScript>().Deposit();
            // If it was unsuccessful
            if (deposited == -1) return;

            var count = GetComponentInChildren<Text>();
            count.text = deposited + " delivered";

            var smoke = transform.Find("Smoke").GetComponent<ParticleSystem>();
            smoke.Stop();

            var confetti = transform.Find("Confetti").GetComponent<ParticleSystem>();
            confetti.Play();
            GetComponent<ChimneyAudioScript>().Deposit();
        }
    }
}
