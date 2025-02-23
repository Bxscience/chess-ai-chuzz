public struct AttackTables{
    // Constants that hold 1's in all positions except the files denoted by the name
    public const ulong _NotAFile = 9187201950435737471ul, 
                       _NotHFile = 18374403900871474942ul,
                       _NotABFile = 18229723555195321596ul,
                       _NotGHFile = 4557430888798830399ul;
    
    // Pregenerated Pawn Attacks Table
    public static ulong[,] PawnAttacks = new ulong[2, 64];

    // Create Attack Tables
    public static void InitAttackTables(){
        for (int i = 0; i < Board.BoardSize * Board.BoardSize; i++){
            MaskPawnAttacks(i);
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

}