using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioController : MonoBehaviour
{
    public AudioMixer mixer;
    //public AudioMixerGroup musicGroup;

    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider masterSlider;
    //Slider min value is -80 and max value 0

    // Start is called before the first frame update
    void Start()
    {
        float musicVolume = 0;

        // "MusicVol" is referencing the name of the manually exposed parameters of each individual audio group
        mixer.GetFloat("MusicVol", out musicVolume);

        musicSlider.value = musicVolume;

        float masterVolume = 0;

        mixer.GetFloat("MasterVol", out masterVolume);

        masterSlider.value = masterVolume;
    }

    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat("MusicVol", musicSlider.value);
        mixer.SetFloat("MasterVol", masterSlider.value);
        
    }
}
