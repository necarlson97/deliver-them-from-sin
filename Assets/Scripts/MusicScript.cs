using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {

    public AudioClip intro;
    public AudioClip theme;


    public void Theme() {
        var a = GetComponentInChildren<AudioSource>();
        a.clip = theme;
        a.Play();
    }
}