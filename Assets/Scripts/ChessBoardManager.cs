using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/8/8/8/1n1Brp2/2PPP3/1P4P1/8 w - - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
        MoveGeneration.GenerateMoves(Chessboard);
    }
}
