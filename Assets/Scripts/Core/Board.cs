using System;

public class Board{
    private const string _DefaultPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public const int BitboardCount = 12;
    public const int BoardSize = 8;
    private readonly string _FENPosition;
    public bool Iswhiteturn;
    public ulong[] Bitboards;
    public ulong Wpieces, Bpieces;
    public AttackTables Attacks;

    public Board(string FEN){
        Iswhiteturn = true;
        Bitboards = new ulong[BitboardCount];
        for (int i = 0; i < BitboardCount; i++)
            Bitboards[i] = ulong.MinValue;
        Wpieces = Bpieces = 0ul;
        _FENPosition = FEN == "" ? _DefaultPosition : FEN;
        Attacks = new AttackTables(this);
        ParseFENString();
        Wpieces = Bitboards[(int)Piece.WPawn] | Bitboards[(int)Piece.WBishop] | Bitboards[(int)Piece.WKnight] | 
                    Bitboards[(int)Piece.WRook] | Bitboards[(int)Piece.WQueen] | Bitboards[(int)Piece.WKing];
        Bpieces = Bitboards[(int)Piece.BPawn] | Bitboards[(int)Piece.BBishop] | Bitboards[(int)Piece.BKnight] | 
                    Bitboards[(int)Piece.BRook] | Bitboards[(int)Piece.BQueen] | Bitboards[(int)Piece.BKing];
    }

    public void TestBoard(){
        Helper.PrintBitboard(AttackTables.MaskPawnAttacks(7));
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
        int index = 63;
        for(int i = 0; i < _FENPosition.Length; i++){
            char letter = _FENPosition[i];
            if(char.IsLetter(letter)){
                Helper.SetBit(ref Bitboards[(int)ValueSwitch(letter)], index);
                index--;
            }
            else if (char.IsDigit(letter))
                index -= (int)Char.GetNumericValue(letter);
        }
    }
}
