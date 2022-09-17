using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class TypesOfMovement
{
    public static List<Tile> VerticalMovement(Tile tile, Player player, bool up)
    {
        int x = tile.X;
        int y = tile.Y;
        Tile[,] tiles = GridManager.instance.Tiles;
        List<Tile> legalMoves = new List<Tile>();

        Func<int, bool> endCondition;

        if (up)
        {
            endCondition = (y => y >= GridManager.instance.Height);
        }
        else 
        {
            endCondition = (y => y < 0);
        }

        while (endCondition(up ? ++y : --y) == false)
        {
            Tile targetTile = tiles[x, y];
            Chesspiece piece = targetTile.OccupyingPiece;
            if (piece is null)
            {
                legalMoves.Add(targetTile);
            }
            else
            {
                if (piece.Player != player)
                {
                    legalMoves.Add(targetTile);
                }
                break;
            }
        }

        return legalMoves;
    }

    public static List<Tile> HorizontalMovement(Tile tile, Player player, bool right)
    {
        int x = tile.X;
        int y = tile.Y;
        Tile[,] tiles = GridManager.instance.Tiles;
        List<Tile> legalMoves = new List<Tile>();

        Func<int, bool> endCondition;

        if (right)
        {
            endCondition = (x => x >= GridManager.instance.Width);
        }
        else
        {
            endCondition = (x => x < 0);
        }

        while (endCondition(right ? ++x : --x) == false)
        {
            Tile targetTile = tiles[x, y];
            Chesspiece piece = targetTile.OccupyingPiece;

            if (piece is null)
            {
                legalMoves.Add(targetTile);
            }
            else
            {
                if (piece.Player != player)
                {
                    legalMoves.Add(targetTile);
                }
                break;
            }
        }

        return legalMoves;
    }

    public static List<Tile> GetLegalMovesForMovementSets(Tile tile, Player player, List<Func<Tile, Player, List<Tile>>> sets)
    {
        int x = tile.X;
        int y = tile.Y;
        Tile[,] tiles = GridManager.instance.Tiles;
        List<Tile> legalMoves = new List<Tile>();

        foreach (var movementSet in sets)
        {
            legalMoves.AddRange(movementSet(tile, player));
        }

        return legalMoves;
    }
}
