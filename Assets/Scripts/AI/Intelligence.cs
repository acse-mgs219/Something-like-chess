using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Intelligence
{
    public readonly Player _player;
    public readonly AITypes.AIType Type;
    protected virtual float localWaitingTime => -0.1f;
    protected float waitingTime;

    public Intelligence(Player player, AITypes.AIType type)
    {
        _player = player;
        Type = type;
        waitingTime = localWaitingTime > 0 ? localWaitingTime : GameManager.instance.turnTime;
    }

    public abstract float EvaluatePosition(Tile[,] grid);

    public abstract void PlayMove();
}
