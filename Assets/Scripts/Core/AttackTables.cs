using UnityEngine.UI;

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
    public readonly static ulong[,] BishopAttacks = new ulong[Board.BoardSize * Board.BoardSize, 512];
    public readonly static ulong[,] RookAttacks = new ulong[Board.BoardSize * Board.BoardSize, 4096];

    public readonly static ulong[] BishopMasks = new ulong[Board.BoardSize * Board.BoardSize];
    public readonly static ulong[] RookMasks  = new ulong[Board.BoardSize * Board.BoardSize];

    // Create Attack Tables
    public static void InitAttackTables(){
        InitSlidersAttacks(true);
        InitSlidersAttacks(false);
    }

    // Pregenerates attack moves for both White and Black Pawns
    public static void MaskPawnAttacks(int index){
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
    public static ulong MaskKnightAttacks(int index){
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
        return attacks;
    }

    // Pregenerates attack moves for King
    public static ulong MaskKingAttacks(int index){
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
        return attacks;
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

    // Converts index to a speific position on the attack mask
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

    // Initialize sliding pieces
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

//======================================== Get Moves from Pieces ========================================//
    // Gets the moves 
    public static ulong GetBishopAttacks(int square, ulong occupancy){
        occupancy &= BishopMasks[square];
        occupancy *= Pregen.BishopMagics[square];
        occupancy >>= 64 - Pregen.BishopRelevantBits[square];
        return BishopAttacks[square, occupancy];
    }

    public static ulong GetRookAttacks(int square, ulong occupancy){
        occupancy &= RookMasks[square];
        occupancy *= Pregen.RookMagics[square];
        occupancy >>= 64 - Pregen.RookRelevantBits[square];
        return RookAttacks[square, occupancy];
    }

    public static ulong GetQueenAttacks(int square, ulong occupancy) => GetBishopAttacks(square, occupancy) | GetRookAttacks(square, occupancy);
}