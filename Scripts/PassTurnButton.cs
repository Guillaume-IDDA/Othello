using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTurnButton : MonoBehaviour
{
    public void NextTurn()
    {
        GameManager.Instance.turnNumber++;
        GameManager.Instance.turnPassed++;
        UIinGame.Instance.UpdateDisplay();

        if (GameManager.Instance.EndOfGameTest())
        {
            GameManager.Instance.isEndGame = true;
            UIinGame.Instance.EndGameDisplay();
        }
    }
}
