using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioScript : AudioScript {
    public List<AudioClip> steps;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip punch;
    public AudioClip dive;
    public AudioClip grunt;
    public AudioClip moan;
    public AudioClip alarm;


    public void Jump() { Play(jump); }
    public void Punch() { Play(punch, false); }
    public void Dive() { Play(dive, false); }

    public void Step() {
        // The animation actually calls the step audio with the
        // 'enable' property on SFX Step, this just
        // swaps out the clip
        var a = transform.Find("SFX Step").GetComponent<AudioSource>();
        if (!a.isPlaying) a.clip = Choose(steps);
    }

    public void Death() { Play(death); }
    public void Grunt() { Play(grunt); }
    public void Moan() { Play(moan); }
    public void Alarm() { Play(alarm); }
}
