using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    protected Animator animator;
    public Color Color;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetColor(Color color)
    {
        Color = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void DestroyAnimation()
    {
        animator.Play("bubbleDestroy");
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
