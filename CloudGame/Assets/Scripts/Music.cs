using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicState
{
    None, Sunny, Carrot, Thunder, Storm
}

public class Music : MonoBehaviour
{
    public static Music Main;
    private MusicState State = MusicState.None;
    private float Volume = 1;

    [SerializeField] private AudioClip[] SunnyMusic;
    [SerializeField] private AudioClip CarrotMusic;
    [SerializeField] private AudioClip[] ThunderMusic;
    [SerializeField] private AudioClip StormMusic;

    [SerializeField] public AudioSource[] Sources;
    private int AllocatedSources = 0;

    void Start() 
    { 
        foreach(var source in Sources) source.loop = true;
        Main = this; 
    }

    public void SetVolume(float v)
    {
        v = Mathf.Clamp01(v);
        Volume = v;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        for(int i = 0; i < AllocatedSources; i++) Sources[i].volume = Volume;
    }

    public void Stop()
    {
        foreach(var source in Sources) source.Stop();
    }

    public void PlaySunny()
    {
        if(State != MusicState.Sunny) 
        {
            Stop();
            AllocatedSources = 0;
            for(int i = 0; i < SunnyMusic.Length; i++)
            {
                Sources[i].clip = SunnyMusic[i];
                Sources[i].volume = 0;
                Sources[i].Play();
            }

            State = MusicState.Sunny;
            PlaySunny();
            return;
        }

        Sources[AllocatedSources].volume = Volume;
        AllocatedSources++;
    }

    public void PlayThunder()
    {
        if(State != MusicState.Thunder) 
        {
            Stop();
            AllocatedSources = 0;
            for(int i = 0; i < ThunderMusic.Length; i++)
            {
                Sources[i].clip = ThunderMusic[i];
                Sources[i].volume = 0;
                Sources[i].Play();
            }

            State = MusicState.Thunder;
            PlayThunder();
            return;
        }

        Sources[AllocatedSources].volume = Volume;
        AllocatedSources++;
    }

    public void PlayCarrot()
    {
        Stop();
        AllocatedSources = 1;
        Sources[0].clip = CarrotMusic;
        Sources[0].volume = Volume;
        Sources[0].Play();
    }

    public void PlayStorm()
    {
        Stop();
        AllocatedSources = 1;
        Sources[0].clip = StormMusic;
        Sources[0].volume = Volume;
        Sources[0].Play();
    }
}
