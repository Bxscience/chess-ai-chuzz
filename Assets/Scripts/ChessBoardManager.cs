using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    void Start(){
        Chessboard = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        Chessboard.TestBoard();
    }
}
