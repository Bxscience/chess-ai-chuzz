public struct AttackTables{
    // Constants that hold 1's in all positions except the files denoted by the name
    private const ulong _NotAFile = 9187201950435737471ul, 
                       _NotHFile = 18374403900871474942ul,
                       _NotABFile = 4557430888798830399ul,
                       _NotGHFile = 18229723555195321596ul;

    // Pregenerated Pawn Attacks Table
    public readonly static ulong[,] PawnAttacks = new ulong[2, Board.BoardSize * Board.BoardSize];
    public readonly static ulong[] KnightAttacks = new ulong[Board.BoardSize * Board.BoardSize];
    public readonly static ulong[] KingAttacks = new ulong[Board.BoardSize * Board.BoardSize];
    public readonly static ulong[] BishopAttacks = new ulong[Board.BoardSize * Board.BoardSize];
    public readonly static ulong[] RookAttacks = new ulong[Board.BoardSize * Board.BoardSize];

    // Create Attack Tables
    public static void InitAttackTables(){
        for (int i = 0; i < Board.BoardSize * Board.BoardSize; i++){
            MaskPawnAttacks(i);
            MaskKnightAttacks(i);
            MaskKingAttacks(i);
            GenerateBishopAttacks(i, 0ul);
            GenerateRookAttacks(i, 0ul);
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
        PawnAttacks[(int)Side.White, index] = wattacks;
        PawnAttacks[(int)Side.Black, index] = battacks;
    }

    // Pregenerates attack moves for Knight
    private static void MaskKnightAttacks(int index){
        ulong attacks = 0ul, bitboard = 0ul;
        Helper.SetBit(ref bitboard, index);
        attacks |= (bitboard << 6) & _NotABFile;
        attacks |= (bitboard << 10) & _NotGHFile;
        attacks |= (bitboard << 15) & _NotAFile;
        attacks |= (bitboard << 17) & _NotHFile;
        attacks |= (bitboard >> 6) & _NotGHFile;
        attacks |= (bitboard >> 10) & _NotABFile;
        attacks |= (bitboard >> 15) & _NotHFile;
        attacks |= (bitboard >> 17) & _NotAFile;
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

    // Generates occupancy bits for bishop (Not attack moves)
    private static ulong MaskBishopAttacks(int index){
        ulong attacks = 0ul;
        int rank, file;
        int targetRank = index / 8, targetFile = index % 8;
        for (rank = targetRank + 1, file = targetFile + 1; rank <= 6 && file <= 6; rank++, file++)
            attacks |= 1ul << (rank * Board.BoardSize + file);
        for (rank = targetRank - 1, file = targetFile + 1; rank >= 1 && file <= 6; rank--, file++)
            attacks |= 1ul << (rank * Board.BoardSize + file);
        for (rank = targetRank + 1, file = targetFile - 1; rank <= 6 && file >= 1; rank++, file--)
            attacks |= 1ul << (rank * Board.BoardSize + file);
        for (rank = targetRank - 1, file = targetFile - 1; rank >= 1 && file >= 1; rank--, file--)
            attacks |= 1ul << (rank * Board.BoardSize + file);
        return attacks;
    }

    // Generates occupancy bits for rook (Not attack moves)
    private static ulong MaskRookAttacks(int index){
        ulong attacks = 0ul;
        int rank, file;
        int targetRank = index / 8, targetFile = index % 8;
        for (rank = targetRank + 1; rank <= 6; rank++)
            attacks |= 1ul << (rank * Board.BoardSize + targetFile);
        for (rank = targetRank - 1; rank >= 1; rank--)
            attacks |= 1ul << (rank * Board.BoardSize + targetFile);
        for (file = targetFile + 1; file <= 6; file++)
            attacks |= 1ul << (targetRank * Board.BoardSize + file);
        for (file = targetFile - 1; file >= 1; file--)
            attacks |= 1ul << (targetRank * Board.BoardSize + file);
        return attacks;
    }

    // Generates bishop attacks on the fly
    private static void GenerateBishopAttacks(int index, ulong blocker){
        ulong attacks = 0ul;
        int rank, file;
        int targetRank = index / 8, targetFile = index % 8;
        for (rank = targetRank + 1, file = targetFile + 1; rank <= 7 && file <= 7; rank++, file++){
            int square = rank * Board.BoardSize + file;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        for (rank = targetRank - 1, file = targetFile + 1; rank >= 0 && file <= 7; rank--, file++){
            int square = rank * Board.BoardSize + file;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        for (rank = targetRank + 1, file = targetFile - 1; rank <= 7 && file >= 0; rank++, file--){
            int square = rank * Board.BoardSize + file;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        for (rank = targetRank - 1, file = targetFile - 1; rank >= 0 && file >= 0; rank--, file--){
            int square = rank * Board.BoardSize + file;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        BishopAttacks[index] = attacks;
    }

    // Generates rook attacks on the fly
    private static void GenerateRookAttacks(int index, ulong blocker){
        ulong attacks = 0ul;
        int rank, file;
        int targetRank = index / 8, targetFile = index % 8;
        for (rank = targetRank + 1; rank <= 7; rank++){
            int square = rank * Board.BoardSize + targetFile;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        for (rank = targetRank - 1; rank >= 0; rank--){
            int square = rank * Board.BoardSize + targetFile;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        for (file = targetFile + 1; file <= 7; file++){
            int square = targetRank * Board.BoardSize + file;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        for (file = targetFile - 1; file >= 0; file--){
            int square = targetRank * Board.BoardSize + file;
            attacks |= 1ul << square;
            if (Helper.CheckBit(1ul << square, blocker, square)) break;
        }
        RookAttacks[index] = attacks;
    }
}