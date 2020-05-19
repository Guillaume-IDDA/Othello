using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGrid : MonoBehaviour {

    [SerializeField] int squareID;

    public int SquareID
    {
        get
        {
            return squareID;
        }

        private set
        {
            squareID = value;
        }
    }
}
