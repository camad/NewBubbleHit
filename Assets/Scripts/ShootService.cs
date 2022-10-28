using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootService : MonoBehaviour
{
    public static ShootService Instance { get; private set; }
    [SerializeField]
    private Transform arrow;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Transform bubblePrefab;
    [SerializeField]
    private ShootBubble currentBubble;
    [SerializeField]
    private SpriteRenderer nextBubble;
    private Color nextBubbleColor;
    private async void Start()
    {
        Instance = this;
        await Task.Delay(200);
        NextBubleColor();
        SpawnNextBubble();
    }
    void Update()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPosition;
        //RayCast1(currentBubble.transform.position, direction);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        if (Input.GetMouseButtonUp(0))
            RayCast(currentBubble.transform.position, direction);
    }
    private void RayCast(Vector3 startPosition, Vector3 direction, bool wallCheck = false)
    {
        LayerMask layers = 1 << 6 | 1 << 8;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, 30f, layers);
        if (hit.collider.tag == "bubble")
        {
            direction = Grid.Instance.GetBubbleNewPosition1(hit);
            currentBubble.Shoot(direction, startPosition);
        }
        if (hit.collider.tag == "wall")
        {
            currentBubble.Shoot(hit.point, startPosition, true);
            return;
        }
    }

    private void RayCast1(Vector3 startPosition, Vector3 direction, bool wallCheck = false)
    {
        LayerMask layers = 1 << 6 | 1 << 8;
        //if (wallCheck)
        //    layers = 1 << 6;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, 30f, layers);
        Debug.DrawRay(startPosition, direction, Color.red, 0.2f);
        if (hit.collider.tag == "wall")
        {
            RayCast1(new Vector3(hit.point.x + hit.point.x * -0.01f, hit.point.y, 0), new Vector3(direction.x * -1, direction.y, 0), true);
            return;
        }
    }


    public void SpawnNextBubble()
    {
        if (GameEnd.Instance.GetEndGame)
            return;
        currentBubble = Instantiate(bubblePrefab, startPosition, Quaternion.identity).GetComponent<ShootBubble>();
        currentBubble.SetColor(nextBubbleColor);
        currentBubble.gameObject.SetActive(true);
        NextBubleColor();
    }
    private void NextBubleColor()
    {
        nextBubbleColor = Grid.Instance.GetRandomColor();
        nextBubble.color = nextBubbleColor;
    }
}
