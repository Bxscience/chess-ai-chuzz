using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    // Empty Board: 8/8/8/8/8/8/8/8 w ---- - 0 1
    void Start(){
        Chessboard = new Board("8/8/1Q6/4P3/4PP2/1N6/6B1/3KR3 w - - 0 1");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
        MoveGeneration.InitAttackedSquares(Chessboard);
        Helper.PrintBitboard(MoveGeneration.AttackedSquares[(int)Side.White]);
        MoveGeneration.GenerateAttackMap(Chessboard);
        int move = Move.EncodeMove((int)Square.h1, (int)Square.h2, Piece.BKing, Piece.WPawn, true, false, true, false);
        Move.PrintMove(move);
    }
}
