using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/8/1Q6/4P3/4PP2/1N6/6B1/3KR3 w - - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
        MoveGeneration.GenerateMoves(Chessboard);
    }
}
