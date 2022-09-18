using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class Queen : Chesspiece
{
    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 1, 0, xMirror: true));
        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 0, 1, yMirror: true));
        _movementSets.Add((g, t, p) => TypesOfMovement.MoveInDirection(g, t, p, 1, 1, xMirror: true, yMirror: true));
    }
}
