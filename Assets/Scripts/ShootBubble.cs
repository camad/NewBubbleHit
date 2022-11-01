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
    List<Vector3> directions = new List<Vector3>();
    private int curentDirection;
    private bool isMove;
    private Vector3 targetPosition;
    void Start()
    {
    }
    public void Shoot(List<Vector3> directions)
    {
        this.directions = new List<Vector3>(directions);
        targetPosition = this.directions[this.directions.Count - 1];
        isMove = true;
        curentDirection = 0;
    }

    void FixedUpdate()
    {
        if (!isMove)
            return;
        Vector3 direction = directions[curentDirection];
        if (curentDirection == directions.Count - 2)
            direction = targetPosition;
        transform.position = Vector3.MoveTowards(transform.position, direction, 0.7f);
        if (Vector2.Distance(transform.position, direction) <= 0.1f)
            curentDirection++;
        if (curentDirection == directions.Count-1)
        {
            transform.position = targetPosition;
            isMove = false;
            ShootEnd();
        }
    }
    public void ShootEnd()
    {
        Grid.Instance.ShootEnd(this, transform.position);
    }
}
