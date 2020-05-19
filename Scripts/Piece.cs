using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    [SerializeField] int pieceID =0;

    // Use this for initialization
    void Start()
    {
        UpdateColor();
    }
	

    public void SetPieceID(int _squareGridID)
    {
        pieceID = _squareGridID;
    }
	// Update is called once per frame
	public void UpdateColor ()
    {

        switch (GameManager.Instance.RootTree.board[pieceID / 8, (pieceID % 8)])
        {
            case Data.STATE.EMPTY:
                break;
            case Data.STATE.WHITE:
                GetComponent<MeshRenderer>().material.color = Color.white;
                break;
            case Data.STATE.BLACK:
                GetComponent<MeshRenderer>().material.color = Color.black;
                break;
            default:
                break;
        }
    }
}
