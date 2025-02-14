using System;

public enum Piece{
    WPawn = 0, WBishop = 1, WKnight = 2, WRook = 3, WQueen = 4, WKing = 5,
    BPawn = 6, BBishop = 7, BKnight = 8, BRook = 9, BQueen = 10, BKing = 11
}

public struct Board{
    public const int BitboardCount = 12;
    private string _FENPosition;
    public ulong[] Bitboards;

    public Board(string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"){
        Bitboards = new ulong[BitboardCount];
        for (int i = 0; i < BitboardCount; i++)
            Bitboards[i] = ulong.MinValue;
        _FENPosition = FEN;
        ParseFENString();
    }

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

    private void ParseFENString(){
        int index = 63;
        for(int i = 0; i < _FENPosition.Length; i++){
            char letter = _FENPosition[i];
            if(char.IsLetter(letter)){
                Bitboards[(int)ValueSwitch(letter)] |= 1ul << index;
                index--;
            }
            else if (char.IsDigit(letter))
                index -= (int)Char.GetNumericValue(letter);
        }
    }
}
