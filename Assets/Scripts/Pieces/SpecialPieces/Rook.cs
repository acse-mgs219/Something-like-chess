using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class Rook : Chesspiece
{
    public override void Init(Player player, Tile tile)
    {
        base.Init(player, tile);

        _movementSets.Add((t, p) => TypesOfMovement.HorizontalMovement(t, p, true));
        _movementSets.Add((t, p) => TypesOfMovement.HorizontalMovement(t, p, false));
        _movementSets.Add((t, p) => TypesOfMovement.VerticalMovement(t, p, true));
        _movementSets.Add((t, p) => TypesOfMovement.VerticalMovement(t, p, false));
    }

    public override bool CanCaptureAt(int x, int y)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanMoveTo(int x, int y)
    {
        throw new System.NotImplementedException();
    }
}
