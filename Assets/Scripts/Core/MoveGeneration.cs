/* Encoding Moves in a integer:                                             Hexadecimal to decode values:
0000 0000 0000 0000 0011 1111       src square (6 bits)                     0x3f
0000 0000 0000 1111 1100 0000       dest square (6 bits)                    0xfc0
0000 0000 1111 0000 0000 0000       piece (4 bits)                          0xf000
0000 1111 0000 0000 0000 0000       promoted piece (4 bits)                 0xf0000
0001 0000 0000 0000 0000 0000       capture flag (1 bit)                    0x100000
0010 0000 0000 0000 0000 0000       double push flag (1 bit)                0x200000
0100 0000 0000 0000 0000 0000       enpassant flag (1 bit)                  0x400000
1000 0000 0000 0000 0000 0000       castling flag (1 bit)                   0x800000
*/
public struct Move{
    // Converts moves on the board to binary for easier handling
    public static int EncodeMove(int src, int dest, Piece piece, Piece promotedPiece, bool capture, bool doublePush, bool enpassant, bool castling){
        int move = 0;
        move |= src;
        move |= dest << 6;
        move |= (int)piece << 12;
        move |= (int)promotedPiece << 16;
        move |= (capture ? 1 : 0) << 20;
        move |= (doublePush ? 1 : 0) << 21;
        move |= (enpassant ? 1 : 0) << 22;
        move |= (castling ? 1 : 0) << 23;
        return move;
    }
    // Gets the source square of the move
    public static int GetSrcSquare(int move) => move & 0x3f;
    // Gets the destination square of the move
    public static int GetDestSquare(int move) => (move & 0xfc0) >> 6;
    // Gets the piece that moved
    public static Piece GetPiece(int move) => (Piece)((move & 0xf000) >> 12);
    // Gets the piece that promoted
    public static Piece GetPromotedPiece(int move) => (Piece)((move & 0xf0000) >> 16);
    // If the move is a capture
    public static bool IsCapture(int move) => (move & 0x100000) >> 20 == 1;
    // If the move is a double pawn push
    public static bool IsPush(int move) => (move & 0x200000) >> 21 == 1;
    // If the move is a enpassant move
    public static bool IsEnpassant(int move) => (move & 0x400000) >> 22 == 1;
    // If the move is a castling move
    public static bool IsCastle(int move) => (move & 0x800000) >> 23 == 1;

    // Helper function that prints out the move in a human readable way
    public static void PrintMove(int move){
        string output = "";
        output += "Src Square: " +  (Square)GetSrcSquare(move) + "\n";
        output += "Dest Square: " + (Square)GetDestSquare(move) + "\n";
        output += "Piece Moved: " + GetPiece(move) + "\n";
        output += "Promoted Piece: " + GetPromotedPiece(move) + "\n";
        output += "Capture Move?: " + IsCapture(move) + "\n";
        output += "Is Double Push?: " + IsPush(move) + "\n";
        output += "Is Enpassant?: " + IsEnpassant(move) + "\n";
        output += "Is Castling?: " + IsCastle(move) + "\n";
        UnityEngine.Debug.Log(output);
    }
}

public struct MoveGeneration{
    // A constant ulong for ranks
    private const ulong _SecondRank = 0x000000000000FF00, _SeventhRank = 0x0000FF000000000;
    
    // A ulong holding the available castling for all sides
    private static ulong _WKing = 0x9, _WQueen = 0x88, _BKing = 0x900000000000000 , _BQueen = 0x8800000000000000;

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

    // Generates a attack bitboard that holds all attacked squares given a board struct
    /*
    public static void GenerateAttackMap(Board board){
        ulong pieceBitboards, attacks = 0ul;    
        for(int index = 0; index < Board.BitboardCount; index++){
            pieceBitboards = board.Bitboards[index];
            switch((Piece)index){   
                case Piece.WPawn:
                    ulong doublePushPawns = ~board.Occupancies[(int)Side.Both] & ((pieceBitboards & _SecondRank) << 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (doublePushPawns << 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (pieceBitboards << 8);
                    int WbitCount = Helper.CountBit(pieceBitboards);
                    for(int count = 0; count < WbitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.PawnAttacks[(int)board.PlayerTurn, idx] & board.Occupancies[(int)Side.Black];
                    }
                    break;
                case Piece.BPawn:
                    ulong Btemp = ~board.Occupancies[(int)Side.Both] & ((pieceBitboards & _SeventhRank) >> 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (Btemp >> 8);
                    attacks |= ~board.Occupancies[(int)Side.Both] & (pieceBitboards >> 8);
                    int BbitCount = Helper.CountBit(pieceBitboards);
                    for(int count = 0; count < BbitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.PawnAttacks[(int)board.PlayerTurn, idx] & board.Occupancies[(int)Side.Black];
                    }
                    break;
                case Piece.BBishop: case Piece.WBishop:
                    int BishopBitCount = Helper.CountBit(pieceBitboards);
                    for (int count = 0; count < BishopBitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.GetBishopAttacks(idx, board.Occupancies[(int)Side.Both]);
                    }
                    break;
                case Piece.WKnight: case Piece.BKnight:
                    int KnightBitCount = Helper.CountBit(pieceBitboards);
                    for (int count = 0; count < KnightBitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.KnightAttacks[idx];
                    }
                    break;
                case Piece.WRook: case Piece.BRook:
                    int RookBitCount = Helper.CountBit(pieceBitboards);
                    for (int count = 0; count < RookBitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.GetRookAttacks(idx, board.Occupancies[(int)Side.Both]);
                    }
                    break;
                case Piece.WQueen: case Piece.BQueen:
                    int QueenBitCount = Helper.CountBit(pieceBitboards);
                    for (int count = 0; count < QueenBitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.GetQueenAttacks(idx, board.Occupancies[(int)Side.Both]);
                    }
                    break;
                case Piece.WKing: case Piece.BKing:
                    int KingBitCount = Helper.CountBit(pieceBitboards);
                    for (int count = 0; count < KingBitCount; count++){
                        int idx = Helper.LSBIndex(pieceBitboards);
                        Helper.PopBit(ref pieceBitboards, idx);
                        attacks |= AttackTables.KingAttacks[idx];
                    }
                    break;
                default: throw new System.Exception("Invalid Piece!");
            }
        }
    }
    
    // TODO: Update the Attack Maps based on the move passed in
    public static void UpdateAttackMaps(Move move){
    }
    */
}

