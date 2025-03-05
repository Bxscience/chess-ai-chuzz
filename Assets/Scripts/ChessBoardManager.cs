using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/8/8/8/8/8/8/8 w ---- - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
    }
}
