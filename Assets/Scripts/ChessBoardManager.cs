using UnityEngine;

public class ChessBoardManager : MonoBehaviour{
    public static Board Chessboard;
    void Start(){
        Chessboard = new Board();
    }

    public void DisplayBitboards(){
        
    }
}
