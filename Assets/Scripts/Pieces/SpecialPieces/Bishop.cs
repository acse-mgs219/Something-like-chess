using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class Bishop : Chesspiece
{
    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((t, p) => TypesOfMovement.MoveInDirection(t, p, 1, 1, xMirror: true, yMirror: true));
    }
}
