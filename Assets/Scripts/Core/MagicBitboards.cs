public struct MagicBitboards{
    // Not used during runtime, used to pre-generate magic numbers
    public static ulong FindMagicNumber(int square, int relevantbits, bool isBishop){
        // Occupancies represent different positions of blocks & available moves
        // Max length is determined by all possible moves the piece has (9 for bishop, 12 for rook)
        // This determines the max possible different configurations of blockers for a piece
        int maxLength = isBishop ? 512 : 4096;
        // Occupancies represent a specific situation where a 1 represents a blocker on that square
        ulong[] occupancies = new ulong[maxLength], 
                // Attacks is the moves available to the piece after occupancies are accounted for
                attacks = new ulong[maxLength], 
                usedAttacks = new ulong[maxLength];

        // Attack mask is an bitboard where 1 represents the possible places the rook attacks
        ulong attackMask = isBishop ? AttackTables.MaskBishopAttacks(square) : AttackTables.MaskRookAttacks(square);

        // Occupancy indicies determines the actual possible configurations of blockers a piece on that square has
        // AKA the amount of different occupancies for a piece on that square
        int occupancyIndicies = 1 << relevantbits;

        // For every possible configuration, the occupancy is set, using the set occupancy method through a given square
        // attacks come from the result of finding occupancies and using the generatepieceattacks method
        for (int index = 0; index < occupancyIndicies; index++){
            occupancies[index] = AttackTables.SetOccupancy(index, relevantbits, attackMask);
            attacks[index] = isBishop ? AttackTables.GenerateBishopAttacks(square, occupancies[index]) : AttackTables.GenerateRookAttacks(square, occupancies[index]); 
        }

        // Generating numbers for a large amount of iterations
        for (int count = 0; count < 0xFFFFFF; count++){
            // Generates a biased ulong as a magic number candidate
            ulong magicNumber = Helper.GetBiasedUlong();
            // Initializes the usedAttacks array and sets all values to zero
            for (int i = 0; i < maxLength; i++) usedAttacks[i] = 0ul;
            // WARNING: Unsure if this condition is needed, try method without
            if (Helper.CountBit((attackMask * magicNumber) & 0xFF00000000000000) < 6) continue; 
            int index;
            bool fail;
            // Testing the magic number, by multiplying the magic number with all the occupancies available on that square
            for (index = 0, fail = false; !fail & index < occupancyIndicies; index++){
                // Hashing function, where for each occupancy is multipled by the magic number leftshifted
                int magicIndex = (int)((occupancies[index] * magicNumber) >> (64 - relevantbits));
                // If the calculated magic index does not collide with a preexisting magic index, map index to hash
                if (usedAttacks[magicIndex] == 0ul)
                    usedAttacks[magicIndex] = attacks[index];
                // If the number does collide, check if the hash is the correct output, if not, return false and generate new magic number
                else if (usedAttacks[magicIndex] != attacks[index])
                    fail = true;
            }
            if (!fail)
                return magicNumber;
        }
        throw new System.Exception("Magic Number not found for " + (Square)square);
    }

    // One time use to get magic numbers
    public static void InitMagicNumbers(){
        string output = "";
        string nonconvert = "";
        for (int i = 0; i < 64; i++){
            if (i % 8 == 0 && i > 0)
                output += "\n";
            ulong number = FindMagicNumber(i, Pregen.BishopRelevantBits[i], true);
            nonconvert += number + ", ";
            output += string.Format("0x{0:x4}", number) + ", ";
        }
        UnityEngine.Debug.Log(nonconvert);
        UnityEngine.Debug.Log(output);
    }
}