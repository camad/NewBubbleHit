using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ShootService : MonoBehaviour
{
    public static ShootService Instance { get; private set; }
    [SerializeField]
    private Transform arrow;
    [SerializeField]
    private LineRenderer line;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Transform bubblePrefab;
    [SerializeField]
    private Transform bubbleShadow;
    [SerializeField]
    private ShootBubble currentBubble;
    [SerializeField]
    private Image nextBubble;
    private Color nextBubbleColor;
    private List<Vector3> linePositions = new List<Vector3>();
    private bool shootProcess;
    private async void Start()
    {
        Instance = this;
        await Task.Delay(200);
        NextBubleColor();
        SpawnNextBubble();
    }
    void Update()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = targetPosition - startPosition;
        linePositions.Clear();
        linePositions.Add(startPosition);
        if (!shootProcess)
            RayCast(startPosition, direction);
        line.positionCount = linePositions.Count-1;
        line.SetPositions(linePositions.ToArray());
        if (Input.GetMouseButtonUp(0))
        {
            if(Input.GetKey(KeyCode.Z))
            {
                Collider2D bubble = Physics2D.OverlapCircle(targetPosition, 0.2f);
                if(bubble != null)
                {
                    currentBubble.SetColor(bubble.GetComponent<Bubble>().Color);
                }
                return;
            }
            shootProcess = true;
            bubbleShadow.transform.position = new Vector3(0, -1000, 0);
            currentBubble.Shoot(linePositions);
        }
    }
    private void RayCast(Vector3 startPosition, Vector3 direction, int countRays = 1)
    {
        LayerMask layers = 1 << 6 | 1 << 8;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, 30f, layers);
        Debug.DrawRay(startPosition, direction, Color.red, 0.2f);
        if (hit.collider.tag == "bubble")
        {
            linePositions.Add(hit.point);
            Vector3 targetPosition = Grid.Instance.GetBubbleNewPosition(hit);
            linePositions.Add(targetPosition);
            bubbleShadow.transform.position = targetPosition;
        }
        if (hit.collider.tag == "wall")
        {
            linePositions.Add(hit.point);
            if (countRays >= 10)
                return;
            RayCast(new Vector3(hit.point.x + hit.point.x * -0.01f, hit.point.y, 0), new Vector3(direction.x * -1, direction.y, 0), countRays+1);
        }
    }

    public void SpawnNextBubble()
    {
        shootProcess = false;
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
