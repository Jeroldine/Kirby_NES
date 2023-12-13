using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourceManager : MonoBehaviour
{
    List<AudioSource> currentAudioSources = new List<AudioSource>();
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] AudioMixerGroup sfxGroup;

    // Start is called before the first frame update
    void Start()
    {
        currentAudioSources.Add(GetComponent<AudioSource>());
    }

    public void PlayOneShot(AudioClip clip, bool isMusic)
    {
        foreach (AudioSource source in currentAudioSources)
        {
            if (source.isPlaying)
                continue;
            source.clip = clip;
            source.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
            source.time = 0;
            source.Play();
            return;
        }
        AudioSource temp = gameObject.AddComponent<AudioSource>();
        currentAudioSources.Add(temp);
        temp.clip = clip;
        temp.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
        temp.time = 0;
        temp.Play();
    }

    public void PlayWithDelay(AudioClip clip, bool isMusic)
    {
        float clipLength = clip.length;
        bool foundClip = false;

        foreach (AudioSource source in currentAudioSources)
        {
            if (source.isPlaying && source.clip == clip && !foundClip && source.time >= (clipLength/2))
            {
                foundClip = true;
                continue;
            }
            else if (source.isPlaying)
                continue;

            source.clip = clip;
            source.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
            source.time = clipLength / 2;
            source.PlayDelayed((clipLength / 2)*0.9f);
            return;
        }

        if (!foundClip)
            return;
        else
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            currentAudioSources.Add(temp);
            temp.clip = clip;
            temp.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
            temp.time = clipLength / 2;
            temp.PlayScheduled(clipLength / 2);
        }
    }

    public void StopClip(AudioClip clip)
    {
        foreach (AudioSource source in currentAudioSources)
        {
            if (source.isPlaying && source.clip == clip)
                source.Stop();
        }
    }

    //public void PlayLoop(AudioClip clip, bool isMusic, float delay)
    //{
    //    foreach (AudioSource source in currentAudioSources)
    //    {
    //        if (source.isPlaying)
    //            continue;
    //        source.clip = clip;
    //        source.loop = true;
    //        source.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
    //        source.time = 0;
    //        source.Play();
    //        source.time = delay;
    //        return;
    //    }
    //    AudioSource temp = gameObject.AddComponent<AudioSource>();
    //    currentAudioSources.Add(temp);
    //    temp.clip = clip;
    //    temp.loop = true;
    //    temp.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
    //    temp.time = 0;
    //    temp.Play();
    //    temp.time = delay;
    //}
}
