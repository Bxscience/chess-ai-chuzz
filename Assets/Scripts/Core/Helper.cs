using System;

public enum Piece{
    WPawn = 0, WBishop = 1, WKnight = 2, WRook = 3, WQueen = 4, WKing = 5,
    BPawn = 6, BBishop = 7, BKnight = 8, BRook = 9, BQueen = 10, BKing = 11
}

public enum Side{
    Black = 0, White = 1
}

// Enum Defining Squares to Bitboard Indexes used for move generation
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
// Some bit twiddling hacks came from http://graphics.stanford.edu/%7Eseander/bithacks.html

    // Gets a random ulong for the magic bitboard
    private static ulong GetRandomUlong(){
        Random rand = new Random();
        return (ulong)rand.Next() << 32 | (uint)rand.Next();
    }

    // Returns the bit (in int type) in a bitboard of an index
    public static int GetBit(ulong Bitboard, int index){return (int)(Bitboard >> index & 1ul);}

    // Changes the bit in a bitboard of an index to 1
    public static void SetBit(ref ulong Bitboard, int index){Bitboard |= 1ul << index;}

    // If bit is on at a certain index, change to zero, otherwise do nothing
    public static void PopBit(ref ulong Bitboard, int index){if (GetBit(Bitboard, index) == 1) Bitboard ^= 1ul << index;}

    // Returns true if two ulongs have the same value at the same index, else returns false
    public static bool CheckBit(ulong a, ulong b, int index){return GetBit(a, index) == GetBit(b, index);}

    // Returns the amount of bits on in a bitboard (WIP, might update to more efficient method)
    public static int CountBit(ulong Bitboard){
        int count = 0;
        while(Bitboard > 0ul){
            count++;
            Bitboard &= Bitboard - 1;
        }
        return count;
    }

    // Gets the index of the LSB in a bitboard (includes the LSB)
    public static int LSBIndex(ulong Bitboard){
        ulong count;
        if (CheckBit(Bitboard, 1ul, 0))
            count = 0;
        else{
            count = 1;
            if ((Bitboard & 0xffffffff) == 0){
                Bitboard >>= 32;
                count += 32;
            }
            if ((Bitboard & 0xffff) == 0){
                Bitboard >>= 16;
                count += 16;
            }
            if ((Bitboard & 0xff) == 0){
                Bitboard >>= 8;
                count += 8;
            }
            if ((Bitboard & 0xf) == 0){
                Bitboard >>= 4;
                count += 4;
            }
            if ((Bitboard & 0x3) == 0){
                Bitboard >>= 2;
                count += 2;
            }
            count -= Bitboard & 0x1;
        }
        return (int)count;
    }

    // Returns a ulong with a bias towards zero
    public static ulong GetBiasedUlong(){
        return GetRandomUlong() & GetRandomUlong() & GetRandomUlong();
    }

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