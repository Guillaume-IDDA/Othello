using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIinGame : MonoBehaviour
{

    public static UIinGame Instance { get; private set; }

    [SerializeField] Text scorePlayer1_Text;
    [SerializeField] Text scorePlayer2_Text;
    [SerializeField] Text TurnPlayer_Text;
    [SerializeField] Text endGame_Text;

    // Use this for initialization
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        scorePlayer1_Text.text = GameManager.Instance.scorePlayer1.ToString() + " pts";
        scorePlayer2_Text.text = GameManager.Instance.scorePlayer2.ToString() + " pts";


        int turnPlayer = GameManager.Instance.turnNumber % 2;
        switch (turnPlayer)
        {
            case 0:
                TurnPlayer_Text.text = "Turn of Player 2!";
                TurnPlayer_Text.color = Color.white;
                break;

            case 1:
                TurnPlayer_Text.text = "Turn of Player 1!";
                TurnPlayer_Text.color = Color.black;
                break;

            default:
                break;
        }

    }

    public void EndGameDisplay()
    {
        if (GameManager.Instance.scorePlayer1 > GameManager.Instance.scorePlayer2)
        {
            endGame_Text.gameObject.SetActive(true);
            endGame_Text.text = "Player 1 Win !";
        }
        else if(GameManager.Instance.scorePlayer1 < GameManager.Instance.scorePlayer2)
        {
            endGame_Text.gameObject.SetActive(true);
            endGame_Text.text = "Player 2 Win !";
        }
        else
        {
            endGame_Text.gameObject.SetActive(true);
            endGame_Text.text = "Draw !";
        }


    }
}
