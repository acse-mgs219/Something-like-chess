using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class King : Chesspiece
{
    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 1, 0, maxMoves: 1, xMirror: true));
        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 0, 1, maxMoves: 1, yMirror: true));
        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 1, 1, maxMoves: 1, xMirror: true, yMirror: true));
    }

    public override void CalculateLegalMoves(Tile[,] grid)
    {
        base.CalculateLegalMoves(grid);
        CheckCastleMoves(grid);
    }

    public void CheckCastleMoves(Tile[,] grid)
    {
        if (HasMoved || _player.IsInCheck)
        {
            return;
        }

        CheckCastle(grid, shortCastle: true);
        CheckCastle(grid, shortCastle: false);
    }

    public void CheckCastle(Tile[,] grid, bool shortCastle)
    {
        int x = _tile.X;

        Tile castleTarget = null;

        Func<int, bool> endCondition = (x => shortCastle ? x <= GridManager.instance.Width - 1 : x >= 0);

        while (endCondition(shortCastle ? ++x : --x))
        {
            Tile tileToCheck = grid[x, _tile.Y];
            Chesspiece potentialRook = tileToCheck.OccupyingPiece;

            if (potentialRook != null)
            {
                if (potentialRook.Type == PieceType.Rook
                    && potentialRook.Player == Player
                    && potentialRook.HasMoved == false
                    && castleTarget != null
                )
                {
                    Move castle = new Move(castleTarget, _tile, potentialRook, castle: true);
                    _legalMoves.Add(castle);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (tileToCheck.IsTileSafeForPlayer(_player) == false && castleTarget == null)
            {
                return;
            }

            if (tileToCheck.X == _tile.X + (shortCastle ? 2 : -2))
            {
                castleTarget = tileToCheck;
            }
        }
    }
}
