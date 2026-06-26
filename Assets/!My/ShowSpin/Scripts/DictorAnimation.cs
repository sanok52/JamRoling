using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static DictorAnimation;

public class DictorAnimation : MonoBehaviour
{
    [SerializeField] private List<DictorAnimationElement> elements = new List<DictorAnimationElement>();
    [SerializeField] private SpriteRenderer dictorSpriteRen;

    [Space]
    [SerializeField] private GameObject leaderScreen;
    [SerializeField] private TMP_Text tmpLeaders;

    private DictorAnimationElement currentAnimation;
    private int currentIndex = 0;

    private void Start()
    {
        SetAnimation("You");
        StartCoroutine(UpdateAnimation());
    }

    public void SetAnimation(string id) 
    {
        //Debug.Log($"Dictor {id}");
        SetAnimation(elements.First(x => x.ID == id));
    }

    public void SetAnimation(DictorAnimationElement dictorAnimationElement)
    {
        currentIndex = 0;
        currentAnimation = dictorAnimationElement;
        dictorSpriteRen.sprite = dictorAnimationElement.sprites[0];
    }
    public void SetLeaders()
    {

    }

    public void ActiveLeaderScreen (bool isActive)
    {
        leaderScreen.SetActive(isActive);
    }

    private IEnumerator UpdateAnimation()
    {
        while (true)
        {
            dictorSpriteRen.sprite = currentAnimation.sprites[currentIndex];
            yield return new WaitForSeconds(currentAnimation.duration);
            currentIndex++;
            if (currentIndex >= currentAnimation.sprites.Length)
                currentIndex = 0;
        }
    }

    [Serializable]
    public struct DictorAnimationElement
    {
        public string ID;
        public Sprite[] sprites;
        public float duration;
    }
}