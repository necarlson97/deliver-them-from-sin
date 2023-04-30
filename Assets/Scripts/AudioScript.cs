using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioScript : MonoBehaviour {
    public void Play(AudioClip clip, bool interrupt=true) {
        var a = GetComponentInChildren<AudioSource>();
        if (a.isPlaying && !interrupt) return;
        a.clip = clip;
        a.pitch = Random.Range(0.9f, 1.1f);
        a.Play();
    }
    public void Play(List<AudioClip> clips, bool interrupt=true) {
        // Unity c# choose random from list
        Play(Choose(clips), interrupt);
    }

    public static T Choose<T>(List<T> list) {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
