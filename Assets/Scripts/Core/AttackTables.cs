public struct AttackTables{
    private static Board _Board;
    public const ulong _NotAFile = 9187201950435737471ul, 
                       _NotHFile = 18374403900871474942ul;
    public ulong[,] PawnAttacks;
    
    public AttackTables(Board board){
        PawnAttacks = new ulong[2, 64];
        _Board = board;
    }

    public static ulong MaskPawnAttacks(int index){
        ulong attacks = 0ul, bitboard = 0ul;
        Helper.SetBit(ref bitboard, index);
        if (_Board.Iswhiteturn){
            attacks |= (bitboard << 7) & _NotAFile;
            attacks |= (bitboard << 9) & _NotHFile;
        }
        else{
            attacks |= (bitboard >> 7) & _NotHFile;
            attacks |= (bitboard >> 9) & _NotAFile;
        }
        return attacks;
    }
}