using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DictorAnimation : MonoBehaviour
{
    [SerializeField] private List<DictorAnimationElement> elements = new List<DictorAnimationElement>();
    [SerializeField] private SpriteRenderer dictorSpriteRen;

    [Space]
    [SerializeField] private GameObject leaderScreen;
    [SerializeField] private TMP_Text tmpLeaders;

    public void SetAnimation( string id) 
    {
        //Debug.Log($"Dictor {id}");
        SetAnimation(elements.First(x => x.ID == id));
    }

    public void SetAnimation(DictorAnimationElement dictorAnimationElement)
    {
        dictorSpriteRen.sprite = dictorAnimationElement.sprite;
    }
    public void SetLeaders()
    {

    }

    public void ActiveLeaderScreen (bool isActive)
    {
        leaderScreen.SetActive(isActive);
    }

    [Serializable]
    public struct DictorAnimationElement
    {
        public string ID;
        public Sprite sprite;
    }
}