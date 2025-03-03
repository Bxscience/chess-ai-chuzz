using System;
    
public struct Board{
    private const string _DefaultPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public const int BitboardCount = 12;
    public const int OccupanciesCount = 3;
    public const int BoardSize = 8;
    private readonly string _FENPosition;
    public Side PlayerTurn;
    public ulong[] Bitboards;
    public ulong[] Occupancies;

    public Board(string FEN){
        PlayerTurn = Side.White;
        Bitboards = new ulong[BitboardCount];
        for (int i = 0; i < BitboardCount; i++)
            Bitboards[i] = ulong.MinValue;
        _FENPosition = FEN == "" ? _DefaultPosition : FEN;
        Occupancies = new ulong[OccupanciesCount];
        ParseFENString();
        Occupancies[(int)Side.White] = Bitboards[(int)Piece.WPawn] | Bitboards[(int)Piece.WBishop] | Bitboards[(int)Piece.WKnight] | 
                    Bitboards[(int)Piece.WRook] | Bitboards[(int)Piece.WQueen] | Bitboards[(int)Piece.WKing];
        Occupancies[(int)Side.Black] = Bitboards[(int)Piece.BPawn] | Bitboards[(int)Piece.BBishop] | Bitboards[(int)Piece.BKnight] | 
                    Bitboards[(int)Piece.BRook] | Bitboards[(int)Piece.BQueen] | Bitboards[(int)Piece.BKing];
        Occupancies[(int)Side.Both] = Occupancies[(int)Side.White] | Occupancies[(int)Side.Black];
    }

    public void TestBoard(){
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
