using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardManager : MonoBehaviour{

    [SerializeField]
    private GameObject PieceParent, WPawn, WKnight, WBishop, WRook, WQueen, WKing, BPawn, BKnight, BBishop, BRook, BQueen, BKing;
    private List<GameObject> Pieces;
    public Board Chessboard;
    public GameObject SelectedPiece;
    public int[] attacks;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Pieces = new List<GameObject>();
        Chessboard = new Board("");
        AttackTables.InitAttackTables();
        attacks = MoveGeneration.InitMoves(Chessboard, Side.White);
        PlacePieces();
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            GameObject temp = GetSelectedPiece();
            if (temp != null && temp != SelectedPiece){
                SelectedPiece = temp;
                int[] pieceMoves = GetMoveListForPiece(attacks, CoordToIndex(SelectedPiece.transform));
                VisualizeMoves(pieceMoves, SelectedPiece);
            }
        }

        if (Input.GetKeyDown(KeyCode.N)){
            Chessboard.PlayerTurn = Helper.GetOpponent(Chessboard.PlayerTurn);
        }
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
                Vector3 position = new Vector3(index % 8 - 7f, 0f, index / 8);
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
        Camera camera = Camera.main;
        Vector3 screenPosition = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        Transform SelectedPiece = null;
        if (hit.collider != null){
            switch(Chessboard.PlayerTurn){
                case Side.White: if (hit.collider.tag == "White Pieces") SelectedPiece = hit.transform; break;
                case Side.Black: if (hit.collider.tag == "Black Pieces") SelectedPiece = hit.transform; break;
                default: throw new Exception("Something went wrong!");
            }
        } else return null; 
        return SelectedPiece.parent.gameObject;
    }

    // Gives the coordinate to square index
    private int CoordToIndex(Transform position) => Math.Abs((int)position.localPosition.x) + Math.Abs((int)position.localPosition.z) * 8;

    // Given the square the piece is on, gives the possible moves for the list
    private int[] GetMoveListForPiece(int[] moveList, int index){
        int[] possibleMoves = new int[256];
        int arrayIndex = 0;
        foreach (int moves in moveList){
            if (Move.GetSrcSquare(moves) == index){
                possibleMoves[arrayIndex++] = moves;
            } else if (moves == 0) break;
        } 
        return possibleMoves;
    }

    private void VisualizeMoves(int[] moves, GameObject piece){
        piece.transform.localPosition += new Vector3(0f, 0.2f, 0f);
    }
}
