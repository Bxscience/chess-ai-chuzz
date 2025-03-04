using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public Board Chessboard;
    void Start(){
        Chessboard = new Board("");
        AttackTables.InitAttackTables();
        Chessboard.TestBoard();
    }
}
