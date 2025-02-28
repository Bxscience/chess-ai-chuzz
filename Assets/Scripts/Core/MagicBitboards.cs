public struct MagicBitboards{
    // TODO: Understand this function fully and fix if needed
    public static ulong FindMagicNumber(int index, int relevantbits, bool isBishop){
        // Occupancies represent different positions of blocks & available moves
        int maxLength = isBishop ? 512 : 4096;
        ulong[] occupancies = new ulong[maxLength], 
        // Attacks represent all possible attacks 
                attacks = new ulong[maxLength], 
                usedAttacks = new ulong[maxLength];
        for (int i = 0; i < maxLength; i++) usedAttacks[i] = 0ul;
        ulong attackMask = isBishop ? AttackTables.MaskBishopAttacks(index) : AttackTables.MaskRookAttacks(index);
        int occupancyIndicies = 1 << relevantbits;
        for (int i = 0; i < occupancyIndicies; i++){
            occupancies[index] = AttackTables.SetOccupancy(index, relevantbits, attackMask);
            attacks[index] = isBishop ? AttackTables.GenerateBishopAttacks(index, occupancies[index]) : AttackTables.GenerateRookAttacks(index, occupancies[index]); 
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