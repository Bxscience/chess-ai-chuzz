public enum Piece{
    WPawn = 0, WBishop = 1, WKnight = 2, WRook = 3, WQueen = 4, WKing = 5,
    BPawn = 6, BBishop = 7, BKnight = 8, BRook = 9, BQueen = 10, BKing = 11
}

public enum Color{
    Black = 0, White = 1
}

public enum Square{
    //WIP
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