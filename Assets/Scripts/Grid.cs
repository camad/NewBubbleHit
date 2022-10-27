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
    private float offsetX = 0.55f;
    private float offsetY = 0.95f;
    private float offsetXCurrent;

    void Start()
    {
        Instance = this;
        offsetXCurrent = offsetX;
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
            GenerateLine(y - (0.15f * y) + 1.5f + y * 0.1f);
        }
    }
    private void GenerateNextLine()
    {
        Vector3 position = Vector3.zero;
        foreach (var item in bubbles)
        {
            position = item.transform.position;
            position.y -= offsetY;
            item.transform.position = position;
        }
        GenerateLine(6.3f);
    }

    private void GenerateLine(float y)
    {
        for (int x = 0; x < width; x++)
        {
            SpawnBubble(new Vector3(x + offsetXCurrent - (width / 2) - 0.45f + x * 0.1f, y, 0) , GetRandomColor());
        }
        if (offsetX == offsetXCurrent)
            offsetXCurrent = 0;
        else
            offsetXCurrent = offsetX;
    }

    private Bubble SpawnBubble(Vector3 position, Color color)
    {
        Bubble bubble = Instantiate(bubblePrefab.transform, position, Quaternion.identity, transform).GetComponent<Bubble>();
        bubble.SetColor(color);
        bubble.gameObject.SetActive(true);
        bubbles.Add(bubble);
        return bubble;
    }
    public void ShootEnd(Bubble bubble, Vector3 position)
    {
        Bubble newBubble = SpawnBubble(position, bubble.Color);
        Destroy(bubble.gameObject);
        ShootService.Instance.SpawnNextBubble();
        CheckDestroy(newBubble);
    }
    //Улучшить проверку размещения, иногда может разместить туда где уже есть пузырь
    public Vector3 GetBubbleNewPosition(Bubble bubble, Transform trans)
    {
        float contactX = trans.position.x;
        float contactY = trans.position.y;
        float positionX = bubble.transform.position.x;
        float positionY = bubble.transform.position.y;
        float x = trans.position.x - offsetX;
        float y = trans.position.y - offsetY;
        if (contactX < positionX)
            x = trans.position.x + offsetX;
        if (contactY < positionY)
            y = trans.position.y + offsetY;
        return new Vector3(x, y, 0);

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
            Collider2D[] collisions = Physics2D.OverlapCircleAll(item.position, 0.7f);
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
}
