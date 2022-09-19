using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    [SerializeField] int _pawnMovementDirection = -1;
    public int PawnMovementDirection => _pawnMovementDirection;

    public bool IsInCheck = false;

    public void TrimChecks()
    {
        foreach (Chesspiece piece in _pieces)
        {
            List<Move> movesToRemove = new List<Move>();
            foreach (Move move in piece.LegalMoves)
            {
                GridManager.instance.ResetPredictionBoard();
                Tile[,] grid = GridManager.instance.PredictionBoard;

                piece.PredictionCopy.PerformMove(move);

                bool dangerousMove = PlayerManager.instance.CalculateChecksAgainstPlayer(this);

                if (dangerousMove)
                {
                    movesToRemove.Add(move);
                }
            }

            piece.LegalMoves.RemoveAll(t => movesToRemove.Contains(t));
        }
    }

    public void StartTurn()
    {
        UIManager.instance.ShowCheck(this, IsInCheck);
        TrimChecks();

        if (_isHuman == false)
        {
            StartCoroutine(PlayRandomMove());
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

            yield return new WaitForSeconds(2);

            pieceToMove.PerformMove(pieceToMove.LegalMoves.RandomElement());
            break;
        }
    }
}
