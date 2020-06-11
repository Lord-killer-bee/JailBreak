using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    int currentIndex = 0;
    Image textImage;
    DateTime startTime;

    private void Start()
    {
        currentIndex = 0;
        textImage = GetComponent<Image>();
        textImage.sprite = sprites[currentIndex];
        startTime = DateTime.Now;
    }

    private void Update()
    {
        if((DateTime.Now - startTime).TotalMilliseconds >= 450)
        {
            currentIndex++;
            textImage.sprite = sprites[currentIndex];
            startTime = DateTime.Now;

            if (currentIndex == sprites.Length - 1)
                currentIndex = -1;
        }
    }

    public void SetSprites(Sprite[] sprites)
    {
        this.sprites = sprites;
    }
}
