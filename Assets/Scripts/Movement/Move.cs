using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private Tile _toTile;
    public Tile ToTile => _toTile;

    private Tile _fromTile;
    public Tile FromTile => _fromTile;

    private Chesspiece _targetPiece;
    public Chesspiece TargetPiece => _targetPiece;

    private bool _enPassant;
    public bool EnPassant => _enPassant;

    private bool _castle;
    public bool Castle => _castle;

    private bool _promotion;
    public bool Promotion => _promotion;

    public Vector3 Translation => new Vector3(ToTile.X - _fromTile.X, ToTile.Y - _fromTile.Y, 0);

    public Move(Tile toTile, Tile fromTile, Chesspiece targetPiece = null, bool enPassant = false, bool castle = false, bool promotion = false)
    {
        _toTile = toTile;
        _fromTile = fromTile;
        _targetPiece = targetPiece;
        _enPassant = enPassant;
        _castle = castle;
        _promotion = promotion;
    }

    public void PerformMove(bool prediction = false)
    {
        if (prediction)
        {
            PredictionMove();
        }
        else
        {
            RealMove();
        }
    }

    private void PredictionMove()
    {
        Chesspiece movingPiece = _fromTile.OccupyingPiece;
        if (movingPiece.PredictionCopy == null)
        {
            Debug.Log("Trying to predict a move for a piece with no prediction copy? Make sure to call this on the original piece, and not the prediction copy directly.");
            return;
        }

        movingPiece = movingPiece.PredictionCopy;
        Tile toTile = GridManager.instance.ConvertRealTileToPrediction(_toTile);
        movingPiece.PlaceAt(toTile);

        if (_castle == false && _targetPiece != null && _targetPiece.PredictionCopy != null)
        {
            _targetPiece.PredictionCopy.Destroy();
        }
        else if (_castle)
        {
            if (_targetPiece != null && _targetPiece.PredictionCopy != null)
            {
                int destinationX = (toTile.X + _fromTile.X) / 2;
                Tile targetTile = GridManager.instance.Board[destinationX, toTile.Y];
                Move rookPartOfCastle = new Move(targetTile, _targetPiece.Tile, castle: true);
                rookPartOfCastle.PredictionMove();
            }
            else
            {
                return;
            }
        }
    }

    public virtual void FinishRealMove()
    {
        if (_castle == false && _targetPiece != null)
        {
            _targetPiece.Destroy();
            GameManager.instance.ResetTurnLimit();
        }

        if ((_promotion && _toTile.OccupyingPiece.Player.IsChoosingPromotion) == false)
        {
            PlayerManager.instance.OnPlayerEndTurn();
        }
    }

    protected virtual void RealMove()
    {
        Chesspiece movingPiece = _fromTile.OccupyingPiece;

        movingPiece.AssociatedMove = this;

        movingPiece.History.RecordMove(this);

        movingPiece.PlaceAt(_toTile, move: false);

        if (_castle)
        {
            if (_targetPiece != null)
            {
                int destinationX = (_toTile.X + _fromTile.X) / 2;
                Tile targetTile = GridManager.instance.Board[destinationX, _toTile.Y];
                Move rookPartOfCastle = new Move(targetTile, _targetPiece.Tile, castle: true);
                rookPartOfCastle.RealMove();
            }
        }

        // Game ends in draw if 50 turns go by without a pawn move or capture.
        if (movingPiece is Pawn pawn)
        {
            GameManager.instance.ResetTurnLimit();

            if (_promotion)
            {
                // Human chooses promotion.
                if (movingPiece.Player.IsHuman)
                {
                    int x = _toTile.X;
                    foreach (PieceType promotionType in pawn.PromotableTypes)
                    {
                        ScriptablePiece Piece = PieceManager.instance.GetPieceOfType(promotionType);
                        Chesspiece instancePiece = Object.Instantiate(Piece.Piece);
                        instancePiece.transform.position = new Vector3(x++, _toTile.Y + movingPiece.Player.PawnMovementDirection, -1f);
                        instancePiece.Type = promotionType;
                        instancePiece.InitPromotionDummy(pawn);
                        movingPiece.Player.PromotionDummies.Add(instancePiece);
                    }
                }
                // AI promotes to queen always. #TODO: Add way for AI to choose promotion?
                else
                {
                    PieceType promotionType = PieceType.Queen;
                    pawn.Promote(promotionType);
                }
            }
        }

        if (GameManager.instance.instantMoves)
        {
            movingPiece.SetInMotion(Translation.normalized * GameManager.instance.pieceSpeed);
        }
        else
        {
            movingPiece.transform.position = new Vector3(_toTile.X, _toTile.Y, -1f);
            FinishRealMove();
        }
    }
}
