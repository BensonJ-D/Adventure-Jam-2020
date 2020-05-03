using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGlow : MonoBehaviour
{
    SpriteRenderer renderer;
    [SerializeField] private float randomFlickerAmount;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        float alpha = renderer.color.a;
        float minGlow = Mathf.Max(0.5f, alpha - randomFlickerAmount);
        float maxGlow = Mathf.Min(1.0f, alpha + randomFlickerAmount);
        
        alpha = Random.Range(minGlow, maxGlow);

        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
    }
}
