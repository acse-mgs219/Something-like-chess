using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEssentials.Extensions;
using static UnityEssentials.Extensions.ColorHelper;

public class Player : MonoBehaviour
{
    [SerializeField] string _name;
    public string Name => _name;

    [SerializeField] bool _isHuman;
    public bool IsHuman => _isHuman;

    [SerializeField] NamedColor _color;
    public NamedColor Color => _color;

    private List<Chesspiece> _pieces = new List<Chesspiece>();
    public List<Chesspiece> Pieces => _pieces;

    public List<Chesspiece> PromotionDummies = new List<Chesspiece>();

    private List<Move> _legalMoves = new List<Move>();
    public List<Move> LegalMoves => _legalMoves;

    [SerializeField] int _pawnMovementDirection = -1;
    public int PawnMovementDirection => _pawnMovementDirection;

    public bool IsInCheck = false;

    public void SetColor(NamedColor color)
    {
        _color = color;
    }

    public void SetIsHuman(bool set)
    {
        _isHuman = set;
    }

    public void TrimChecks()
    {
        List<Move> movesToRemove = new List<Move>();

        foreach (Move move in _legalMoves)
        {
            GridManager.instance.ResetPredictionBoard();
            Tile[,] grid = GridManager.instance.PredictionBoard;

            move.PerformMove(prediction: true);

            bool dangerousMove = PlayerManager.instance.CalculateChecksAgainstPlayer(this);

            if (dangerousMove)
            {
                movesToRemove.Add(move);
            }
        }

        foreach (Move move in movesToRemove)
        {
            Chesspiece movingPiece = move.FromTile.OccupyingPiece;
            movingPiece.LegalMoves.Remove(move);
            _legalMoves.Remove(move);
        }
    }

    public void StartTurn()
    {
        UIManager.instance.ShowCheck(this, IsInCheck);
        TrimChecks();

        if (_pieces.All(p => p.LegalMoves.Count == 0))
        {
            GameManager.instance.ChangeState(GameState.EndGame);
        }
        else
        {
            if (_isHuman == false)
            {
                StartCoroutine(PlayRandomMove());
            }
        }
    }

    IEnumerator PlayRandomMove()
    {
        while (true)
        {
            Chesspiece pieceToMove = _pieces.RandomElement();
            if (pieceToMove.LegalMoves == null || pieceToMove.LegalMoves.Count == 0)
            {
                continue;
            }

            yield return new WaitForSeconds(0.1f);

            pieceToMove.LegalMoves.RandomElement().PerformMove();
            break;
        }
    }
}
