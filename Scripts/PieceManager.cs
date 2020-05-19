using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour {

    public static PieceManager Instance { get; private set; }

    [SerializeField] GameObject startPieces;

    private void Awake()
    {
        Instance = this;
    }

    List<Piece> piecesList = new List<Piece>();

	// Use this for initialization
	void Start ()
    {
        for (int i = 0; i < 4; i++)
        {
            piecesList.Add(transform.GetChild(i).GetComponent<Piece>());
        }
	}
	
	// Update is called once per frame
	public void AddPiece (Piece _piece)
    {
        piecesList.Add(_piece);
    }

    public void UpdateColorOfPieces()
    {
        foreach (Piece piece in piecesList)
        {
            piece.UpdateColor();
        }
    }
}
