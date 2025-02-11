using UnityEditor;
using UnityEngine;

public class ChessGameManager : MonoBehaviour{

    private const int BOARD_SIDE = 8;
    
    [SerializeField]
    private GameObject WPawn, WBishop, WKnight, WRook, WQueen, WKing,
                       BPawn, BBishop, BKnight, BRook, BQueen, BKing;


    [SerializeField, Tooltip("Supports a FEN string notation")]
    private string startPosition;

    void Onable(){
        startPosition = startPosition == "" ? startPosition : "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        parseFENString();
    }

    private void parseFENString(){
        Vector3 FENStartPosition = transform.position - new Vector3(3.85f, 0.3f, 3.85f);
        for(int i = 0; i < startPosition.Length; i++){
            char letter = startPosition[i];
            if ()
            float x = i % BOARD_SIDE * 1.1f;
            float y = -1 * (i / BOARD_SIDE * 1.1f); 
        }
    }
}
