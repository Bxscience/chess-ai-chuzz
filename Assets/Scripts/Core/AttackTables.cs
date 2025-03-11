public struct AttackTables{
    // Constants that hold 1's in all positions except the files denoted by the name
    private const ulong _NotAFile = 0x7F7F7F7F7F7F7F7F, 
                       _NotHFile = 0xFEFEFEFEFEFEFEFE,
                       _NotABFile = 0x3F3F3F3F3F3F3F3F,
                       _NotGHFile = 0xFCFCFCFCFCFCFCFC;

    // Pregenerated Pawn Attacks Table
    public readonly static ulong[,] PawnAttacks = new ulong[(int)Side.Both, Board.BoardSize * Board.BoardSize];
    // Pregenerated Knights Attacks Table
    public readonly static ulong[] KnightAttacks = new ulong[Board.BoardSize * Board.BoardSize];
    // Pregenerated King Attacks Table
    public readonly static ulong[] KingAttacks = new ulong[Board.BoardSize * Board.BoardSize];
    // Pregenerated Bishop Attacks Table
    public readonly static ulong[,] BishopAttacks = new ulong[Board.BoardSize * Board.BoardSize, 512];
    // Pregenerated Rook Attacks Table
    public readonly static ulong[,] RookAttacks = new ulong[Board.BoardSize * Board.BoardSize, 4096];
    public readonly static ulong[] BishopMasks = new ulong[Board.BoardSize * Board.BoardSize];
    public readonly static ulong[] RookMasks  = new ulong[Board.BoardSize * Board.BoardSize];

    // Create Attack Tables
    public static void InitAttackTables(){
        for (int square = 0; square < 64; square++){
            MaskPawnAttacks(square);
            MaskKnightAttacks(square);
            MaskKingAttacks(square);
        }
        InitSlidersAttacks(true);
        InitSlidersAttacks(false);
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
    public static ulong MaskBishopAttacks(int index){
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
    public static ulong MaskRookAttacks(int index){
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
    public static ulong GenerateBishopAttacks(int index, ulong blocker){
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
        return attacks;
    }

    // Generates rook attacks on the fly
    public static ulong GenerateRookAttacks(int index, ulong blocker){
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
        return attacks;
    }

    // Converts index to a specific position on the attack mask
    public static ulong SetOccupancy(int index, int bitsInMask, ulong attackMask){
        ulong occupancy = 0ul, bitboard = attackMask;
        for(int i = 0; i < bitsInMask; i++){
            int square = Helper.LSBIndex(bitboard);
            Helper.PopBit(ref bitboard, square);
            if (Helper.CheckBit((ulong)index, 1ul << i, i))
                occupancy |= 1ul << square;
        }
        return occupancy;
    }

    // Initialize sliding pieces, true for bishop, false for rooks.
    private static void InitSlidersAttacks(bool isBishop){
        for (int square = 0; square < Board.BoardSize * Board.BoardSize; square++){
            BishopMasks[square] = MaskBishopAttacks(square);
            RookMasks[square] = MaskRookAttacks(square);
            ulong attackMask = isBishop ? BishopMasks[square] : RookMasks[square];
            int relevantbits = isBishop ? Pregen.BishopRelevantBits[square] : Pregen.RookRelevantBits[square];
            int occupancyIndicies = 1 << relevantbits;
            for (int index = 0; index < occupancyIndicies; index++){
                if (isBishop){
                    ulong occupancy = SetOccupancy(index, relevantbits, attackMask);
                    int magicIndex = (int)((occupancy * Pregen.BishopMagics[square]) >> (64 - relevantbits));
                    BishopAttacks[square, magicIndex] = GenerateBishopAttacks(square, occupancy);
                } else { 
                    ulong occupancy = SetOccupancy(index, relevantbits, attackMask);
                    int magicIndex = (int)((occupancy * Pregen.RookMagics[square]) >> (64 - relevantbits));
                    RookAttacks[square, magicIndex] = GenerateRookAttacks(square, occupancy);
                }   
            }
        }
    }

    // Gets the moves for bishops at a specific square
    public static ulong GetBishopAttacks(int square, ulong occupancy){
        occupancy &= BishopMasks[square];
        occupancy *= Pregen.BishopMagics[square];
        occupancy >>= 64 - Pregen.BishopRelevantBits[square];
        return BishopAttacks[square, occupancy];
    }

    // Gets the moves for a rook at a specific square
    public static ulong GetRookAttacks(int square, ulong occupancy){
        occupancy &= RookMasks[square];
        occupancy *= Pregen.RookMagics[square];
        occupancy >>= 64 - Pregen.RookRelevantBits[square];
        return RookAttacks[square, occupancy];
    }
    
    // Gets the moves for a queen at a specific square
    public static ulong GetQueenAttacks(int square, ulong occupancy) => GetBishopAttacks(square, occupancy) | GetRookAttacks(square, occupancy);


    // Given a square to check whether it is attacked by the given side, after given a board position of bitboards and occupancies.
    // For pawns, for a given square, find the attacks of a temp pawn of the opposite color on the given square
    // If it attacks any of the pawns currently on the board, it means the square is being attacked
    // Similar concept for other pieces, but since they are symmetric we don't need multiple if-conditions
    public static bool IsSquareAttacked(int square, ulong[] bitboards, ulong[] occupancies, Side side){
        // Checks attacks by white pawns
        if (side == Side.White && (PawnAttacks[(int)Side.Black, square] & bitboards[(int)Piece.WPawn]) > 0) return true;
        // Checks attacks by black pawns
        if (side == Side.Black && (PawnAttacks[(int)Side.White, square] & bitboards[(int)Piece.BPawn]) > 0) return true;
        // Checks attacks by knights
        if ((KnightAttacks[square] & (side == Side.White ? bitboards[(int)Piece.WKnight] : bitboards[(int)Piece.BKnight])) > 0) return true;
        // Checks attacks by kings
        if ((KingAttacks[square] & (side == Side.White ? bitboards[(int)Piece.WKing] : bitboards[(int)Piece.BKing])) > 0) return true;
        // Checks attacks by bishops        
        if ((GetBishopAttacks(square, occupancies[(int)Side.Both]) & (side == Side.White ? bitboards[(int)Piece.WBishop] : bitboards[(int)Piece.BBishop])) > 0) return true;
        // Checks attacks by rooks
        if ((GetRookAttacks(square, occupancies[(int)Side.Both]) & (side == Side.White ? bitboards[(int)Piece.WRook] : bitboards[(int)Piece.BRook])) > 0) return true;
        // No checks needed for queens as its covered by bishop & rook
        return false;
    }
}