using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Timeline.TimelineAsset;

public class MusicManager
{
    private AudioSource musicSource;

    public static SFXData SFXData;
    private Dictionary<string, AudioDataPlay> musics = new Dictionary<string, AudioDataPlay>();

    public void Init()
    {
        musicSource = GameObject.Find("AudioSourceMusic").GetComponent<AudioSource>();
        SFXData = Resources.Load<SFXData>("SFXData");

        foreach (var music in SFXData.MusicGamePlay)
        {
            musics.Add(music.ID, music);
        }
    }

    public void StopMusic()
    {
        musicSource.DOFade(0f, 2f).SetUpdate(true).SetAutoKill(true);
    }

    public void PlayMusic (string id, float durationFade = 1f)
    {
        musicSource.DOFade(0.32f, durationFade).SetUpdate(true).SetAutoKill(true);
        musicSource.Play(musics[id]);
    }

    public void SetPitch(float pitch)
    {
        //musicSource.volume = 0.45f;
        musicSource.DOPitch(pitch, 0.5f).SetUpdate(true);
    }

    internal void SetVolume(float v)
    {
        //musicSource.DOFade(v, 0.5f).SetUpdate(true);
    }
}