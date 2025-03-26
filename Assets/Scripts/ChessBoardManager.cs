using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ChessBoardManager : MonoBehaviour{

    [SerializeField]
    private GameObject PieceParent, WPawn, WKnight, WBishop, WRook, WQueen, WKing, BPawn, BKnight, BBishop, BRook, BQueen, BKing;
    private List<GameObject> Pieces;
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Pieces = new List<GameObject>();
        Chessboard = new Board("");
        AttackTables.InitAttackTables();
        int[] attacks = MoveGeneration.InitMoves(Chessboard, Side.Black);
        PlacePieces();
    }

    void Update(){
        if (Input.GetMouseButtonDown(0))
            GetSelectedPiece();
    }

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

    private GameObject GetSelectedPiece(){
        Camera camera = Camera.main;
        Vector3 screenPosition = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        GameObject SelectedPiece = null;
        switch(Chessboard.PlayerTurn){
            case Side.White: if (hit.collider.tag == "White Pieces") SelectedPiece = hit.transform.gameObject; break;
            case Side.Black: if (hit.collider.tag == "Black Pieces") SelectedPiece = hit.transform.gameObject; break;
            default: throw new System.Exception("Something went wrong!");
        }
        return SelectedPiece;
    }
}
