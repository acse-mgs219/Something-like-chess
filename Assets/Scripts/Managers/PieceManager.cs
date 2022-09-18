using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEssentials.Extensions;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PieceManager : MonoBehaviour
{
    public static PieceManager instance;

    private List<ScriptablePiece> _pieces;
    private ObservableCollection<Tile> _highlightedTiles;

    private Chesspiece _selectedPiece;
    public Chesspiece SelectedPiece => _selectedPiece;

    private void Awake()
    {
        instance = this;

        _highlightedTiles = new ObservableCollection<Tile>();
        _highlightedTiles.CollectionChanged += HighlightedTilesChanged;

        _pieces = Resources.LoadAll<ScriptablePiece>($"Pieces").ToList();
    }

    public void SetSelectedPiece(Chesspiece Piece)
    {
        _selectedPiece = Piece;

        _highlightedTiles.RemoveIf(ht => (_selectedPiece?.LegalMoves?.Contains(ht) ?? false) == false);

        if (_selectedPiece != null)
        {
            foreach (Tile tile in _selectedPiece.LegalMoves)
            {
                if (_highlightedTiles.Contains(tile) == false)
                {
                    _highlightedTiles.Add(tile);
                }
            }
        }
    }

    public void HighlightedTilesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (Tile tile in e?.OldItems ?? new List<Tile>())
        {
            tile.SetIndicator(false);
        }

        foreach (Tile tile in e?.NewItems ?? new List<Tile>())
        {
            tile.SetIndicator(true, _selectedPiece);
        }
    }

    public void SpawnPieceAtTile(Player player, PieceType type, Tile tile)
    {
        ScriptablePiece Piece = GetPieceOfType(type);
        Chesspiece instancePiece = Instantiate(Piece.Piece);
        instancePiece.Type = type;

        instancePiece.Init(player, tile);
    }

    public void CreateCopyOfPiece(Player player, PieceType type, Tile tile)
    {
        ScriptablePiece Piece = GetPieceOfType(type);
        Chesspiece instancePiece = Instantiate(Piece.Piece);


    }

    public ScriptablePiece GetPieceOfType(PieceType type)
    {
        ScriptablePiece piece = _pieces.First(p => p.name == type.ToString());
        return piece;
    }
}

public enum PieceType
{
    Rook,
    Knight,
    Bishop,
    King,
    Queen,
    Pawn
}
