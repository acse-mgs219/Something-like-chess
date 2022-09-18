using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class Rook : Chesspiece
{
    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((t, p) => TypesOfMovement.MoveInDirection(t, p, 1, 0, xMirror: true));
        _movementSets.Add((t, p) => TypesOfMovement.MoveInDirection(t, p, 0, 1, yMirror: true));
    }
}
