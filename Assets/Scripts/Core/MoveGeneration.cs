public struct MoveGeneration{
    private const ulong BRank = 0x000000000000FF00;
    // Holds a bitmap for all squares that are attacked by each board, includes pieces occupied by other white pieces
    public static ulong[] AttackedSquares = {0ul, 0ul};

    // Given a board struct, generates a bitboard of all attacked squares in the position
    public static void InitAttackedSquares(Board board){
        int offset = board.PlayerTurn == Side.White ? 0 : 6;
        for (int count = offset; count < 6 + offset; count++){
            ulong bitboard = board.Bitboards[count];
            int bits = Helper.CountBit(bitboard);
            for (int iternator = 0; iternator < bits; iternator++){
                int index = Helper.LSBIndex(bitboard);
                Helper.PopBit(ref bitboard, index);
                switch(count){
                    case 0:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.PawnAttacks[(int)board.PlayerTurn, index];
                        break;
                    case 6:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.PawnAttacks[(int)board.PlayerTurn, index];
                        break;
                    case 1: case 7:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.GetBishopAttacks(index, board.Occupancies[(int)Side.Both]);
                        break;
                    case 2: case 8:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.KnightAttacks[index];
                        break;
                    case 3: case 9:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.GetRookAttacks(index, board.Occupancies[(int)Side.Both]);
                        break;
                    case 4: case 10:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.GetQueenAttacks(index, board.Occupancies[(int)Side.Both]);
                        break;
                    case 5: case 11:
                        AttackedSquares[(int)board.PlayerTurn] |= AttackTables.KingAttacks[index];
                        break;
                }
            }
        }
    }

    // TODO: Consider moving source & target to a move struct, figure ou    t pawn pushes
    public static void GenerateMoves(Board board){
        int source, target;
        ulong pieceBitboards, attacks = 0ul;

        for(int index = 0; index < Board.BitboardCount; index++){
            pieceBitboards = board.Bitboards[index];
            switch((Piece)index){
                case Piece.WPawn:
                    ulong Wtemp = ~board.Occupancies[(int)Side.Both] & (pieceBitboards << 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (Wtemp << 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (pieceBitboards << 8);
                    break;
                case Piece.BPawn:
                    ulong Btemp = ~board.Occupancies[(int)Side.Both] & (pieceBitboards >> 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (Btemp >> 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (pieceBitboards >> 8);
                    break;
            }
        }
    }
}
