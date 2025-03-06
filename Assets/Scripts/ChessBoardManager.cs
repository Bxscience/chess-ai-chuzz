using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/pppppppp/8/4P3/4R3/3R4/2PPP3/8 w ---- - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
        MoveGeneration.GenerateMoves(Chessboard);
    }
}
