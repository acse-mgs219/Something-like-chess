using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : Intelligence
{
    public float WaitingTime = 0.1f;

    public RandomMove(Player player) : base(player, AITypes.AIType.RandomMove) { }

    public override float EvaluatePosition(Tile[,] grid)
    {
        // The position is equal.
        return 0f;
    }

    public override void PlayMove()
    {
        _player.StartCoroutine(PlayRandomMove());
    }

    IEnumerator PlayRandomMove()
    {
        yield return new WaitForSeconds(WaitingTime);

        _player.LegalMoves.RandomElement().PerformMove();
    }
}
