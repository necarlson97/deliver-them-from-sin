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


    public void Jump() { Play(jump); }
    public void Punch() { Play(punch, false); }
    public void Dive() { Play(dive, false); }

    public void Step() { Play(steps, false); }

    public void Death() { Play(death); }
    public void Grunt() { Play(grunt); }
}
