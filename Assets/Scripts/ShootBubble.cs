using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    void FixedUpdate()
    {
        if (check)
            return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity, 0.7f, 1 << 6);
        if (hit.collider != null)
            Debug.Log(hit.transform);
        if (hit.collider != null)
        {
            SetPositionInGreed(hit.transform);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (check)
            return;
        if (collision.transform.tag == "bubble")
            SetPositionInGreed(collision.transform);
    }

    private void SetPositionInGreed(Transform trans)
    {
        check = true;
        Grid.Instance.AddNewBubble(this, trans);
    }
}
