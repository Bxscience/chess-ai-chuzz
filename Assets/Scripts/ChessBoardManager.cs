using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/1p1p1p2/5N2/1B6/1p2p3/3N4/1p4p1/4bR1N w - - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
        int[] attacks = MoveGeneration.InitMoves(Chessboard, Side.Black);
        foreach (int move in attacks){
            if (move != 0)
                Move.PrintMove(move);
        }
    }
}
