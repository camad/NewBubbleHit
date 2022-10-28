using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    public static GameEnd Instance { get; private set; }
    [SerializeField]
    private Transform gameEndScreenPanel;
    [SerializeField]
    private TextMeshProUGUI winText;
    [SerializeField]
    private TextMeshProUGUI pointsText;

    private bool endGame;
    public bool GetEndGame { get { return endGame; }}

    private void Start()
    {
        Instance = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "bubble")
            GameEndScreenLoad(1);
    }
    //private void FixedUpdate()
    //{
    //    if(Physics2D.OverlapBox(new Vector2(transform.position.x-5, transform.position.y), GetComponent<BoxCollider2D>().size*5, 0, 1 << 6))
    //    {
    //        GameEndScreenLoad(1);
    //    }
    //}

    public void GameEndScreenLoad(int type)
    {
        endGame = true;
        string winText = "Победа!";
        if (type == 1)
            winText = "Проигрыш";

        this.winText.text = winText;
        pointsText.text = Grid.Instance.GetPoints.ToString();
        gameEndScreenPanel.gameObject.SetActive(true);
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }
}
