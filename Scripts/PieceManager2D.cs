using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager2D : MonoBehaviour {

    public static PieceManager2D Instance { get; private set; }

    [SerializeField] GameObject startPieces;

    private void Awake()
    {
        Instance = this;
    }

    List<Piece2D> piecesList = new List<Piece2D>();

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            piecesList.Add(transform.GetChild(i).GetComponent<Piece2D>());
        }
    }

    // Update is called once per frame
    public void AddPiece(Piece2D _piece)
    {
        piecesList.Add(_piece);
    }

    public void UpdateColorOfPieces()
    {
        foreach (Piece2D piece in piecesList)
        {
            piece.UpdateColor();
        }
    }
}
