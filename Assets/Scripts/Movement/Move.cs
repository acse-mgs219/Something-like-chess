using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public Tile ToTile;

    private Tile _fromTile;
    public Tile FromTile => _fromTile;

    private Chesspiece _targetPiece;
    public Chesspiece TargetPiece => _targetPiece;

    private bool _enPassant;
    public bool EnPassant => _enPassant;

    private bool _castle;
    public bool Castle => _castle;

    public Move(Tile toTile, Tile fromTile, Chesspiece targetPiece = null, bool enPassant = false, bool castle = false)
    {
        ToTile = toTile;
        _fromTile = fromTile;
        _targetPiece = targetPiece;
        _enPassant = enPassant;
        _castle = castle;
    }
}
