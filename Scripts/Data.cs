using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{

    public enum STATE { EMPTY, WHITE, BLACK };

    public int nbOfPieceEarn = 0;

    public int value = 0;

    public int squareGridChoosenByAI;

    public STATE IA = STATE.WHITE;

    public bool isOpponent = false; // definie si IA ou Joueur


    public List<Data> children = new List<Data>(); // Noeuds Enfants

    // grille
    public STATE[,] board = {   { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.WHITE, STATE.BLACK, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.BLACK, STATE.WHITE, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY },
                                { STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY, STATE.EMPTY }
                            };


    public int minmax()
    {
        if (children.Count == 0) // Dernier noeud
        {
            value = evaluate();
            return value;
        }
        else if (isOpponent) // tour adverse
        {
            int minValue = 100000;
            for (int i = 0; i < children.Count; i++) // détermine la valeur min et max pour chaque noeud de la branche
            {
                int tmpValue = children[i].minmax(); // effectue min max pour tous les enfants afin de déterminer la valeur minimale pour le cas opposant
                if (tmpValue <= minValue)
                {
                    minValue = tmpValue;
                }
                else
                {
                    if(i > 0 && i < children.Count-1)
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
            value = minValue;
            return minValue;
        }
        else
        {
            int maxValue = -100000;
            for (int j = 0; j < children.Count; j++)
            {
                int tmpValue = children[j].minmax(); // effectue min max pour tous les enfants afin de déterminer la valeur maximale pour le cas IA
                if (tmpValue >= maxValue)
                {
                    maxValue = tmpValue;
                }
                else
                {
                    if (j > 0 && j < children.Count - 1)
                    {
                        children.RemoveAt(j);
                        j--;
                    }
                }
            }
            value = maxValue;
            return maxValue;
        }
    }

    int evaluate()
    {
        if(squareGridChoosenByAI /8 == 0 && GameManager.Instance.levelIA != GameManager.IALevel.EASY)
        {
            if (GameManager.Instance.levelIA == GameManager.IALevel.HARD && (squareGridChoosenByAI % 8 == 0 || squareGridChoosenByAI % 8 == 7))
            {
                return nbOfPieceEarn + 200;
            }
            else
            {
                return nbOfPieceEarn + 100 ;
            }
        }
        else if (squareGridChoosenByAI / 8 == 7 && GameManager.Instance.levelIA != GameManager.IALevel.EASY)
        {

            if (GameManager.Instance.levelIA == GameManager.IALevel.HARD && (squareGridChoosenByAI % 8 == 0 || squareGridChoosenByAI % 8 == 7))
            {
                return nbOfPieceEarn + 200;
            }
            else
            {
                return nbOfPieceEarn + 100;
            }
        }
        else
        {
            return nbOfPieceEarn;
        }
    }
}




