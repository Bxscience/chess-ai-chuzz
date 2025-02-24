public enum Piece{
    WPawn = 0, WBishop = 1, WKnight = 2, WRook = 3, WQueen = 4, WKing = 5,
    BPawn = 6, BBishop = 7, BKnight = 8, BRook = 9, BQueen = 10, BKing = 11
}

public enum Color{
    Black = 0, White = 1
}

public enum Square{
    h1, g1, f1, e1, d1, c1, b1, a1,
    h2, g2, f2, e2, d2, c2, b2, a2,
    h3, g3, f3, e3, d3, c3, b3, a3,
    h4, g4, f4, e4, d4, c4, b4, a4,
    h5, g5, f5, e5, d5, c5, b5, a5,
    h6, g6, f6, e6, d6, c6, b6, a6,
    h7, g7, f7, e7, d7, c7, b7, a7,
    h8, g8, f8, e8, d8, c8, b8, a8

}

public struct Helper{
//======================================== Bit Manipulation ========================================//
    // Returns the bit (in int type) in a bitboard of an index
    public static int GetBit(ulong Bitboard, int index){return (int)(Bitboard >> index & 1ul);}

    // Changes the bit in a bitboard of an index to 1
    public static void SetBit(ref ulong Bitboard, int index){Bitboard |= 1ul << index;}

    // If bit is on at a certain index, change to zero, otherwise do nothing
    public static void PopBit(ref ulong Bitboard, int index){if (GetBit(Bitboard, index) == 1) Bitboard ^= 1ul << index;}

//======================================== Output ========================================//
    // Prints out the bitboard in a human-friendly way
    public static void PrintBitboard(ulong Bitboard){
        string output = "";
        for (int rank = Board.BoardSize - 1; rank >= 0; rank--){
            for (int file = Board.BoardSize - 1; file >= 0; file--){
                int index = rank * Board.BoardSize + file;
                output += GetBit(Bitboard, index) == 0 ? "0 " : "1 ";
            }
            output += "\n";
        }
        UnityEngine.Debug.Log(output);
    }
}