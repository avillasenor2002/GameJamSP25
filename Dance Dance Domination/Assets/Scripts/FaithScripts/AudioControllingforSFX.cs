using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControllingforSFX : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource fusionSound;

    public void MuteAllExceptFusion()
    {
        mixer.SetFloat("TempoBarBeat", -80f);
        mixer.SetFloat("SFX", -80f);
        mixer.SetFloat("FusionSFX", 0f);

        if (!fusionSound.isPlaying)
        {
            fusionSound.Play();
        }
    }

    public void RestoreAll()
    {
        mixer.SetFloat("TempoBarBeat", 0f);
        mixer.SetFloat("SFX", 0f);
        mixer.SetFloat("FusionSFX", 0f);

        if (fusionSound.isPlaying)
        {
            fusionSound.Stop();
        }

    }

}
