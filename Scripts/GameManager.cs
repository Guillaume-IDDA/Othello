using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject piece;
    [SerializeField] GameObject piece2D;

    Data rootTree = new Data();
    public int turnNumber = 1;
    public int turnPassed = 0; // nombre de tours passé

    public int scorePlayer1 = 2;
    public int scorePlayer2 = 2;
    public int nbSimulation = 0;

    public bool isEndGame = false;

    public enum Opponent { PLAYER, IA };
    public enum IALevel { EASY, MEDIUM, HARD};

    public Opponent opponentType;
    public IALevel levelIA;

    public Data RootTree
    {
        get
        {
            return rootTree;
        }

        private set
        {
            rootTree = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {

        if (SceneController.Instance.idButton == 0)
        {
            opponentType = Opponent.PLAYER;

        }
        else if (SceneController.Instance.idButton == 1)
        {
            opponentType = Opponent.IA;
            levelIA = IALevel.EASY;
        }
        else if (SceneController.Instance.idButton == 2)
        {
            opponentType = Opponent.IA;
            levelIA = IALevel.MEDIUM;
        }
        else if (SceneController.Instance.idButton == 3)
        {
            opponentType = Opponent.IA;
            levelIA = IALevel.HARD;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (opponentType == Opponent.PLAYER)
        {
            ClickPlayer();
        }
        else if(opponentType == Opponent.IA)
        {
            if (turnNumber % 2 == 1)
            {
                ClickPlayer();
            }
            else
            {
                AI_Play();
            }
        }
    }

    void UpdateScoresPlayers(int _PlayerID, int _NumberOfPiecesEarn)
    {
        switch (_PlayerID)
        {
            case 1:
                GameManager.Instance.scorePlayer1 += _NumberOfPiecesEarn + 1;
                GameManager.Instance.scorePlayer2 -= _NumberOfPiecesEarn;
                break;

            case 2:
                scorePlayer1 -= _NumberOfPiecesEarn;
                scorePlayer2 += _NumberOfPiecesEarn + 1;
                break;

            default:
                break;
        }
    }

    public bool EndOfGameTest()
    {
        if (
            scorePlayer1 + scorePlayer2 == 64
            || scorePlayer1 == 0
            || scorePlayer2 == 0
            || turnPassed == 2

          )
        {
            return true;
        }


        return false;

    }

    void InstantiatePiece(int _squareGridID)
    {
        Piece Actualpiece = PoolManager.Instance.Get(piece).GetComponent<Piece>();
        Actualpiece.transform.position = new Vector3((_squareGridID % 8) + 0.5f, 0.5f, (7 - _squareGridID / 8) + 0.5f);
        Actualpiece.SetPieceID(_squareGridID); // donne l'id de la grille à la pièce
        PieceManager.Instance.AddPiece(Actualpiece);// ajoute la nouvelle pièce à la liste des pièces sur la table

        Piece2D Actualpiece2D = PoolManager.Instance.Get(piece2D).GetComponent<Piece2D>();
        Actualpiece2D.transform.position = new Vector3((_squareGridID % 8) + 0.5f, 0.6f, (7 - _squareGridID / 8) + 20.5f);
        Actualpiece2D.SetPieceID(_squareGridID); // donne l'id de la grille à la pièce
        PieceManager2D.Instance.AddPiece(Actualpiece2D);// ajoute la nouvelle pièce à la liste des pièces sur la table
    }


    void AI_Play()
    {
        rootTree.children.Clear();
        rootTree.isOpponent = false;
        rootTree.nbOfPieceEarn = 0;

        nbSimulation = 0;

        int depth = 0;

        if(levelIA == IALevel.EASY)
        {
            depth = 0;
        }
        else if(levelIA == IALevel.MEDIUM)
        {
            depth = 2;
        }
        else
        {
            depth = 3;
        }

        Simulate(ref rootTree, turnNumber, depth);
        int value = rootTree.minmax(); // renvoi la valeur la + interessante pour l'ia

        if(rootTree.children.Count > 0)
        {
            for (int i = 0; i < rootTree.children.Count; i++)
            {
                if (rootTree.children[i].value == value) // s'arrête au noeud de l'arbre où la valeur du noeud est égal à la valeur du minmax minmax
                {
                    rootTree = rootTree.children[i]; // descend d'un cran dans l'arbre au choix effectué par l'ia

                    InstantiatePiece(rootTree.squareGridChoosenByAI); // place le pion à la case choisi par l'ia

                    UpdateScoresPlayers(2, rootTree.nbOfPieceEarn);

                    turnPassed = 0; // remet le contour des tours passé à 0;

                    turnNumber++;

                    PieceManager.Instance.UpdateColorOfPieces();// Update la couleur de toute les pièces
                    PieceManager2D.Instance.UpdateColorOfPieces();// Update la couleur de tout les sprites de pièces
                    UIinGame.Instance.UpdateDisplay();

                    if (EndOfGameTest())
                    {
                        isEndGame = true;
                        UIinGame.Instance.EndGameDisplay();
                    }
                    return;
                }
            }
        }
        else
        {
            turnNumber++;
            turnPassed++;
            UIinGame.Instance.UpdateDisplay();

            if (EndOfGameTest())
            {
                isEndGame = true;
                UIinGame.Instance.EndGameDisplay();
            }
        }
    }

    bool TestValidityOfPosition(int _i, int _j)
    {
        ////////// cas pièce noire à gauche //////////
        if ((_j - 2 > -1) && (rootTree.board[_i, _j - 1] == Data.STATE.BLACK))
        {
            for (int i = _j - 1; i > -1; i--)
            {
                if (rootTree.board[_i, i] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[_i, i] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire à droite //////////
        if ((_j + 2 < 8) && (rootTree.board[_i, _j + 1] == Data.STATE.BLACK))
        {
            for (int i = _j + 1; i < 8; i++)
            {
                if (rootTree.board[_i, i] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[_i, i] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire en haut //////////
        if ((_i - 2 > -1) && (rootTree.board[_i - 1, _j] == Data.STATE.BLACK))
        {

            for (int i = _i - 1; i > -1; i--)
            {
                if (rootTree.board[i, _j] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[i, _j] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire en bas //////////
        if ((_i + 2 < 8) && (rootTree.board[_i + 1, _j] == Data.STATE.BLACK))
        {
            for (int i = _i + 1; i < 8; i++)
            {
                if (rootTree.board[i, _j] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[i, _j] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire en haut à gauche //////////
        if ((_i - 2 > -1) && (_j - 2 > -1) && (rootTree.board[_i - 1, _j - 1] == Data.STATE.BLACK))
        {
            int minValue = Mathf.Min(_i, _j);

            for (int i = minValue - 1; i > -1; i--)
            {
                if (rootTree.board[_i - (minValue - i), _j - (minValue - i)] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[_i - (minValue - i), _j - (minValue - i)] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire en haut à droite //////////
        if ((_i - 2 > -1) && (_j + 2 < 8) && (rootTree.board[_i - 1, _j + 1] == Data.STATE.BLACK))
        {
            int minValue = Mathf.Min(_i, 7 - _j);

            for (int i = minValue - 1; i > -1; i--)
            {
                if (rootTree.board[_i - (minValue - i), _j + (minValue - i)] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[_i - (minValue - i), _j + (minValue - i)] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire en bas à gauche //////////
        if ((_i + 2 < 8) && (_j - 2 > -1) && (rootTree.board[_i + 1, _j - 1] == Data.STATE.BLACK))
        {
            int minValue = Mathf.Min(7 - _i, _j);

            for (int i = minValue - 1; i > -1; i--)
            {
                if (rootTree.board[_i + (minValue - i), _j - (minValue - i)] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[_i + (minValue - i), _j - (minValue - i)] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        ////////// cas pièce noire en bas à droite //////////
        if ((_i + 2 < 8) && (_j + 2 < 8) && (rootTree.board[_i + 1, _j + 1] == Data.STATE.BLACK))
        {
            int minValue = Mathf.Min(7 - _i, 7 - _j);

            for (int i = minValue - 1; i > -1; i--)
            {
                if (rootTree.board[_i + (minValue - i), _j + (minValue - i)] == Data.STATE.WHITE)
                {
                    return true;
                }
                if (rootTree.board[_i + (minValue - i), _j + (minValue - i)] == Data.STATE.EMPTY)
                {
                    break;
                }
            }
        }

        return false;
    }

    // met à jour la grille du noeud et évalue le noeud (nb de pièces gagné = nbOfPiecesTotal)
    void UpdateNodeDataGrid(ref Data _data, int _i, int _j, int _turnNumber)
    {
        Data.STATE actualState = Data.STATE.EMPTY;
        Data.STATE oppositeState = Data.STATE.EMPTY;

        if (_data.board[_i, _j] == Data.STATE.EMPTY)
        {
            // *************************************************
            // ********** tour impair (couleur noire) **********
            // ********** tour pair(couleur blanche) ***********
            // *************************************************
            if (_turnNumber % 2 == 1)
            {
                actualState = Data.STATE.WHITE;// état du pion à tester
                oppositeState = Data.STATE.BLACK; // état du pion à jouer
                _data.isOpponent = true;
            }
            else
            {
                actualState = Data.STATE.BLACK; // état du pion à tester
                oppositeState = Data.STATE.WHITE; // état du pion à jouer
                _data.isOpponent = false;
            }

            int nbOfPiecesTotal = 0;

            ////////// cas pièce à gauche //////////
            if ((_j - 2 > -1) && (_data.board[_i, _j - 1] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                for (int i = _j - 1; i > -1; i--)
                {
                    if (_data.board[_i, i] == oppositeState)
                    {
                        nbOfPieces = (_j - 1) - i;
                        break;
                    }
                    if (_data.board[_i, i] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                // met à jour le tableau de données
                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i, _j - i] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            ////////// cas pièce à droite //////////
            if ((_j + 2 < 8) && (_data.board[_i, _j + 1] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                for (int i = _j + 1; i < 8; i++)
                {
                    if (_data.board[_i, i] == oppositeState)
                    {
                        nbOfPieces = i - (_j + 1);
                        break;
                    }
                    if (_data.board[_i, i] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i, _j + i] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            ////////// cas pièce en haut //////////
            if ((_i - 2 > -1) && (_data.board[_i - 1, _j] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                for (int i = _i - 1; i > -1; i--)
                {
                    if (_data.board[i, _j] == oppositeState)
                    {
                        nbOfPieces = (_i - 1) - i;
                        break;
                    }
                    if (_data.board[i, _j] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i - i, _j] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            ////////// cas pièce en bas //////////
            if ((_i + 2 < 8) && (_data.board[_i + 1, _j] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                for (int i = _i + 1; i < 8; i++)
                {
                    if (_data.board[i, _j] == oppositeState)
                    {
                        nbOfPieces = i - (_i + 1);
                        break;
                    }
                    if (_data.board[i, _j] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i + i, _j] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }


            }

            ////////// cas pièce en haut à gauche //////////
            if ((_i - 2 > -1) && (_j - 2 > -1) && (_data.board[_i - 1, _j - 1] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                int minValue = Mathf.Min(_i, _j);

                for (int i = minValue - 1; i > -1; i--)
                {
                    if (_data.board[_i - (minValue - i), _j - (minValue - i)] == oppositeState)
                    {
                        nbOfPieces = (minValue - 1) - i;
                        break;
                    }
                    if (_data.board[_i - (minValue - i), _j - (minValue - i)] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i - i, _j - i] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            ////////// cas pièce en haut à droite //////////
            if ((_i - 2 > -1) && (_j + 2 < 8) && (_data.board[_i - 1, _j + 1] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                int minValue = Mathf.Min(_i, 7 - _j);

                for (int i = minValue - 1; i > -1; i--)
                {
                    if (_data.board[_i - (minValue - i), _j + (minValue - i)] == oppositeState)
                    {
                        nbOfPieces = (minValue - 1) - i;
                        break;
                    }
                    if (_data.board[_i - (minValue - i), _j + (minValue - i)] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i - i, _j + i] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            ////////// cas pièce en bas à gauche //////////
            if ((_i + 2 < 8) && (_j - 2 > -1) && (_data.board[_i + 1, _j - 1] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                int minValue = Mathf.Min(7 - _i, _j);

                for (int i = minValue - 1; i > -1; i--)
                {
                    if (_data.board[_i + (minValue - i), _j - (minValue - i)] == oppositeState)
                    {
                        nbOfPieces = (minValue - 1) - i;
                        break;
                    }
                    if (_data.board[_i + (minValue - i), _j - (minValue - i)] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i + i, _j - i] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            ////////// cas pièce blanche en bas à droite //////////
            if ((_i + 2 < 8) && (_j + 2 < 8) && (_data.board[_i + 1, _j + 1] == actualState))
            {

                int nbOfPieces = 0;

                // compte le nombre de pièces à changer
                int minValue = Mathf.Min(7 - _i, 7 - _j);

                for (int i = minValue - 1; i > -1; i--)
                {
                    if (_data.board[_i + (minValue - i), _j + (minValue - i)] == oppositeState)
                    {
                        nbOfPieces = (minValue - 1) - i;
                        break;
                    }
                    if (_data.board[_i + (minValue - i), _j + (minValue - i)] == Data.STATE.EMPTY)
                    {
                        break;
                    }
                }

                if (nbOfPieces > 0)
                {
                    for (int i = 0; i < nbOfPieces + 1; i++)
                    {
                        _data.board[_i + i, _j + i] = oppositeState;
                    }

                    nbOfPiecesTotal += nbOfPieces;
                }
            }

            _data.squareGridChoosenByAI = (_i * 8 + _j);
            _data.nbOfPieceEarn = nbOfPiecesTotal;
        }
    }


    void Simulate(ref Data data, int _turnNumber, int _depth)
    {
        if (EndOfGameTest() == false)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data.board[i, j] == Data.STATE.EMPTY)
                    {
                        // Test si la case actuelle est valide
                        if (TestValidityOfPosition(i, j) == true)
                        {
                            //Copie du tableau de données dans un tableau temporaire (= nouveau noeud)
                            Data tmpData = new Data();
                            Array.Copy(data.board, tmpData.board, 64);

                            //défini l'état du tour du joueur (joueur ou ia)
                            tmpData.isOpponent = !data.isOpponent;

                            if(levelIA != IALevel.EASY)
                            {
                                if (_depth > 0)
                                {
                                    Simulate(ref tmpData, _turnNumber + 1, _depth - 1);
                                }
                            }

                            // change l'état des pièces en fonction du tour (noir pour joueur et blanc pour IA)
                            UpdateNodeDataGrid(ref tmpData, i, j, _turnNumber);
                            data.children.Add(tmpData);
                        }
                    }
                }
            }
        }
    }

    void ClickPlayer()
    {
        Data.STATE actualState = Data.STATE.EMPTY;
        Data.STATE oppositeState = Data.STATE.EMPTY;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                SquareGrid squareGrid = hit.transform.GetComponent<SquareGrid>();
                if (squareGrid != null)
                {
                    int squareGridID = squareGrid.SquareID;
                    //test si case vide
                    if (rootTree.board[squareGridID / 8, (squareGridID % 8)] == Data.STATE.EMPTY)
                    {
                        // *************************************************
                        // ********** tour impair (couleur noire) **********
                        // ********** tour pair(couleur blanche) ***********
                        // *************************************************
                        if (turnNumber % 2 == 1)
                        {
                            actualState = Data.STATE.WHITE;
                            oppositeState = Data.STATE.BLACK;
                        }
                        else
                        {
                            actualState = Data.STATE.BLACK;
                            oppositeState = Data.STATE.WHITE;
                        }
                        bool pieceIsInstantiate = false;
                        int nbOfPiecesTotal = 0;


                        ////////// cas pièce blanche à gauche //////////
                        if (
                            ((squareGridID % 8) - 2 > -1)
                            && (rootTree.board[(squareGridID / 8), (squareGridID % 8) - 1] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            for (int i = (squareGridID % 8) - 1; i > -1; i--)
                            {
                                if (rootTree.board[(squareGridID / 8), i] == oppositeState)
                                {
                                    nbOfPieces = ((squareGridID % 8) - 1) - i;
                                    break;
                                }
                                if (rootTree.board[(squareGridID / 8), i] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            // met à jour le tableau de données
                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8), (squareGridID % 8) - i] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        ////////// cas pièce blanche à droite //////////
                        if (
                            ((squareGridID % 8) + 2 < 8)
                            && (rootTree.board[(squareGridID / 8), (squareGridID % 8) + 1] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            for (int i = (squareGridID % 8) + 1; i < 8; i++)
                            {
                                if (rootTree.board[(squareGridID / 8), i] == oppositeState)
                                {
                                    nbOfPieces = i - ((squareGridID % 8) + 1);
                                    break;
                                }
                                if (rootTree.board[(squareGridID / 8), i] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8), (squareGridID % 8) + i] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        ////////// cas pièce blanche en haut //////////
                        if (
                            ((squareGridID / 8) - 2 > -1)
                            && (rootTree.board[(squareGridID / 8) - 1, (squareGridID % 8)] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            for (int i = (squareGridID / 8) - 1; i > -1; i--)
                            {
                                if (rootTree.board[i, (squareGridID % 8)] == oppositeState)
                                {
                                    nbOfPieces = ((squareGridID / 8) - 1) - i;
                                    break;
                                }
                                if (rootTree.board[i, (squareGridID % 8)] == Data.STATE.EMPTY)
                                {
                                    break;
                                }

                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8) - i, (squareGridID % 8)] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        ////////// cas pièce blanche en bas //////////
                        if (
                            ((squareGridID / 8) + 2 < 8)
                            && (rootTree.board[(squareGridID / 8) + 1, (squareGridID % 8)] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            for (int i = (squareGridID / 8) + 1; i < 8; i++)
                            {
                                if (rootTree.board[i, (squareGridID % 8)] == oppositeState)
                                {
                                    nbOfPieces = i - ((squareGridID / 8) + 1);
                                    break;
                                }
                                if (rootTree.board[i, (squareGridID % 8)] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8) + i, (squareGridID % 8)] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }


                        }

                        ////////// cas pièce blanche en haut à gauche //////////
                        if (
                            ((squareGridID / 8) - 2 > -1) && ((squareGridID % 8) - 2 > -1)
                            && (rootTree.board[(squareGridID / 8) - 1, (squareGridID % 8) - 1] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            int minValue = Mathf.Min((squareGridID / 8), (squareGridID % 8));

                            for (int i = minValue - 1; i > -1; i--)
                            {
                                if (rootTree.board[(squareGridID / 8) - (minValue - i), (squareGridID % 8) - (minValue - i)] == oppositeState)
                                {
                                    nbOfPieces = (minValue - 1) - i;
                                    break;
                                }
                                if (rootTree.board[(squareGridID / 8) - (minValue - i), (squareGridID % 8) - (minValue - i)] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8) - i, (squareGridID % 8) - i] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        ////////// cas pièce blanche en haut à droite //////////
                        if (
                            ((squareGridID / 8) - 2 > -1) && ((squareGridID % 8) + 2 < 8)
                            && (rootTree.board[(squareGridID / 8) - 1, (squareGridID % 8) + 1] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            int minValue = Mathf.Min((squareGridID / 8), 7 - (squareGridID % 8));

                            for (int i = minValue - 1; i > -1; i--)
                            {
                                if (rootTree.board[(squareGridID / 8) - (minValue - i), (squareGridID % 8) + (minValue - i)] == oppositeState)
                                {
                                    nbOfPieces = (minValue - 1) - i;
                                    break;
                                }
                                if (rootTree.board[(squareGridID / 8) - (minValue - i), (squareGridID % 8) + (minValue - i)] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8) - i, (squareGridID % 8) + i] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        ////////// cas pièce blanche en bas à gauche //////////
                        if (
                            ((squareGridID / 8) + 2 < 8) && ((squareGridID % 8) - 2 > -1)
                            && (rootTree.board[(squareGridID / 8) + 1, (squareGridID % 8) - 1] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            int minValue = Mathf.Min(7 - (squareGridID / 8), (squareGridID % 8));

                            for (int i = minValue - 1; i > -1; i--)
                            {
                                if (rootTree.board[(squareGridID / 8) + (minValue - i), (squareGridID % 8) - (minValue - i)] == oppositeState)
                                {
                                    nbOfPieces = (minValue - 1) - i;
                                    break;
                                }
                                if (rootTree.board[(squareGridID / 8) + (minValue - i), (squareGridID % 8) - (minValue - i)] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8) + i, (squareGridID % 8) - i] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        ////////// cas pièce blanche en bas à droite //////////
                        if (
                            ((squareGridID / 8) + 2 < 8) && ((squareGridID % 8) + 2 < 8)
                            && (rootTree.board[(squareGridID / 8) + 1, (squareGridID % 8) + 1] == actualState)
                            )
                        {

                            int nbOfPieces = 0;

                            // compte le nombre de pièces à changer
                            int minValue = Mathf.Min(7 - (squareGridID / 8), 7 - (squareGridID % 8));

                            for (int i = minValue - 1; i > -1; i--)
                            {
                                if (rootTree.board[(squareGridID / 8) + (minValue - i), (squareGridID % 8) + (minValue - i)] == oppositeState)
                                {
                                    nbOfPieces = (minValue - 1) - i;
                                    break;
                                }
                                if (rootTree.board[(squareGridID / 8) + (minValue - i), (squareGridID % 8) + (minValue - i)] == Data.STATE.EMPTY)
                                {
                                    break;
                                }
                            }

                            if (nbOfPieces > 0)
                            {
                                for (int i = 0; i < nbOfPieces + 1; i++)
                                {
                                    rootTree.board[(squareGridID / 8) + i, (squareGridID % 8) + i] = oppositeState;
                                }

                                nbOfPiecesTotal += nbOfPieces;

                                if (pieceIsInstantiate == false && nbOfPieces > 0)
                                {
                                    InstantiatePiece(squareGridID);
                                    pieceIsInstantiate = true;
                                }
                            }
                        }

                        if (nbOfPiecesTotal > 0)
                        {
                            if (turnNumber % 2 == 1)
                            {
                                UpdateScoresPlayers(1, nbOfPiecesTotal);
                            }
                            else
                            {
                                UpdateScoresPlayers(2, nbOfPiecesTotal);
                            }

                            turnPassed = 0; // remet le contour des tours passé à 0;
                            turnNumber++;
                        }
                        PieceManager.Instance.UpdateColorOfPieces(); // Update la couleur de toute les pièces
                        PieceManager2D.Instance.UpdateColorOfPieces();// Update la couleur de tout les sprites de pièces
                        UIinGame.Instance.UpdateDisplay();

                        if (EndOfGameTest())
                        {
                            isEndGame = true;
                            UIinGame.Instance.EndGameDisplay();
                        }
                    }
                }
            }
        }
    }
}
