using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimneyAudioScript : AudioScript {
    public AudioClip deposit;

    public void Deposit() { Play(deposit); }
}
