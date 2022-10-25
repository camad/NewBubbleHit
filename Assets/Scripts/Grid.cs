using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid Instance { get; private set; }
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private Bubble bubblePrefab;
    [SerializeField]
    private List<Color> Colors = new List<Color>();

    private List<Bubble> bubbles = new List<Bubble>();
    private float offsetX = 0.5f;

    void Start()
    {
        Instance = this;
        CreateGrid();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            GenerateNextLine();
    }

    private void CreateGrid()
    {
        for (int y = height; y > 0; y--)
        {
            GenerateLine(y - (0.15f * y) + 2.2f);
        }
    }
    private void GenerateNextLine()
    {
        Vector3 position = Vector3.zero;
        foreach (var item in bubbles)
        {
            position = item.transform.position;
            position.y -= 0.85f;
            item.transform.position = position;
        }
        GenerateLine(6.45f);
    }

    private void GenerateLine(float y)
    {
        for (int x = 0; x < width; x++)
        {
            SpawnNewBubble(new Vector3(x + offsetX - (width / 2) - 0.25f, y, 0) , GetRandomColor());
        }
        if (offsetX == 0.5f)
            offsetX = 0;
        else
            offsetX = 0.5f;
    }

    private Bubble SpawnNewBubble(Vector3 position, Color color)
    {
        Bubble bubble = Instantiate(bubblePrefab.transform, position, Quaternion.identity, transform).GetComponent<Bubble>();
        bubble.SetColor(color);
        bubble.gameObject.SetActive(true);
        bubbles.Add(bubble);
        return bubble;
    }
    //Улучшить проверку размещения, иногда может разместить туда где уже есть пузырь
    public void AddNewBubble(Bubble bubble, Transform trans)
    {
        float contactX = trans.position.x;
        float contactY = trans.position.y;
        float positionX = bubble.transform.position.x;
        float positionY = bubble.transform.position.y;
        float x = trans.position.x - 0.5f;
        float y = trans.position.y - 0.85f;
        if (contactX < positionX)
            x = trans.position.x + 0.5f;
        if (contactY < positionY)
            y = trans.position.y + 0.85f;
        print("new Vector3(x, y, 0) " + new Vector3(x, y, 0));
        Bubble newBubble =  SpawnNewBubble(new Vector3(x, y, 0), bubble.Color);
        Destroy(bubble.gameObject);
        ShootService.Instance.SpawnNextBubble();
        CheckDestroy(newBubble);
    }

    public Color GetRandomColor()
    {
        return Colors[Random.Range(0, Colors.Count)];
    }

    //Сделать так чтобы если пузыри не имеют связи они падали
    private void CheckDestroy(Bubble bubble)
    {
        List<Transform> opportunes = new List<Transform>();
        opportunes.Add(bubble.transform);
        for (int i = 0; i < opportunes.Count; i++)
        {
            Transform item = opportunes[i];
            radius = item.GetComponent<CircleCollider2D>().radius;
            Collider2D[] collisions = Physics2D.OverlapCircleAll(item.position, item.GetComponent<CircleCollider2D>().radius + 0.025f);
            foreach (var item1 in collisions)
            {
                if (item1.tag != "bubble")
                    continue;
                if (item1.GetComponent<Bubble>().Color == bubble.Color && !opportunes.Contains(item1.transform))
                {
                    opportunes.Add(item1.transform);
                }
            }
        }
        if(opportunes.Count > 2)
            for (int i = 0; i < opportunes.Count; i++)
            {
                bubbles.Remove(opportunes[i].GetComponent<Bubble>());
                Destroy(opportunes[i].gameObject);
            }


    }
    private float radius = 1;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, radius);
    }

}
