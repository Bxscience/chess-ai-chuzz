using UnityEngine.UIElements.Experimental;

public struct AttackTables{
    // Constants that hold 1's in all positions except the files denoted by the name
    public const ulong _NotAFile = 9187201950435737471ul, 
                       _NotHFile = 18374403900871474942ul,
                       _NotABFile = 4557430888798830399ul,
                       _NotGHFile = 18229723555195321596ul;

    // Pregenerated Pawn Attacks Table
    public static ulong[,] PawnAttacks = new ulong[2, Board.BoardSize * Board.BoardSize];
    public static ulong[] KnightAttacks = new ulong[Board.BoardSize * Board.BoardSize];
    public static ulong[] KingAttacks = new ulong[Board.BoardSize * Board.BoardSize];

    // Create Attack Tables
    public static void InitAttackTables(){
        for (int i = 0; i < Board.BoardSize * Board.BoardSize; i++){
            MaskPawnAttacks(i);
            MaskKnightAttacks(i);
            MaskKingAttacks(i);
        }
    }

    // Pregenerates attack moves for both White and Black Pawns
    private static void MaskPawnAttacks(int index){
        ulong wattacks = 0ul, battacks = 0ul, bitboard = 0ul;
        Helper.SetBit(ref bitboard, index);
        wattacks |= (bitboard << 7) & _NotAFile;
        wattacks |= (bitboard << 9) & _NotHFile;
        battacks |= (bitboard >> 7) & _NotHFile;
        battacks |= (bitboard >> 9) & _NotAFile;
        PawnAttacks[(int)Color.White, index] = wattacks;
        PawnAttacks[(int)Color.Black, index] = battacks;
    }

    // Need to fix AHHHHHHH (Error when knights at edges due to move being inside _NOT files)
    private static void MaskKnightAttacks(int index){
        ulong attacks = 0ul, bitboard = 0ul;
        Helper.SetBit(ref bitboard, index);
        attacks |= (bitboard << 6) & _NotABFile;
        attacks |= (bitboard << 10) & _NotGHFile;
        attacks |= (bitboard << 15) & _NotABFile;
        attacks |= (bitboard << 17) & _NotGHFile;
        attacks |= (bitboard >> 6) & _NotGHFile;
        attacks |= (bitboard >> 10) & _NotABFile;
        attacks |= (bitboard >> 15) & _NotGHFile;
        attacks |= (bitboard >> 17) & _NotABFile;
        KnightAttacks[index] = attacks;
    }

    // Pregenerates attack moves for King
    private static void MaskKingAttacks(int index){
        ulong attacks = 0ul, bitboard = 0ul;
        Helper.SetBit(ref bitboard, index);
        attacks |= (bitboard << 1) & _NotHFile;
        attacks |= (bitboard << 7) & _NotAFile;
        attacks |= bitboard << 8;
        attacks |= (bitboard << 9) & _NotHFile;
        attacks |= (bitboard >> 1) & _NotAFile;
        attacks |= (bitboard >> 7) & _NotHFile;
        attacks |= bitboard >> 8;
        attacks |= (bitboard >> 9) & _NotAFile;
        KingAttacks[index] = attacks;
    }

}