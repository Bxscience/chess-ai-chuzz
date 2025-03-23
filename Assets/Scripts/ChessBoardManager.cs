using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/8/1Q6/n2PP3/4pP2/1N3N1q/6B1/3KR3 w - - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
        int[] attacks = MoveGeneration.InitMoves(Chessboard, Side.White);
        //foreach (int move in attacks){
        //    Move.PrintMove(move);
        //}
    }
}
