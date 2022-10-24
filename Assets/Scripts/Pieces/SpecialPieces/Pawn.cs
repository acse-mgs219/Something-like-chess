using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEssentials.Extensions;

public class Pawn : Chesspiece
{
    public List<PieceType> PromotableTypes;
    public List<Tile> EnPassantTiles => _legalMoves.Where(m => m.EnPassant).Select(m => m.ToTile).ToList();

    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 0, _player.PawnMovementDirection, maxMoves: HasMoved ? 1 : 2, canCapture: false, canPromote: true));
        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 1, _player.PawnMovementDirection, maxMoves: 1, xMirror: true, mustCapture: true, canPromote: true));
    }

    public override void CalculateLegalMoves(Tile[,] grid)
    {
        base.CalculateLegalMoves(grid);
        FillEnPassantMoves(grid);
    }

    public void FillEnPassantMoves(Tile[,] grid)
    {
        int currentY = _tile.Y;

        if (currentY <= 1 || currentY >= GridManager.instance.Height - 2)
        {
            return;
        }

        List<Tile> tilesToConsider = new List<Tile>();

        if (_tile.X > 0)
        {
            tilesToConsider.Add(grid[_tile.X - 1, _tile.Y]);
        }
        if (_tile.X < GridManager.instance.Width - 1)
        {
            tilesToConsider.Add(grid[_tile.X + 1, _tile.Y]);
        }

        List<Chesspiece> neighboringPawns = tilesToConsider.Select(t => t.OccupyingPiece).Where(
            p => p != null 
            && p is Pawn 
            && p.Player != Player
        ).ToList();
        if (neighboringPawns.Count == 0)
        {
            return;
        }

        foreach (Pawn pawn in neighboringPawns)
        {
            List<Tile> historyTiles = pawn.History?.OccupiedTiles ?? new List<Tile>();
            if (historyTiles.Count < 2)
            {
                continue;
            }

            int lastY = historyTiles[historyTiles.Count - 2].Y;

            if (Math.Abs(currentY - lastY) == 2 && PlayerManager.instance.HasPlayer2MovedSincePlayer1Turn(pawn.Player, _player, pawn.History.MoveTurns[historyTiles.Count - 1]) == false)
            {
                Move enPassant = new Move(GridManager.instance.GetTileAtPosition(pawn._tile.X, (currentY + lastY) / 2), _tile, pawn, enPassant: true);
                _legalMoves.Add(enPassant);
            }
        }
    }

    public void Promote(PieceType type)
    {
        ScriptablePiece Piece = PieceManager.instance.GetPieceOfType(type);
        Chesspiece instancePiece = Instantiate(Piece.Piece);
        instancePiece.Init(_player, _tile);

        _player.PromotionDummies.ForEach(d => Destroy(d.gameObject));
        _player.PromotionDummies.Clear();
        AssociatedMove.FinishRealMove();
        Destroy();
    }
}
