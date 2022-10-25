using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private void Start()
    {
        Instance = this;
        SpawnNextBubble();
    }
    void Update()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle-90);

        if (Input.GetMouseButtonUp(0))
        {
            currentBubble.Shoot(direction);
        }
    }

    public void SpawnNextBubble()
    {
        currentBubble = Instantiate(bubblePrefab, startPosition, Quaternion.identity).GetComponent<ShootBubble>();
        currentBubble.SetColor(Grid.Instance.GetRandomColor());
        currentBubble.gameObject.SetActive(true);
    }
}
