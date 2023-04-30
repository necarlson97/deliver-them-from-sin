using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSliderScript : MonoBehaviour {

    float sfx = 0.4f;
    float music = 0.3f;
    
    void Start() {
        UpdateAll();    
    }

    void UpdateAll() {
        // Bring all audio sources in alignment with
        // the sfx and music floats here
        Debug.Log(FindObjectsOfType<AudioSource>());
        foreach(var source in FindObjectsOfType<AudioSource>()) {
            if (source.name == "Music") source.volume = music;
            else source.volume = sfx;
        }
        transform.Find("SFX Slider").GetComponent<Slider>().value = sfx;
        transform.Find("Music Slider").GetComponent<Slider>().value = music;
    }

    public void ChangeMusic(float v) {
        music = v;
        UpdateAll();
    }

    public void ChangeSFX(float v) {
        sfx = v;
        UpdateAll();
    }
}