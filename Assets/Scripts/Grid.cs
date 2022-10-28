using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

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
    [SerializeField]
    private TextMeshProUGUI pointsText;
    [SerializeField]
    private TextMeshProUGUI missesText;
    private List<Bubble> bubbles = new List<Bubble>();
    private float offsetX = 0.55f;
    private float offsetY = 0.95f;
    private float offsetXCurrent;
    private int points;
    private int misses;
    private int missesMax = 3;

    public int GetPoints { get { return points; } }
    async void Start()
    {
        Instance = this;
        await Task.Delay(500);
        offsetXCurrent = offsetX;
        ChangeMisses(missesMax);
        CreateGrid();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            GenerateNextLine();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            SceneManager.LoadScene(0);
    }

    private async void CreateGrid()
    {
        for (int y = height; y > 0; y--)
        {
            GenerateLine(y - (0.15f * y) + 4.34f + y * 0.1f);
            await Task.Delay(width * 50);
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
        GenerateLine(9.09f);
    }

    private async void GenerateLine(float y)
    {
        for (int x = 0; x < width; x++)
        {
            SpawnBubble(new Vector3(x + offsetXCurrent - (width / 2) - 0.13f + x * 0.1f, y, 0) , GetRandomColor());
            await Task.Delay(20);
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

        float borferOffset = 0;
        if (x > 4f)
            borferOffset = -1.1f;
        if (x < -4.5f)
            borferOffset = 1.1f;
        x += borferOffset;
        return new Vector3(x, y, 0);

    }

    public Color GetRandomColor()
    {
        return Colors[Random.Range(0, Colors.Count)];
    }

    //Сделать так чтобы если пузыри не имеют связи они падали
    private async void CheckDestroy(Bubble bubble)
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
        {
            for (int i = 0; i < opportunes.Count; i++)
            {
                await Task.Delay(100);
                bubbles.Remove(opportunes[i].GetComponent<Bubble>());
                Destroy(opportunes[i].gameObject);
                AddPoints(1);
            }
        }
        else
        {
            ChangeMisses(-1);
            if (misses < 1)
            {
                GenerateNextLine();
                ChangeMisses(missesMax);
            }
            return;
        }
        CheckNotConnection();
    }
    private async void CheckNotConnection()
    {
        List<Transform> opportunes = new List<Transform>();
        opportunes.Add(bubbles[0].transform);
        for (int i = 0; i < opportunes.Count; i++)
        {
            Transform item = opportunes[i].transform;
            Collider2D[] collisions = Physics2D.OverlapCircleAll(item.position, 0.7f);
            foreach (var item1 in collisions)
            {
                if (item1.tag != "bubble")
                    continue;
                if (!opportunes.Contains(item1.transform))
                {
                    opportunes.Add(item1.transform);
                }
            }
        }
        List<Transform> destroy = new List<Transform>();
        for (int i = 0; i < bubbles.Count; i++)
        {
            Transform item = bubbles[i].transform;
            if (!opportunes.Contains(item.transform))
                destroy.Add(item.transform);
        }
        if(bubbles.Count == 1)
        {
            Destroy(bubbles[0]);
            bubbles.Clear();
        }
        bool checkFirstBubble = false;
        for (int i = 0; i < bubbles.Count; i++)
        {
            if(bubbles[i].transform.localPosition.y > -1)
            {
                checkFirstBubble = true;
                break;
            } 
        }
        if (!checkFirstBubble)
        {
            destroy.Clear();
            for (int i = 0; i < bubbles.Count; i++)
                destroy.Add(bubbles[i].transform);
        }

        for (int i = 0; i < destroy.Count; i++)
        {
            Transform item = destroy[i].transform;
            await Task.Delay(100);
            bubbles.Remove(item.transform.GetComponent<Bubble>());
            Destroy(item.gameObject);
            AddPoints(1);
        }
        if (!checkFirstBubble)
        {
            GameEnd.Instance.GameEndScreenLoad(0);
        }
    }

    private void ChangeMisses(int amount)
    {
        misses += amount;
        missesText.text = misses.ToString();
    }
    private void AddPoints(int amount)
    {
        points += amount;
        pointsText.text = points.ToString();
    }
}
