using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

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
        musicSource.DOFade(0f, 2f);
    }

    public void PlayMusic (string id, float durationFade = 1f)
    {
        musicSource.DOFade(0.45f, durationFade);
        musicSource.Play(musics[id]);
    }
}