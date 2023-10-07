using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer mixer;

    public static float volume { get; private set; }

    private float volumeInterval = 0.1f;
    void Start()
    {
        UpdateMixerVolume(volume);
    }

    void Update()
    {
        if (Input.GetButtonDown("Decrease Volume") && volume > 0.001f)
        {
            if (volume < 0.001f)
            {
                volume = 0.001f;
            }
            UpdateMixerVolume(volume -= volumeInterval);
        }
        else if (Input.GetButtonDown("Increase Volume") && volume < 1)
        {
            if (volume == 0.001f)
            {
                volume = 0;
            }
            UpdateMixerVolume(volume += volumeInterval);
        }
    }
    private void UpdateMixerVolume(float volume)
    {
        mixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
    public static void SetVolume(float v)
    {
        volume = v;
    }
}
