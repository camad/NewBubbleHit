using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ShootBubble : Bubble
{
    private Rigidbody2D rb;
    private bool check;
    private Vector3 targetPosition = Vector3.zero;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Shoot(Vector2 direction)
    {
        rb.velocity = direction.normalized * 30f;
    }


    void FixedUpdate()
    {
        if (check)
            return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity, 1f, 1 << 6);
        if (hit.collider != null)
        {
            targetPosition = Grid.Instance.GetBubbleNewPosition(this, hit.transform);
            animator.Play("ShootReady");
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
        if(targetPosition == Vector3.zero)
            targetPosition = Grid.Instance.GetBubbleNewPosition(this, trans);
        rb.velocity = Vector2.zero;
        animator.Play("ShootReady");
    }
    public void ShootEnd()
    {
        Grid.Instance.ShootEnd(this, targetPosition);
    }
}
