using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public Color Color;

    public void SetColor(Color color)
    {
        Color = color;
        GetComponent<SpriteRenderer>().color = color;
    }
}
