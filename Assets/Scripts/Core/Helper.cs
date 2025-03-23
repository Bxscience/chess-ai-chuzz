using System;

public struct Helper{
//======================================== Bit Manipulation ========================================//
// Some bit twiddling hacks came from http://graphics.stanford.edu/%7Eseander/bithacks.html

    // Gets a random ulong for the magic bitboard
    private static ulong GetRandomUlong(){
        Random rand = new Random();
        return (ulong)rand.Next() << 32 | (uint)rand.Next();
    }

    // Returns the bit (in int type) in a bitboard of an index
    public static int GetBit(ulong Bitboard, int index) => (int)((Bitboard >> index) & 1ul);

    // Changes the bit in a bitboard of an index to 1
    public static void SetBit(ref ulong Bitboard, int index) => (Bitboard) |= (1ul << index);

    // If bit is on at a certain index, change to zero, otherwise do nothing
    public static void PopBit(ref ulong Bitboard, int index) => (Bitboard) &= ~(1ul << index);

    // Returns true if two ulongs have the same value at the same index, else returns false
    public static bool CheckBit(ulong a, ulong b, int index) => GetBit(a, index) == GetBit(b, index);

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

    // Returns a ulong with a bias towards zero bits
    public static ulong GetBiasedUlong() => GetRandomUlong() & GetRandomUlong() & GetRandomUlong();

//======================================== Input ========================================//
    // Places the inputted piece on the given square
    public static void PlacePiece(Board board, Piece piece, Square square) => SetBit(ref board.Bitboards[(int)piece], (int)square);

    

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