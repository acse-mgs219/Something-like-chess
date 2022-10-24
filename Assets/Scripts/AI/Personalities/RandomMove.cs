using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : Intelligence
{
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
        yield return new WaitForSeconds(waitingTime);

        _player.LegalMoves.RandomElement().PerformMove();
    }
}
