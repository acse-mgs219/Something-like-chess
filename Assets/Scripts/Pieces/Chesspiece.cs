using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public abstract class Chesspiece : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;

    protected Tile _tile;
    public Tile Tile => _tile;
    public PieceType Type;

    // Means that capturing this piece ends the game.
    [SerializeField] protected bool _vip;
    public bool VIP => _vip;

    protected List<Tile> _legalMoves;
    public List<Tile> LegalMoves => _legalMoves;

    protected Player _player;
    public Player Player => _player;

    public Color Color => ColorHelper.Instance.GetColor(_player.Color);

    protected List<Func<Tile[,], Tile, Player, List<Tile>>> _movementSets;
    public bool HasMoved => History.Moves > 0;

    public PieceHistory History;
    private Chesspiece _predictionCopy;
    public Chesspiece PredictionCopy => _predictionCopy;

    public virtual void CalculateLegalMoves(Tile[,] grid)
    {
        _legalMoves = TypesOfMovement.GetLegalMovesForMovementSets(grid, _tile, _player, _movementSets);
    }

    // Should only be called from PieceManager when instantiating piece
    // #TODO: Surely there is a way to give constructor arguments in Unity
    public virtual void Init(Player player, Tile tile)
    {
        _movementSets = new List<Func<Tile[,], Tile, Player, List<Tile>>>();
        _player = player;
        _player.Pieces.Add(this);

        PlaceAt(tile);
        _renderer.color = Color;
        History = new PieceHistory(tile);
    }

    public void PlaceCopyOnPredicitonBoard()
    {
        ScriptablePiece Piece = PieceManager.instance.GetPieceOfType(Type);
        Chesspiece instancePiece = Instantiate(Piece.Piece);

        instancePiece._movementSets = _movementSets;
        instancePiece._player = _player;
        instancePiece._vip = _vip;
        Tile tileForCopy = GridManager.instance.PredictionBoard[_tile.X, _tile.Y];
        instancePiece.PlaceAt(tileForCopy);
        _predictionCopy = instancePiece;
    }

    public void Destroy()
    {
        _tile.OccupyingPiece = null;
        _tile = null;
        _player.Pieces.Remove(this);

        if (VIP)
        {
            _player.Pieces.ForEach(p => Destroy(p));
            Destroy(_player.gameObject);
            GameManager.instance.ChangeState(GameState.EndGame);
        }

        _player = null;
        Destroy(gameObject);
    }

    public void PlaceAt(Tile tile)
    {
        _tile = tile;
        _tile.OccupyingPiece = this;
        transform.position = new Vector3(_tile.X, _tile.Y, _tile.transform.position.z - 0.1f);
    }

    public void MoveTo(Tile tile)
    {
        if (_legalMoves.Contains(tile))
        {
            History.RecordMove(tile);

            _tile.OccupyingPiece = null;
            _tile = tile;
            _tile.OccupyingPiece?.Destroy();
            _tile.OccupyingPiece = this;
            transform.position = new Vector3(_tile.X, _tile.Y, _tile.transform.position.z - 0.1f);

            // Game ends in draw if 50 turns go by without a pawn move.
            if (this is Pawn pawn)
            {
                GameManager.instance.TurnLimit = GameManager.instance.CurrentTurn + 50;
                if (pawn.EnPassantTiles.TryGetValue(tile, out Pawn passedPawn))
                {
                    passedPawn.Destroy();
                }
            }

            PlayerManager.instance.OnPlayerEndTurn();
        }

        PieceManager.instance.SetSelectedPiece(null);
    }

    #region CustomEquality
    public override bool Equals(object other)
    {
        return Equals(other as Chesspiece);
    }

    public virtual bool Equals(Chesspiece other)
    {
        if (other == null)
        {
            return false; 
        }

        bool sameType = other.Type == Type;
        bool sameVIP = other.VIP == VIP;
        bool samePlayer = other.Player == Player;

        return sameType && sameVIP && samePlayer;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;

            hash = hash * 23 + Type.GetHashCode();
            hash = hash * 23 + _vip.GetHashCode();
            hash = hash * 23 + _player.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(Chesspiece item1, Chesspiece item2)
    {
        if (object.ReferenceEquals(item1, item2)) { return true; }
        if ((object)item1 == null || (object)item2 == null) { return false; }
        return item1.Equals(item2);
    }

    public static bool operator !=(Chesspiece item1, Chesspiece item2)
    {
        return !(item1 == item2);
    }
    #endregion
}

public class PieceHistory
{
    public List<Tile> OccupiedTiles { get; private set; }
    public List<Chesspiece> CapturedPieces { get; private set; }
    public List<int> MoveTurns { get; private set; }
    public int Moves { get; private set; }

    public PieceHistory(Tile tile)
    {
        OccupiedTiles = new List<Tile>() { tile };
        CapturedPieces = new List<Chesspiece>();
        MoveTurns = new List<int>() { 0 };
        Moves = 0;
    }

    public void RecordMove(Tile toTile)
    {
        OccupiedTiles.Add(toTile);
        if (toTile.OccupyingPiece != null) CapturedPieces.Add(toTile.OccupyingPiece);
        MoveTurns.Add(GameManager.instance.CurrentTurn);
        Moves++;
    }
}