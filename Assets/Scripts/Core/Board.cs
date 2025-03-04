using System;
    
public struct Board{
    private const string _DefaultPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const int BitboardCount = 12;
    public const int OccupanciesCount = 3;
    public const int BoardSize = 8;
    private readonly string _FENPosition;
    public Side PlayerTurn;
    public int Enpassant;
    public ulong[] Bitboards;
    public ulong[] Occupancies;

    public Board(string FEN){
        Bitboards = new ulong[BitboardCount];
        Occupancies = new ulong[OccupanciesCount];
        _FENPosition = (FEN == "") ? _DefaultPosition : FEN;
        PlayerTurn = Side.White;
        Enpassant = -1;
        ParseFENString();
        Occupancies[(int)Side.White] = Bitboards[(int)Piece.WPawn] | Bitboards[(int)Piece.WBishop] | Bitboards[(int)Piece.WKnight] | 
                    Bitboards[(int)Piece.WRook] | Bitboards[(int)Piece.WQueen] | Bitboards[(int)Piece.WKing];
        Occupancies[(int)Side.Black] = Bitboards[(int)Piece.BPawn] | Bitboards[(int)Piece.BBishop] | Bitboards[(int)Piece.BKnight] | 
                    Bitboards[(int)Piece.BRook] | Bitboards[(int)Piece.BQueen] | Bitboards[(int)Piece.BKing];
        Occupancies[(int)Side.Both] = Occupancies[(int)Side.White] | Occupancies[(int)Side.Black];
    }

    public void TestBoard(){
        Helper.SetBit(ref Bitboards[(int)Piece.BPawn], (int)Square.e4);
        Helper.SetBit(ref Bitboards[(int)Piece.BPawn], (int)Square.f3);

        Helper.PrintBitboard(Bitboards[(int)Piece.BPawn]);

        Helper.PrintBitboard(AttackTables.PawnAttacks[(int)Side.Black, (int)Square.e4]);

        UnityEngine.Debug.Log(AttackTables.IsSquareAttacked((int)Square.d3, Bitboards, Occupancies, Side.White));
        UnityEngine.Debug.Log(AttackTables.IsSquareAttacked((int)Square.f3, Bitboards, Occupancies, Side.White));
        UnityEngine.Debug.Log(AttackTables.IsSquareAttacked((int)Square.d3, Bitboards, Occupancies, Side.Black));
        UnityEngine.Debug.Log(AttackTables.IsSquareAttacked((int)Square.f3, Bitboards, Occupancies, Side.Black));
    }

    // Represents all values in FEN String as Piece Enums for easy transversal
    private Piece ValueSwitch(char c) => c switch{
        'P' => Piece.WPawn,
        'B' => Piece.WBishop,
        'N' => Piece.WKnight,
        'R' => Piece.WRook,
        'Q' => Piece.WQueen,
        'K' => Piece.WKing,

        'p' => Piece.BPawn,
        'b' => Piece.BBishop,
        'n' => Piece.BKnight,
        'r' => Piece.BRook,
        'q' => Piece.BQueen,
        'k' => Piece.BKing,
        _ => throw new Exception("Invalid FEN string!")
    };

    // Parses the FEN String and updates revelant bitboards with pieces
    private void ParseFENString(){
        // 0 => Board Position
        // 1 => Turn to Move
        // 2 => Castle Rights
        // 3 => En Passant Square
        // 4 => Half Move Clock
        // 5 => Full Move Counter
        string[] FENPartitions = _FENPosition.Split(" ");

        // Parses Position FEN
        for(int i = 0, index = 63; i < FENPartitions[0].Length; i++){
            char letter = FENPartitions[0][i];
            if(char.IsLetter(letter)){
                Helper.SetBit(ref Bitboards[(int)ValueSwitch(letter)], index);
                index--;
            }
            else if (char.IsDigit(letter))
                index -= (int)Char.GetNumericValue(letter);
        }

        // Updates Turn to Move
        PlayerTurn = FENPartitions[1] == "w" ? Side.White : Side.Black;

        // TODO: Castling Rights
        
        // Holds Enpassant Square
        Square result;
        if (Enum.TryParse(FENPartitions[3], out result)) Enpassant = (int)result;
        else if (FENPartitions[3][0] == '-') Enpassant = -1;
        else throw new Exception("Invalid FEN String");

        // TODO: Half Move Clock
        // TODO: Full Move Counter
    }
}

// Enum to store index of bitboards
public enum Piece{
    WPawn, WBishop, WKnight, WRook, WQueen, WKing,
    BPawn, BBishop, BKnight, BRook, BQueen, BKing
}

// Enum for side to move
public enum Side{
    Black = 0, White = 1, Both = 2
}

// Enum Defining Squares to Bitboard Indexes used for move generation
public enum Square{
    h1, g1, f1, e1, d1, c1, b1, a1,
    h2, g2, f2, e2, d2, c2, b2, a2,
    h3, g3, f3, e3, d3, c3, b3, a3,
    h4, g4, f4, e4, d4, c4, b4, a4,
    h5, g5, f5, e5, d5, c5, b5, a5,
    h6, g6, f6, e6, d6, c6, b6, a6,
    h7, g7, f7, e7, d7, c7, b7, a7,
    h8, g8, f8, e8, d8, c8, b8, a8

}

// Relating to castling
// 0001 = White King can Castle Kingside
// 0010 = White King can Castle Queenside
// 0100 = Black King can Castle Kingside
// 1000 = Black King can Castle Queenside
[Flags]
public enum CastlingRights{
    wk = 1, wq = 2, bk = 4, bq = 8
}