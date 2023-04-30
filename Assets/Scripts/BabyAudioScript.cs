using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyAudioScript : AudioScript {
    public AudioClip whine;
    public AudioClip pickup;

    public void Whine() { Play(whine); }
    public void Pickup() { Play(pickup); }
}
