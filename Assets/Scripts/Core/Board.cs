using System;
using System.Collections.Generic;

public struct Board{
    private const string _DefaultPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const int BitboardCount = 12;
    public const int OccupanciesCount = 3;
    public const int BoardSize = 8;
    private readonly string _FENPosition;
    public Side PlayerTurn;
    public Square Enpassant;
    public ulong[] Bitboards;
    public ulong[] Occupancies;
    public Stack<int> History;
    public CastlingRights Rights;

    public Board(string FEN){
        Bitboards = new ulong[BitboardCount];
        Occupancies = new ulong[OccupanciesCount];
        _FENPosition = (FEN == "") ? _DefaultPosition : FEN;
        PlayerTurn = Side.Both;
        History = new Stack<int>();
        Enpassant = Square.noSq;
        Rights = 0;
        ParseFENString();
        Occupancies[(int)Side.White] = Bitboards[(int)Piece.WPawn] | Bitboards[(int)Piece.WBishop] | Bitboards[(int)Piece.WKnight] | 
                    Bitboards[(int)Piece.WRook] | Bitboards[(int)Piece.WQueen] | Bitboards[(int)Piece.WKing];
        Occupancies[(int)Side.Black] = Bitboards[(int)Piece.BPawn] | Bitboards[(int)Piece.BBishop] | Bitboards[(int)Piece.BKnight] | 
                    Bitboards[(int)Piece.BRook] | Bitboards[(int)Piece.BQueen] | Bitboards[(int)Piece.BKing];
        Occupancies[(int)Side.Both] = Occupancies[(int)Side.White] | Occupancies[(int)Side.Black];
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

        // Parses castling rights
        if (FENPartitions[2][0] == '-') Rights = 0;
        else{
            for (int i = 0; i < FENPartitions[2].Length; i++){
                switch(FENPartitions[2][i]){
                    case 'K': Rights |= CastlingRights.wk; break;
                    case 'Q': Rights |= CastlingRights.wq; break;
                    case 'k': Rights |= CastlingRights.bk; break;
                    case 'q': Rights |= CastlingRights.bq; break;
                    default: throw new Exception("Invalid FEN String");
                }
            }
        }

        // Holds Enpassant Square
        Square result;
        if (Enum.TryParse(FENPartitions[3], out result)) Enpassant = result;
        else if (FENPartitions[3][0] == '-') Enpassant = Square.noSq;
        else throw new Exception("Invalid FEN String");

        // TODO: Half Move Clock
        // TODO: Full Move Counter
    }

    public void MakeMove(int move){

    }

}