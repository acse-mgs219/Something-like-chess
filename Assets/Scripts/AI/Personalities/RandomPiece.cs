using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPiece : Intelligence
{
    public float WaitingTime = 1f;

    public RandomPiece(Player player) : base(player, AITypes.AIType.RandomPiece) { }


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
        while (true)
        {
            Chesspiece pieceToMove = _player.Pieces.RandomElement();
            if (pieceToMove.LegalMoves == null || pieceToMove.LegalMoves.Count == 0)
            {
                continue;
            }

            yield return new WaitForSeconds(WaitingTime);

            pieceToMove.LegalMoves.RandomElement().PerformMove();
            break;
        }
    }
}
