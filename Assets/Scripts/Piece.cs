public enum PieceType{
    Pawn, Knight, Bishop, Rook, Queen, King
}

public enum PieceColor{
    White, Black
}

public struct Piece{
    public PieceType type;
    public PieceColor color;

    public Piece(PieceType type, PieceColor color){
        this.type = type;
        this.color = color;
    }
}