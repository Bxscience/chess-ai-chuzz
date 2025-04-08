using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardManager : MonoBehaviour{

    [SerializeField]
    private GameObject PieceParent, WPawn, WKnight, WBishop, WRook, WQueen, WKing, BPawn, BKnight, BBishop, BRook, BQueen, BKing, SelectedSquareIndicator, PossibleMovesIndicator, AttackedMovesIndicator;
    
    [HideInInspector]
    private List<GameObject> Pieces;
    
    [HideInInspector]
    public Board Chessboard;

    [HideInInspector]
    public GameObject SelectedPiece;

    // Holds all gameobjects used to visualize a move
    [HideInInspector]
    public List<GameObject> SelectedPieceVisuals;

    // All moves in the position, Attacks for the currently selected piece
    [HideInInspector]
    public int[] Attacks, SelectedPieceAttacks;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1

    void Start(){
        Pieces = new List<GameObject>();
        Chessboard = new Board("r3k2r/8/8/8/8/8/8/8 b kq - 0 1");
        AttackTables.InitAttackTables();
        Attacks = MoveGeneration.InitMoves(Chessboard, Chessboard.PlayerTurn);
        SelectedPieceAttacks = null;
        PlacePieces();
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            int move = HandleMoveInputs();
            if (move != 0) MakeMove(move);
        }

        if (Input.GetKeyDown(KeyCode.N)){
            Chessboard.PlayerTurn = Helper.GetOpponent(Chessboard.PlayerTurn);
        }
    }

    // TODO: Fix raycasting or find better way to get the piece that is being captured
    private void MakeMove(int move){
        int src = Move.GetSrcSquare(move), dest = Move.GetDestSquare(move);
        Vector3 offset;
        Transform destSquare = transform.Find("Tiles");
        destSquare = destSquare.Find("Model " + dest);
        if (Move.IsCapture(move)){
            RaycastHit hit;
            if (!Move.IsEnpassant(move)){
                if (Physics.Raycast(destSquare.position + new Vector3(0.5f, 0f, 0.5f), Vector3.up, out hit, 10.0f, LayerMask.GetMask("White") | LayerMask.GetMask("Black")))
                    Destroy(hit.collider.gameObject);
                 else throw new Exception("Capture Piece not on square!");
            } else {
                offset = Chessboard.PlayerTurn == Side.White ? new Vector3(0.5f, 0f, -0.5f) : new Vector3(0.5f, 0f, 1.5f);
                if (Physics.Raycast(destSquare.position + offset, Vector3.up, out hit, 10.0f, LayerMask.GetMask("White") | LayerMask.GetMask("Black")))
                    Destroy(hit.collider.gameObject);
                else throw new Exception("Enpassant Piece not on square!");
            }
        } else if (Move.IsCastle(move)){
            Square rookSq;
            RaycastHit hit;
            // Offset depends on rook in the a1, h1, a8, h8
            switch (dest){
                case (int)Square.g1:
                    offset = new Vector3(1.5f, 0f, 0.5f);
                    rookSq = Square.f1;
                    break;
                case (int)Square.c1:
                    offset = new Vector3(-1.5f, 0f, 0.5f);
                    rookSq = Square.d1;
                    break;
                case (int)Square.g8:
                    offset = new Vector3(1.5f, 0f, 0.5f);
                    rookSq = Square.f8;
                    break;
                case (int)Square.c8:
                    offset = new Vector3(-1.5f, 0f, 0.5f);
                    rookSq = Square.d8;
                    break;
                default: throw new Exception("I dunno how this error happened ngl");
            }
            if (Physics.Raycast(destSquare.position + offset, Vector3.up, out hit, 10.0f, LayerMask.GetMask("White") | LayerMask.GetMask("Black"))){
                hit.collider.transform.parent.transform.localPosition = IndexToCoord((int)rookSq);
            } else throw new Exception("Castling Rook not found!");
        }
        SelectedPiece.transform.localPosition = IndexToCoord(dest) + new Vector3(0f, 0.2f, 0f);
        DeselectPiece();
    }

    // Handles all the move inputs to get from actions on board to move ints
    private int HandleMoveInputs(){
        GameObject temp = GetSelectedPiece();
        int destSquare = GetSelectedSquare();
        // If there is a piece selected and its a new piece
        if (temp != null && temp != SelectedPiece){
            DeselectPiece();
            SelectedPiece = temp;
            SelectedPieceAttacks = MoveGeneration.SortMoves(Attacks, Properties.src, CoordToIndex(SelectedPiece.transform));
            VisualizeMoves(SelectedPieceAttacks, SelectedPiece);
        } 
        // If there is a piece selected and its the same piece
        else if (temp != null && temp == SelectedPiece) DeselectPiece();
        // If there is a piece selected and its a square that was clicked
        else if (temp == null && SelectedPiece != null && destSquare != -1){
            int[] chosenMoves = MoveGeneration.SortMoves(SelectedPieceAttacks, Properties.dest, destSquare);
            return chosenMoves[0];
        }
        return 0;
    }

    // Should be run the first time the board is created, adds all the pieces on the board
    private void PlacePieces(){
        for (int pieces = 0; pieces < Board.BitboardCount; pieces++){
            ulong pieceBitboard = Chessboard.Bitboards[pieces];
            int count = Helper.CountBit(pieceBitboard);
            for (int iterator = 0; iterator < count; iterator++){
                GameObject pieceObject;
                int index = Helper.LSBIndex(pieceBitboard);
                Helper.PopBit(ref pieceBitboard, index);
                Vector3 position = IndexToCoord(index);
                switch((Piece)pieces){
                    case Piece.WPawn: pieceObject = WPawn; break;
                    case Piece.WKnight: pieceObject = WKnight; break;
                    case Piece.WBishop: pieceObject = WBishop; break;
                    case Piece.WRook: pieceObject = WRook; break;
                    case Piece.WQueen: pieceObject = WQueen; break;
                    case Piece.WKing: pieceObject = WKing; break;
                    case Piece.BPawn: pieceObject = BPawn; break;
                    case Piece.BKnight: pieceObject = BKnight; break;
                    case Piece.BBishop: pieceObject = BBishop; break;
                    case Piece.BRook: pieceObject = BRook; break;
                    case Piece.BQueen: pieceObject = BQueen; break;
                    case Piece.BKing: pieceObject = BKing; break;
                    default: throw new System.Exception("Invalid Piece!");
                }
                GameObject piece = Instantiate(pieceObject, PieceParent.transform, false);
                piece.transform.localPosition = position;
                Pieces.Add(piece);
            }
        }
    }

    // Raycasts the mouse position on the screen to the board. Returns the piece that the ray hits.
    private GameObject GetSelectedPiece(){
        Vector3 screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Transform piece;
        int layer = Chessboard.PlayerTurn == Side.White ? LayerMask.GetMask("White") : LayerMask.GetMask("Black");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer)){
            piece = hit.transform;
        } else return null;
        return piece.parent.gameObject;
    }

    // Using a raycast, determined the square that was clicked on
    private int GetSelectedSquare(){
        Vector3 screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Board")))
            return int.Parse(hit.collider.name.Split(" ")[1]);
        return -1;
    }

    // Gives the square to coordinate in world 
    private Vector3 IndexToCoord(int index) => new Vector3(-1 * index % 8, 0.3f, index / 8);

    // Gives the coordinate to square index
    private int CoordToIndex(Transform position) => Math.Abs((int)position.localPosition.x) + Math.Abs((int)position.localPosition.z) * 8;

    // Visualizes the moves that a piece can do, as well as the current selected piece
    private void VisualizeMoves(int[] moves, GameObject piece){
        SelectedPieceVisuals.Add(Instantiate(SelectedSquareIndicator, piece.transform.position, Quaternion.identity));
        piece.transform.localPosition += new Vector3(0f, 0.2f, 0f);
        foreach (int move in moves){
            if (move != 0){
                GameObject obj;
                if (Move.IsCapture(move))
                    obj = Instantiate(AttackedMovesIndicator, PieceParent.transform, false);
                else
                    obj = Instantiate(PossibleMovesIndicator, PieceParent.transform, false);
                obj.transform.localPosition = IndexToCoord(Move.GetDestSquare(move));
                SelectedPieceVisuals.Add(obj);
            }
        }
    }

    // Removes the visuals for a selected piece
    private void DeselectPiece(){
        if (SelectedPieceVisuals == null || SelectedPiece == null) return;
        SelectedPiece.transform.localPosition -= new Vector3(0f, 0.2f, 0f);

        SelectedPiece = null;
        for (int i = 0; i < SelectedPieceVisuals.Count; i++){
            Destroy(SelectedPieceVisuals[i]);
        }
        SelectedPieceVisuals.Clear();
    }
}
