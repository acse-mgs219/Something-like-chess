using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class Knight : Chesspiece
{
    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 2, 1, maxMoves: 1, xMirror: true, yMirror: true));
        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 1, 2, maxMoves: 1, xMirror: true, yMirror: true));
    }
}
