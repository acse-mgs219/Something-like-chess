using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Intelligence
{
    public readonly Player _player;
    public readonly AITypes.AIType Type;

    public Intelligence(Player player, AITypes.AIType type)
    {
        _player = player;
        Type = type;
    }

    public abstract float EvaluatePosition(Tile[,] grid);

    public abstract void PlayMove();
}
