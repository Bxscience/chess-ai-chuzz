using System;

// Enum to store index of bitboards
public enum Piece{
    WPawn, WBishop, WKnight, WRook, WQueen, WKing,
    BPawn, BBishop, BKnight, BRook, BQueen, BKing, noPiece
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
    h8, g8, f8, e8, d8, c8, b8, a8, noSq

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

public enum Properties{
    src, dest, piece, promotedPiece, capture, doublePush, enpassant, castling
}