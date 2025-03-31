/* Encoding Moves in a integer:                                             Hexadecimal to decode values:
0000 0000 0000 0000 0011 1111       src square (6 bits)                     0x3f
0000 0000 0000 1111 1100 0000       dest square (6 bits)                    0xfc0
0000 0000 1111 0000 0000 0000       piece (4 bits)                          0xf000
0000 1111 0000 0000 0000 0000       promoted piece (4 bits)                 0xf0000
0001 0000 0000 0000 0000 0000       capture flag (1 bit)                    0x100000
0010 0000 0000 0000 0000 0000       double push flag (1 bit)                0x200000
0100 0000 0000 0000 0000 0000       enpassant flag (1 bit)                  0x400000
1000 0000 0000 0000 0000 0000       castling flag (1 bit)                   0x800000
when move == 0, the move is null
*/
using System;

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
    private const ulong _FirstRank = 0xFF, _SecondRank = 0xFF00, _SeventhRank = 0xFF000000000000, _EighthRank = 0xFF00000000000000;
    
    // A ulong holding the available castling for all sides
    //private static ulong _WKing = 0x9, _WQueen = 0x88, _BKing = 0x900000000000000 , _BQueen = 0x8800000000000000;

    // Holds a bitmap for all squares that are attacked by each board, includes pieces occupied by other white pieces
    public static ulong[] AttackedSquares = {0ul, 0ul};

    // Given a board struct, generates a bitboard of all attacked squares in the position
    public static void InitAttackMap(Board board){
        for (int piece = 0; piece < Board.BitboardCount; piece++){
            int side = piece / 6;
            ulong bitboard = board.Bitboards[piece];
            int bits = Helper.CountBit(bitboard);
            for (int iternator = 0; iternator < bits; iternator++){
                int srcSquare = Helper.LSBIndex(bitboard);
                Helper.PopBit(ref bitboard, srcSquare);
                switch(piece){
                    case 0:
                        AttackedSquares[side] |= AttackTables.PawnAttacks[side, srcSquare];
                        break;
                    case 6:
                        AttackedSquares[side] |= AttackTables.PawnAttacks[side, srcSquare];
                        break;
                    case 1: case 7:
                        AttackedSquares[side] |= AttackTables.GetBishopAttacks(srcSquare, board.Occupancies[(int)Side.Both]);
                        break;
                    case 2: case 8:
                        AttackedSquares[side] |= AttackTables.KnightAttacks[srcSquare];
                        break;
                    case 3: case 9:
                        AttackedSquares[side] |= AttackTables.GetRookAttacks(srcSquare, board.Occupancies[(int)Side.Both]);
                        break;
                    case 4: case 10:
                        AttackedSquares[side] |= AttackTables.GetQueenAttacks(srcSquare, board.Occupancies[(int)Side.Both]);
                        break;
                    case 5: case 11:
                        AttackedSquares[side] |= AttackTables.KingAttacks[srcSquare];
                        break;
                }
            }
        }
    }

    // Updates attack map based on new information from move
    public static void UpdateAttackMap(int move){}

    // Creates an int array storing possible moves while determining legal moves
    public static int[] InitMoves(Board board, Side side){
        int[] moveList = new int[256];
        int moveIndex = 0;
        if (IsInCheck(board, side)){
            //TODO: generate moves when under check
            return moveList;
        }
        int offset = side == Side.Black ? 6 : 0;
        for (int piece = offset; piece < 6 + offset; piece++){
            ulong pieceBitboard = board.Bitboards[piece];
            int bits = Helper.CountBit(pieceBitboard);
            for (int count = 0; count < bits; count++){
                ulong attacks;
                int possibleAttacks, enpassant;
                int src = Helper.LSBIndex(pieceBitboard);
                Helper.PopBit(ref pieceBitboard, src);
                switch ((Piece)piece){
                    case Piece.WPawn:
                        // Checking whether the pawn is on a promotion square, if so then adds promotion moves
                        if (Helper.CheckBit(1ul << src, _SeventhRank, src)){
                            int dest = src + 8;
                            int moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.WQueen,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                            moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.WRook,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                            moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.WBishop,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                            moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.WKnight,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                        }
                        // Handles single pawn pushes
                        else if (Helper.GetBit(board.Occupancies[(int)Side.Both], src + 8) == 0){
                            int wMove = Move.EncodeMove(src, src + 8,
                                            (Piece)piece, Piece.noPiece,
                                            false, false, false, false);   
                            moveList[moveIndex++] = wMove;
                            // Handles double pawn pushes
                            if (Helper.CheckBit(1ul << src, _SecondRank, src) && Helper.GetBit(board.Occupancies[(int)Side.Both], src + 16) == 0){
                                    int move = Move.EncodeMove(src, src + 16,
                                                    (Piece)piece, Piece.noPiece,
                                                    false, true, false, false);
                                    moveList[moveIndex++] = move;
                            }
                        }

                        // Handles pawn captures
                        attacks = AttackTables.PawnAttacks[(int)side, src] & board.Occupancies[(int)Helper.GetOpponent(side)];
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (Helper.CheckBit(1ul << src, _SeventhRank, src)){
                                int moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.WQueen,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                                moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.WRook,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                                moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.WBishop,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                                moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.WKnight,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                            } else {
                                int moves = Move.EncodeMove(src, dest,
                                                (Piece)piece, Piece.noPiece,
                                                true, false, false, false);

                                moveList[moveIndex++] = moves;                
                            }
                        }

                        // Enpassants for the white side (If black did a double pawn push, then white can enpassant)
                        enpassant = (int)board.Enpassant;
                        if (board.PlayerTurn == Side.White && (src << 7 == enpassant || src << 9 == enpassant)){
                            int moves = Move.EncodeMove(src, enpassant,
                                            (Piece)piece, Piece.noPiece,
                                            true, false, true, false);
                            moveList[moveIndex++] = moves;
                        }
                        break;
                    case Piece.BPawn:
                        // Checking whether the pawn is on a promotion square, if so then adds promotion moves
                        if (Helper.CheckBit(1ul << src, _SecondRank, src)){
                            int dest = src - 8;
                            int moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.BQueen,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                            moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.BRook,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                            moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.BBishop,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                            moves = Move.EncodeMove(src, dest,
                                           (Piece)piece, Piece.BKnight,
                                           false, false, false, false);
                            moveList[moveIndex++] = moves;
                        }
                        // Handles single pawn pushes
                        else if (Helper.GetBit(board.Occupancies[(int)Side.Both], src - 8) == 0){
                            int bMove = Move.EncodeMove(src, src - 8,
                                            (Piece)piece, Piece.noPiece,
                                            false, false, false, false);   
                            moveList[moveIndex++] = bMove;
                            // Handles double pawn pushes
                            if (Helper.CheckBit(1ul << src, _SeventhRank, src) && Helper.GetBit(board.Occupancies[(int)Side.Both], src - 16) == 0){
                                    int move = Move.EncodeMove(src, src - 16,
                                                    (Piece)piece, Piece.noPiece,
                                                    false, true, false, false);
                                    moveList[moveIndex++] = move;
                            }
                        }

                        // Handles pawn captures
                        attacks = AttackTables.PawnAttacks[(int)side, src] & board.Occupancies[(int)Helper.GetOpponent(side)];
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (Helper.CheckBit(1ul << src, _SecondRank, src)){
                                int moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.BQueen,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                                moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.BRook,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                                moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.BBishop,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                                moves = Move.EncodeMove(src, dest,
                                            (Piece)piece, Piece.BKnight,
                                            true, false, false, false);

                                moveList[moveIndex++] = moves;
                            } else {
                                int moves = Move.EncodeMove(src, dest,
                                                (Piece)piece, Piece.noPiece,
                                                true, false, false, false);

                                moveList[moveIndex++] = moves;                
                            }
                        }
                        // Enpassants for the white side (If black did a double pawn push, then white can enpassant)
                        enpassant = (int)board.Enpassant;
                        if (board.PlayerTurn == Side.White && (src >> 7 == enpassant || src >> 9 == enpassant)){
                            int moves = Move.EncodeMove(src, enpassant,
                                            (Piece)piece, Piece.noPiece,
                                            true, false, true, false);
                            moveList[moveIndex++] = moves;
                        }
                        break;
                    case Piece.BBishop: case Piece.WBishop:
                        attacks = AttackTables.GetBishopAttacks(src, board.Occupancies[(int)Side.Both]);
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (!Helper.CheckBit(board.Occupancies[(int)side], attacks, dest))
                                continue;
                            int move = Move.EncodeMove(src, dest, 
                                            (Piece)piece, Piece.noPiece, 
                                            Helper.GetBit(board.Occupancies[(int)Helper.GetOpponent(side)], dest) == 1, false, 
                                            false, false);
                            moveList[moveIndex++] = move;
                        }
                        break;
                    case Piece.BKnight: case Piece.WKnight:
                        attacks = AttackTables.KnightAttacks[src];
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (!Helper.CheckBit(board.Occupancies[(int)side], attacks, dest))
                                continue;
                            int move = Move.EncodeMove(src, dest, 
                                            (Piece)piece, Piece.noPiece, 
                                            Helper.GetBit(board.Occupancies[(int)Helper.GetOpponent(side)], dest) == 1, false, 
                                            false, false);
                            moveList[moveIndex++] = move;
                        }
                        break;
                    case Piece.WRook: case Piece.BRook:
                        attacks = AttackTables.GetRookAttacks(src, board.Occupancies[(int)Side.Both]);
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (!Helper.CheckBit(board.Occupancies[(int)side], attacks, dest))
                                continue;
                            int move = Move.EncodeMove(src, dest, 
                                            (Piece)piece, Piece.noPiece, 
                                            Helper.GetBit(board.Occupancies[(int)Helper.GetOpponent(side)], dest) == 1, false, 
                                            false, false);
                            moveList[moveIndex++] = move;
                        }
                        break;
                    case Piece.BQueen: case Piece.WQueen:
                        attacks = AttackTables.GetQueenAttacks(src, board.Occupancies[(int)Side.Both]);
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (!Helper.CheckBit(board.Occupancies[(int)side], attacks, dest))
                                continue;
                            int move = Move.EncodeMove(src, dest, 
                                            (Piece)piece, Piece.noPiece, 
                                            Helper.GetBit(board.Occupancies[(int)Helper.GetOpponent(side)], dest) == 1, false, 
                                            false, false);
                            moveList[moveIndex++] = move;
                        }
                        break;
                    case Piece.BKing: case Piece.WKing:
                        // TODO: Implement Castling Moves
                        attacks = AttackTables.KingAttacks[src];
                        possibleAttacks = Helper.CountBit(attacks);
                        for (int iternator = 0; iternator < possibleAttacks; iternator++){
                            int dest = Helper.LSBIndex(attacks);
                            Helper.PopBit(ref attacks, dest);
                            if (!Helper.CheckBit(board.Occupancies[(int)side], attacks, dest))
                                continue;
                            int move = Move.EncodeMove(src, dest, 
                                            (Piece)piece, Piece.noPiece, 
                                            Helper.GetBit(board.Occupancies[(int)Helper.GetOpponent(side)], dest) == 1, false, 
                                            false, false);
                            moveList[moveIndex++] = move;
                        }
                        break;
                }
            }
        }
        if (side == Side.White){
            if (((short)board.Rights & (short)CastlingRights.wk) > 0) moveList[moveIndex++] = CastlingMove(board, CastlingRights.wk);
            if (((short)board.Rights & (short)CastlingRights.wq) > 0) moveList[moveIndex++] = CastlingMove(board, CastlingRights.wq);
        } else if (side == Side.Black){
            if (((short)board.Rights & (short)CastlingRights.bk) > 0) moveList[moveIndex++] = CastlingMove(board, CastlingRights.bk);
            if (((short)board.Rights & (short)CastlingRights.bq) > 0) moveList[moveIndex++] = CastlingMove(board, CastlingRights.bq);
        }
        return moveList;
    }

    // Checks whether the given side is in check
    public static bool IsInCheck(Board board, Side side){
        ulong kingBitboard = board.Bitboards[(int)(side == Side.Black ? Piece.BKing : Piece.WKing)];
        if (kingBitboard == 0) return false;
        int index = Helper.LSBIndex(kingBitboard);
        return Helper.CheckBit(kingBitboard, AttackedSquares[(int)(side == Side.Black ? Side.White : Side.Black)], index);
    }

    // Sorts the moves in a array of moves, given the property to be searched and the value
    public static int[] SortMoves(int[] inputMoves, Properties p, int value){
        int[] moves = new int[256];
        int moveIndex = 0;
        switch (p){
            case Properties.src:
                foreach (int move in inputMoves){
                    if (Move.GetSrcSquare(move) == value)
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.dest:
                foreach (int move in inputMoves){
                    if (Move.GetDestSquare(move) == value)
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.piece:
                foreach (int move in inputMoves){
                    if (Move.GetPiece(move) == (Piece)value)
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.promotedPiece:
                foreach (int move in inputMoves){
                    if (Move.GetPromotedPiece(move) == (Piece)value)
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.capture:
                foreach (int move in inputMoves){
                    if (Move.IsCapture(move) == (value == 1 ? true : false))
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.doublePush:
                foreach (int move in inputMoves){
                    if (Move.IsPush(move) == (value == 1 ? true : false))
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.enpassant:
                foreach (int move in inputMoves){
                    if (Move.IsEnpassant(move) == (value == 1 ? true : false))
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
            case Properties.castling:
                foreach (int move in inputMoves){
                    if (Move.IsCastle(move) == (value == 1 ? true : false))
                        moves[moveIndex++] = move;
                    else if (move == 0) break;
                }
                break;
        }
        return moves;
    }

    // If the castle flag is available, checks the conditions required for castling
    private static int CastlingMove(Board board, CastlingRights right){
        // Castle Mask is the squares that need to be clear for castling to occur
        // Relevant Rank is rank that matters for the provided parameter
        // Irrelevant Sq is the square that doesn't matter if its in check
        ulong castleMask, irrelevantSq, relevantRank = board.Occupancies[(int)Side.Both];
        switch(right){
            case CastlingRights.wk:
                relevantRank &= _FirstRank;
                castleMask = 0x6;
                if ((relevantRank & castleMask) == 0 && (AttackedSquares[(int)Side.White] & castleMask) == 0) 
                    return Move.EncodeMove((int)Square.e1, (int)Square.g1, Piece.WKing, Piece.noPiece, false, false, false, true);
                break;
            case CastlingRights.wq:
                relevantRank &= _FirstRank;
                castleMask = 0x70;
                irrelevantSq = 0x40;
                if ((relevantRank & castleMask) == 0 && (AttackedSquares[(int)Side.White] & (castleMask ^ irrelevantSq)) == 0)
                    return Move.EncodeMove((int)Square.e1, (int)Square.c1, Piece.WKing, Piece.noPiece, false, false, false, true);
                break;
            case CastlingRights.bk:
                relevantRank &= _SecondRank;
                castleMask = 0x600000000000000;
                if ((relevantRank & castleMask) == 0 && (AttackedSquares[(int)Side.Black] & castleMask) == 0)
                    return Move.EncodeMove((int)Square.e8, (int)Square.g8, Piece.BKing, Piece.noPiece, false, false, false, true);
                break;
            case CastlingRights.bq:
                relevantRank &= _SecondRank;
                castleMask = 0x7000000000000000;
                irrelevantSq = 0x4000000000000000;
                if ((relevantRank & castleMask) == 0 && (AttackedSquares[(int)Side.White] & castleMask ^ irrelevantSq) == 0)
                    return Move.EncodeMove((int)Square.e8, (int)Square.c8, Piece.BKing, Piece.noPiece, false, false, false, true);
                break;
            default: throw new System.Exception("Invalid Castling Flag!");
        }
        return 0;
    }
}

