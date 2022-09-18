using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class Pawn : Chesspiece
{
    public bool HasMoved = false;

    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((t, p) => TypesOfMovement.MoveInDirection(t, p, 0, _player.PpawnMovementDirection, maxMoves: HasMoved ? 1 : 2, canCapture: false));
        _movementSets.Add((t, p) => TypesOfMovement.MoveInDirection(t, p, 1, _player.PpawnMovementDirection, maxMoves: 1, yMirror: true, mustCapture: true));
    }
}
