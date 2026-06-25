using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModeUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private List<GameModeUIPreset> gameModes = new List<GameModeUIPreset>();

    public void SetMode(SpinGameFlow.SpinGameMode gameMode)
    {
        GameModeUIPreset preset = gameModes.First(x => x.GameMode == gameMode);

        renderer.sprite = preset.sprite;
        renderer.transform.localScale = preset.scale;
        renderer.color = preset.color;
        transform.DOPunchScale(Vector3.one * 0.3f, 0.5f);
    }

    [Serializable]
    public struct GameModeUIPreset
    {
        public string Title;
        public SpinGameFlow.SpinGameMode GameMode;
        public Sprite sprite;
        public Vector3 scale;
        public Color color;
    }
}
