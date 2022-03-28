using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSpriteRenderer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private int fps;
    [SerializeField] private float fpsCounter;
    private int frame;

    // Update is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            frame++;
            fpsCounter = 0f;
            
            if (frame >= sprites.Length) frame = 0;

            spriteRenderer.sprite = sprites[frame];

        }
        
    }
}
