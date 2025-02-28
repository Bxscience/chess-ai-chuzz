public struct MagicBitboards{
    // TODO: Understand this function fully and fix if needed
    public static ulong FindMagicNumber(int index, int relevantbits, bool isBishop){
        // Occupancies represent different positions of blocks & available moves
        // Max length is determined by all possible moves the piece has (9 for bishop, 12 for rook)
        // This determines the max possible different configurations of blockers for a piece
        int maxLength = isBishop ? 512 : 4096;
        // Occupancies represent a specific situation where a 1 represents a blocker on that square
        ulong[] occupancies = new ulong[maxLength], 
                // Attacks is the moves available to the piece after occupancies are accounted for
                attacks = new ulong[maxLength], 
                usedAttacks = new ulong[maxLength];

        // Initializes the usedAttacks array and sets all values to zero
        for (int i = 0; i < maxLength; i++) usedAttacks[i] = 0ul;

        // Attack mask is an bitboard where 1 represents the possible places the rook attacks
        ulong attackMask = isBishop ? AttackTables.MaskBishopAttacks(index) : AttackTables.MaskRookAttacks(index);

        // Occupancy indicies determines the actual possible configurations of blockers a piece on that square has
        // AKA the amount of different occupancies for a piece on that square
        int occupancyIndicies = 1 << relevantbits;
        // For every possible configuration, the occupancy is set, 
        for (int i = 0; i < occupancyIndicies; i++){
            occupancies[i] = AttackTables.SetOccupancy(i, relevantbits, attackMask);
            attacks[i] = isBishop ? AttackTables.GenerateBishopAttacks(index, occupancies[i]) : AttackTables.GenerateRookAttacks(index, occupancies[i]); 
        }
        for (int count = 0; count < 0xFFFFFF; count++){
            ulong magicNumber = Helper.GetBiasedUlong();
            if (Helper.CountBit((attackMask * magicNumber) & 0xFF00000000000000) < 6) continue; 
            int i = 0;
            bool fail = false;
            for (i = 0; !fail & i < occupancyIndicies; i++){
                int magicIndex = (int)((occupancies[index] * magicNumber) >> (64 - relevantbits));
                if (usedAttacks[magicIndex] == 0ul)
                    usedAttacks[magicIndex] = attacks[index];
                fail = usedAttacks[magicIndex] != attacks[index];
            }
            if (!fail)
                return magicNumber;
        }
        return 0ul;
    }

    public static void InitMagicNumbers(){
    }
}