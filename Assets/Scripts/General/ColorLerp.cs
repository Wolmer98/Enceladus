using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLerp : MonoBehaviour
{
    float lerpValue = 0;
    public float speed = 1;
    public Image image;
    public Color targetColor;
    Color startColor;

    void Start()
    {
        lerpValue = 0;
        startColor = image.color;
    }

    void Update()
    {
        lerpValue += Time.deltaTime * speed;
        image.color = Color.Lerp(startColor, targetColor, lerpValue);
    }
}
