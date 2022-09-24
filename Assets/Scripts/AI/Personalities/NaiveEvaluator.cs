using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class NaiveEvaluator : Intelligence
{
    public float WaitingTime = 0.1f;

    public NaiveEvaluator(Player player) : base(player, AITypes.AIType.RandomMove) { }

    private float PieceTypeToValue(PieceType type, Tile tile = null)
    {
        switch(type)
        {
            case PieceType.Rook:
                return 5f;
            case PieceType.Knight:
                return 2.9f;
            case PieceType.Bishop:
                return 3.1f;
            case PieceType.King:
                return 50f;
            case PieceType.Queen:
                return 9f;
            case PieceType.Pawn:
                return ((tile != null) && (tile.Y == 0 || tile.Y == GridManager.instance.Height - 1)) ? 9f : 1f;
            default:
                Debug.Log("Unknown piece. Perhaps fairy piece? Teach Naive Evaluator how to evaluate this piece by editing PieceTypeToValue please!");
                return 3f;
        }
    }

    public float EvaluateTileValue(Tile tile)
    {
        Chesspiece tilePiece = tile.OccupyingPiece;

        if (tilePiece == null)
        {
            return 0f;
        }

        Player tileOccupier = tilePiece.Player;

        float tileValue;

        float baseValue = PieceTypeToValue(tilePiece.Type, tile);

        List<PieceType> attackerTypes = PlayerManager.instance.Players
            .Where(p => p != tileOccupier)
            .SelectMany(p => p.LegalMoves)
            .Where(m => m.ToTile == tile && m.FromTile.OccupyingPiece != null)
            .Select(m => m.FromTile.OccupyingPiece.Type)
            .ToList();

        if (attackerTypes.Any() == false)
        {
            tileValue = baseValue;
        }
        else
        {
            bool isDefended = tileOccupier.LegalMoves.Any(m => m.ToTile == tile);
            if (isDefended == false)
            {
                tileValue = tileOccupier == _player ? 0f : baseValue / 2;
            }
            else
            {
                float smallestAttackerValue = attackerTypes.Select(at => PieceTypeToValue(at)).Min();
                tileValue = Mathf.Max(baseValue - smallestAttackerValue, (tileOccupier == _player ? 0f : baseValue / 2));
            }
        }

        return tileOccupier == _player ? tileValue : -tileValue;
    }

    public override float EvaluatePosition(Tile[,] grid)
    {
        float boardValue = 0f;
        foreach (Tile tile in grid)
        {
            boardValue += EvaluateTileValue(tile);
        }

        return boardValue;
    }

    public override void PlayMove()
    {
        Dictionary<Move, float> valuesPerMove = new Dictionary<Move, float>();

        List<Move> originalLegalMoves = _player.LegalMoves.Select(m => m).ToList();

        float bestMoveValue = -999f;
        foreach (Move move in originalLegalMoves)
        {
            GridManager.instance.ResetPredictionBoard();

            move.PerformMove(prediction: true);

            PlayerManager.instance.CalculateAllPiecesLegalMoves(prediction: true);

            float moveValue = EvaluatePosition(GridManager.instance.PredictionBoard);
            valuesPerMove[move] = moveValue;
            bestMoveValue = Mathf.Max(bestMoveValue, moveValue);
        }

        Debug.Log($"Best possible move for {_player.Name} leaves them with value {bestMoveValue}.");

        Move chosenMove = valuesPerMove.Where(kv => kv.Value == bestMoveValue).RandomElement().Key;
        //Move chosenMove = valuesPerMove.RandomElementByWeight(kv => kv.Value).Key;
        //Move bestMove = valuesPerMove.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

        _player.StartCoroutine(PlayMove(chosenMove));
    }

    IEnumerator PlayMove(Move move)
    {
        yield return new WaitForSeconds(WaitingTime);

        move.PerformMove();
    }
}
