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

    public PieceHistory History;
    public bool HasMoved => History.Moves > 0;

    protected Chesspiece _originalPiece;
    public Chesspiece OriginalPiece => _originalPiece;
    protected Chesspiece _predictionCopy;
    public Chesspiece PredictionCopy => _predictionCopy;
    private bool _isPredictionCopy = false;
    public bool IsPredictionCopy => _isPredictionCopy;

    private Pawn _promotingPawn = null;
    public Pawn PromotingPawn => _promotingPawn;
    public bool IsPromotionDummy = false;

    private Rigidbody2D _body;

    public virtual void CalculateLegalMoves(Tile[,] grid)
    {
        _legalMoves = TypesOfMovement.GetLegalMovesForMovementSets(grid, _tile, _player, _movementSets);
    }

    // Should only be called from PieceManager when instantiating piece
    // #TODO: Surely there is a way to give constructor arguments in Unity
    public virtual void Init(Player player, Tile tile)
    {
        History = new PieceHistory(tile);
        _movementSets = new List<Func<Tile[,], Tile, Player, List<Move>>>();
        _player = player;
        _player.Pieces.Add(this);
        _body = GetComponent<Rigidbody2D>();

        PlaceAt(tile);
        _renderer.color = Color;
    }

    public virtual void InitPromotionDummy(Pawn promotingPawn)
    {
        _promotingPawn = promotingPawn;
        IsPromotionDummy = true;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;
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
            instancePiece.History = History;
            instancePiece._originalPiece = this;
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

    // #TODO: Rework this so destroying pieces just moves them to the side of the board and makes them unmoveable.
    public void Destroy()
    {
        if (_tile.OccupyingPiece == this)
        {
            _tile.OccupyingPiece = null;
        }
        _tile = null;

        if (VIP && IsPredictionCopy == false)
        {
            UnityEngine.Debug.LogAssertion("King was captured, this shouldn't have been possible?");
            _player.Pieces.ForEach(p => Destroy(p));
            Destroy(_player.gameObject);
            GameManager.instance.ChangeState(GameState.EndGame);
        }

        if (_predictionCopy != null)
        {
            _predictionCopy.Destroy();
        }

        if (_originalPiece != null)
        {
            _originalPiece._predictionCopy = null;
        }

        _player.Pieces.Remove(this);
        _player = null;
        Destroy(gameObject);
    }

    public void PlaceAt(Tile tile)
    {
        if (_tile != null && _tile.OccupyingPiece != null) _tile.OccupyingPiece = null;

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

        if (move.Castle == false && move.TargetPiece != null && move.TargetPiece.PredictionCopy != null)
        {
            move.TargetPiece.PredictionCopy.Destroy();
        }
        else if (move.Castle)
        {
            if (move.TargetPiece != null && move.TargetPiece.PredictionCopy != null)
            {
                int destinationX = (move.ToTile.X + move.FromTile.X) / 2;
                Tile targetTile = GridManager.instance.Board[destinationX, move.ToTile.Y];
                Move rookPartOfCastle = new Move(targetTile, move.TargetPiece._tile, castle: true);
                move.TargetPiece.PredictionCopy.PredictionMove(rookPartOfCastle);
            }
            else
            {
                return;
            }
        }
    }

    private void Update()
    {
        if (!IsPredictionCopy && _body.velocity != Vector2.zero)
        {
            if (transform.position.x.ApproximatelyEquals(_tile.X, 0.1f) && transform.position.y.ApproximatelyEquals(_tile.Y, 0.1f))
            {
                transform.position = new Vector3(_tile.X, _tile.Y, transform.position.z);
                _body.velocity = Vector2.zero;
            }
        }
    }

    protected virtual void RealMove(Move move)
    {

        History.RecordMove(move);

        _tile.OccupyingPiece = null;
            
        _tile = move.ToTile;
        _tile.OccupyingPiece = this;
        _body.velocity = new Vector2(move.Translation.x / (Time.deltaTime * 100), move.Translation.y / (Time.deltaTime * 100));

        if (move.Castle == false && move.TargetPiece != null)
        {
            move.TargetPiece.Destroy();
        }
        else if (move.Castle)
        {
            if (move.TargetPiece != null)
            {
                int destinationX = (move.ToTile.X + move.FromTile.X) / 2;
                Tile targetTile = GridManager.instance.Board[destinationX, move.ToTile.Y];
                Move rookPartOfCastle = new Move(targetTile, move.TargetPiece._tile, castle: true);
                move.TargetPiece.RealMove(rookPartOfCastle);
            }
            else
            {
                return;
            }
        }

        // Game ends in draw if 50 turns go by without a pawn move.
        if (this is Pawn pawn)
        {
            GameManager.instance.RemainingTurns = GameManager.instance.CurrentTurn + GameManager.instance.TurnLimit;

            if (move.Promotion)
            {
                // Human chooses promotion.
                if (_player.IsHuman)
                {
                    int x = _tile.X;
                    foreach (PieceType promotionType in pawn.PromotableTypes)
                    {
                        ScriptablePiece Piece = PieceManager.instance.GetPieceOfType(promotionType);
                        Chesspiece instancePiece = Instantiate(Piece.Piece);
                        instancePiece.transform.position = new Vector3(x++, _tile.Y + _player.PawnMovementDirection, -1f);
                        instancePiece.Type = promotionType;
                        instancePiece.InitPromotionDummy(pawn);
                        _player.PromotionDummies.Add(instancePiece);
                    }
                }
                // AI promotes randomly.
                else
                {
                    PieceType promotionType = pawn.PromotableTypes.RandomElement();
                    pawn.Promote(promotionType);
                }
            }
        }

        PieceManager.instance.SetSelectedPiece(null);
        if (!move.Promotion) PlayerManager.instance.OnPlayerEndTurn();
    }

    private void OnMouseDown()
    {
        if (IsPromotionDummy == false)
        {
            return;
        }

        PromotingPawn.Promote(Type);
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