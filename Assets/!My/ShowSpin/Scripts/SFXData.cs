using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXData", menuName = "ChessNock/SFX/SFXData")]
public class SFXData : ScriptableObject
{
    public List<AudioDataPlay> MusicGamePlay = new List<AudioDataPlay>();
}