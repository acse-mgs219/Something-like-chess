using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class TypesOfMovement
{
    public static int DoMove(ref int original, int change)
    {
        original += change;
        return original;
    }

    public static List<Tile> MoveInDirection(Tile[,] grid, Tile tile, Player player, int xMove, int yMove, int maxMoves = -1, bool xMirror = false, bool yMirror = false, bool canCapture = true, bool mustCapture = false, bool stopAtCapture = true)
    {
        int x = tile.X;
        int y = tile.Y;
        List<Tile> legalMoves = new List<Tile>();

        Func<int, int, bool> endCondition = ((x, y) => y >= GridManager.instance.Height || y < 0 || x >= GridManager.instance.Width || x < 0);

        int movesMade = 0;
        while (endCondition(DoMove(ref x, xMove), DoMove(ref y, yMove)) == false && movesMade++ != maxMoves)
        {
            Tile targetTile = grid[x, y];
            Chesspiece piece = targetTile.OccupyingPiece;
            if (piece is null)
            {
                if (mustCapture == false) legalMoves.Add(targetTile);
            }
            else
            {
                if (piece.Player != player)
                {
                    if (canCapture)
                    {
                        legalMoves.Add(targetTile);
                        if (piece.VIP == true)
                        {
                            piece.Player.IsInCheck = true;
                        }
                    }
                }
                if (stopAtCapture) break;
            }
        }

        if (xMirror) legalMoves.AddRange(MoveInDirection(grid, tile, player, -xMove, yMove, maxMoves, false, false, canCapture, mustCapture, stopAtCapture));
        if (yMirror) legalMoves.AddRange(MoveInDirection(grid, tile, player, xMove, -yMove, maxMoves, false, false, canCapture, mustCapture, stopAtCapture));
        if (xMirror && yMirror) legalMoves.AddRange(MoveInDirection(grid, tile, player, -xMove, -yMove, maxMoves, false, false, canCapture, mustCapture, stopAtCapture));

        return legalMoves;
    }

    public static List<Tile> GetLegalMovesForMovementSets(Tile[,] grid, Tile tile, Player player, List<Func<Tile[,], Tile, Player, List<Tile>>> sets)
    {
        List<Tile> legalMoves = new List<Tile>();

        foreach (var movementSet in sets)
        {
            legalMoves.AddRange(movementSet(grid, tile, player));
        }

        return legalMoves;
    }
}
