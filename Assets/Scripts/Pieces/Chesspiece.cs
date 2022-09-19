using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    protected List<Move> _legalMoves;
    public List<Move> LegalMoves => _legalMoves;

    protected Player _player;
    public Player Player => _player;

    public Color Color => ColorHelper.Instance.GetColor(_player.Color);

    protected List<Func<Tile[,], Tile, Player, List<Move>>> _movementSets;
    public bool HasMoved => History.Moves > 0;

    public PieceHistory History;
    protected Chesspiece _predictionCopy;
    public Chesspiece PredictionCopy => _predictionCopy;
    private bool _isPredictionCopy = false;
    public bool IsPredictionCopy => _isPredictionCopy;

    public virtual void CalculateLegalMoves(Tile[,] grid)
    {
        _legalMoves = TypesOfMovement.GetLegalMovesForMovementSets(grid, _tile, _player, _movementSets);
    }

    // Should only be called from PieceManager when instantiating piece
    // #TODO: Surely there is a way to give constructor arguments in Unity
    public virtual void Init(Player player, Tile tile)
    {
        _movementSets = new List<Func<Tile[,], Tile, Player, List<Move>>>();
        _player = player;
        _player.Pieces.Add(this);

        PlaceAt(tile);
        _renderer.color = Color;
        History = new PieceHistory(tile);
    }

    public virtual void PlaceCopyOnPredicitonBoard()
    {
        if (_predictionCopy == null)
        {
            ScriptablePiece Piece = PieceManager.instance.GetPieceOfType(Type);
            Chesspiece instancePiece = Instantiate(Piece.Piece);

            instancePiece._movementSets = _movementSets;
            instancePiece._player = _player;
            instancePiece._vip = _vip;
            instancePiece._isPredictionCopy = true;
            Tile tileForCopy = GridManager.instance.PredictionBoard[_tile.X, _tile.Y];
            instancePiece.PlaceAt(tileForCopy);
            _predictionCopy = instancePiece;
        }
        else
        {
            Tile tileForCopy = GridManager.instance.PredictionBoard[_tile.X, _tile.Y];
            _predictionCopy.PlaceAt(tileForCopy);
        }
    }

    public void Destroy()
    {
        _tile.OccupyingPiece = null;
        _tile = null;

        if (VIP)
        {
            _player.Pieces.ForEach(p => Destroy(p));
            Destroy(_player.gameObject);
            GameManager.instance.ChangeState(GameState.EndGame);
        }

        if (_predictionCopy != null)
        {
            _player.Pieces.Remove(this);
            _predictionCopy.Destroy();
        }

        _player = null;
        Destroy(gameObject);
    }

    public void PlaceAt(Tile tile)
    {
        if (_tile?.OccupyingPiece != null) _tile.OccupyingPiece = null;

        _tile = tile;
        _tile.OccupyingPiece = this;
        transform.position = new Vector3(_tile.X, _tile.Y, _isPredictionCopy ? 15f : -1f);
    }

    public void MoveTo(Tile tile)
    {
        Move toMove = _legalMoves.FirstOrDefault(m => m.ToTile == tile);
        if (toMove != null)
        {
            PerformMove(toMove);
        }
        else
        {
            PieceManager.instance.SetSelectedPiece(null);
        }
    }

    public void PerformMove(Move move)
    {
        if (_isPredictionCopy)
        {
            PredictionMove(move);
        }
        else
        {
            RealMove(move);
        }
    }

    private void PredictionMove(Move move)
    {
        _tile.OccupyingPiece = null;

        _tile = GridManager.instance.ConvertRealTileToPrediction(move.ToTile);
        _tile.OccupyingPiece = this;
        transform.position = new Vector3(_tile.X, _tile.Y, transform.position.z);
    }

    private void RealMove(Move move)
    {

        History.RecordMove(move);

        _tile.OccupyingPiece = null;
            
        _tile = move.ToTile;

        if (move.Castle == false && move.TargetPiece != null)
        {
            move.TargetPiece.Destroy();
        }
        _tile.OccupyingPiece = this;
        transform.position = new Vector3(_tile.X, _tile.Y, transform.position.z);

        // Game ends in draw if 50 turns go by without a pawn move.
        if (this is Pawn)
        {
            GameManager.instance.TurnLimit = GameManager.instance.CurrentTurn + 50;
        }

        PieceManager.instance.SetSelectedPiece(null);
        PlayerManager.instance.OnPlayerEndTurn();
    }
}

public class PieceHistory
{
    public List<Move> PlayedMoves { get; private set; }
    public List<Tile> OccupiedTiles { get; private set; }
    public List<Chesspiece> CapturedPieces { get; private set; }
    public List<int> MoveTurns { get; private set; }
    public int Moves { get; private set; }

    public PieceHistory(Tile tile)
    {
        PlayedMoves = new List<Move>();
        OccupiedTiles = new List<Tile>() { tile };
        CapturedPieces = new List<Chesspiece>();
        MoveTurns = new List<int>() { 0 };
        Moves = 0;
    }

    public void RecordMove(Move move)
    {
        PlayedMoves.Add(move);
        OccupiedTiles.Add(move.ToTile);
        if (move.TargetPiece != null && move.Castle == false)
        {
            CapturedPieces.Add(move.TargetPiece);
        }
        MoveTurns.Add(GameManager.instance.CurrentTurn);
        Moves++;
    }
}