using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassAudioScript : AudioScript {
    public List<AudioClip> broken;
    public void Broken() { Play(broken); }
}
