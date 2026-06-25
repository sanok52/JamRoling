using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Material[] materials;

    private void OnEnable()
    {
        Material material = materials.RandomElement();
        foreach (var renderer in renderers)
        {
            renderer.material = material;
        }
    }
}
