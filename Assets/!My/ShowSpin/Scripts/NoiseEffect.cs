using System.Collections;
using UnityEngine;

public class NoiseEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float dealy = 0.5f;

    void Start()
    {
        StartCoroutine(UpdateNoiseRoutine());   
    }

    private IEnumerator UpdateNoiseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dealy);
            UpdateNoise();
        }
    }

    private void UpdateNoise()
    {
        Vector3 scale = transform.localScale;
        spriteRenderer.transform.localScale = new Vector3(scale.x * Mathf.Sign(Random.Range(-2, 2)), 
            scale.y * Mathf.Sign(Random.Range(-2, 2)), 
            scale.z * Mathf.Sign(Random.Range(-2, 2)));
        spriteRenderer.sprite = sprites.RandomElement();
    }
}
