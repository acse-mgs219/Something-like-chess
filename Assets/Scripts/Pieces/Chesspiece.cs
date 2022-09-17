using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public abstract class Chesspiece : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;

    protected Tile _tile;

    // Means that capturing this piece ends the game.
    [SerializeField] protected bool _vip;
    public bool VIP => _vip;

    protected List<Tile> _legalMoves;
    public List<Tile> LegalMoves => _legalMoves;

    protected Player _player;
    public Player Player => _player;

    public Color Color => ColorHelper.Instance.GetColor(_player.Color);

    protected List<Func<Tile, Player, List<Tile>>> _movementSets;

    public abstract bool CanMoveTo(int x, int y);
    public abstract bool CanCaptureAt(int x, int y);

    public void CalculateLegalMoves()
    {
        _legalMoves = TypesOfMovement.GetLegalMovesForMovementSets(_tile, _player, _movementSets);
    }

    // Should only be called from PieceManager when instantiating piece
    // #TODO: Surely there is a way to give constructor arguments in Unity
    public virtual void Init(Player player, Tile tile)
    {
        _movementSets = new List<Func<Tile, Player, List<Tile>>>();
        _player = player;
        _player.Pieces.Add(this);

        MoveTo(tile, forceMove: true);
        _renderer.color = Color;
    }

    public void Destroy()
    {
        _tile.OccupyingPiece = null;
        _tile = null;
        _player.Pieces.Remove(this);
        _player = null;

        if (VIP)
        {
            GameManager.instance.ChangeState(GameState.GoogGame);
        }

        Destroy(gameObject);
    }

    public void MoveTo(Tile tile, bool forceMove = false)
    {
        if (forceMove || _legalMoves.Contains(tile))
        {
            if (_tile != null)
            {
                _tile.OccupyingPiece = null;
            }

            _tile = tile;
            _tile.OccupyingPiece?.Destroy();
            _tile.OccupyingPiece = this;
            this.transform.position = new Vector3(_tile.X, _tile.Y, this.transform.position.z);
            PlayerManager.instance.CalculateAllPiecesLegalMoves();
        }

        PieceManager.instance.SetSelectedPiece(null);

        // Do not want to move turns while we are still palcing initial pieces!
        if (forceMove == false)
        {
            PlayerManager.instance.OnPlayerEndTurn();
        }
    }
}
