using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ShootBubble : Bubble
{
    private Rigidbody2D rb;
    private bool check;
    private Vector3 targetPosition = Vector3.zero;
    private Animator animator;
    private Vector3 direction;
    private Vector3 startDirection;
    private bool targetIsWall = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Shoot(Vector3 direction, Vector3 startDirection, bool targetIsWall = false)
    {
        if (transform)
        {
            this.targetIsWall = targetIsWall;
            this.startDirection = startDirection;
            //if(targetIsWall)
            //    rb.velocity = direction.normalized * 10f;
            //else
            //{
            this.direction = direction;
            //}
        }
        //rb.velocity = direction.normalized * 30f;
    }

    void FixedUpdate()
    {
        if(direction != Vector3.zero)   
        transform.position = Vector3.MoveTowards(transform.position, direction, 0.5f);

        if (targetIsWall)
            return;
        if (transform.position.normalized == direction.normalized)
            animator.Play("ShootReady");

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (targetIsWall)
        //{
        //    RayCast(transform.position, rb.velocity);
        //    return;
        //}
        if (collision.transform.tag == "bubble")
        {
            animator.Play("ShootReady");
        }
        if (collision.transform.tag == "wall")
        {
            direction = transform.position - startDirection;
            direction = new Vector3(direction.x * -1, direction.y, 0);
            if (RayCast(transform.position, direction))
                return;
            Shoot(direction, transform.position, true);
            return;
        }
    }


    private bool RayCast(Vector3 startPosition, Vector3 direction)
    {
        LayerMask layers = 1 << 6 | 1 << 8;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, 30f, layers);
        if (hit.collider.tag == "bubble")
        {
            direction = Grid.Instance.GetBubbleNewPosition1(hit);
            Shoot(direction, startDirection);
            targetIsWall = false;
            return true;
        }
        return false;
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
        Grid.Instance.ShootEnd(this, direction);
    }
}
