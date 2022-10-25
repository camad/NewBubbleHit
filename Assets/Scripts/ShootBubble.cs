using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBubble : Bubble
{
    private Rigidbody2D rb;
    private bool check;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 direction)
    {
        rb.velocity = direction * 2.5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (check)
            return;
        if (collision.transform.tag == "wall")
            rb.velocity = rb.velocity * -1;
        if (collision.transform.tag == "bubble")
            SetPositionInGreed(collision);
    }

    private void SetPositionInGreed(Collision2D collision)
    {
        Debug.Log(collision.transform.tag);
        check = true;
        Grid.Instance.AddNewBubble(this, collision);
    }
}
