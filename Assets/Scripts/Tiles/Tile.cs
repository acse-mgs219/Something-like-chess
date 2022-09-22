using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
    private string _tileName;
    public string TileName => _tileName;
    [SerializeField] List<Color> _colors;

    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] protected GameObject _moveIndicator;
    [SerializeField] protected GameObject _captureIndicator;

    public Chesspiece OccupyingPiece;
    private bool _isFree => (OccupyingPiece == null);

    private int _x;
    public int X => _x;
    private int _y;
    public int Y => _y;

    private bool _isMouseOver;
    public bool IsMouseOver
    {
        get => _isMouseOver;
        set
        {
            _isMouseOver = value;
            _highlight.SetActive(_isMouseOver);
        }
    }

    public virtual void Init(int x, int y)
    {
        _tileName = $"{(char)(x + 65)}{y}";
        _x = x; 
        _y = y;
        int colorIndex = (x + y) % _colors.Count;
        _renderer.color = _colors[colorIndex];
    }

    public void SetIndicator(bool set, Chesspiece forPiece = null)
    {
        if (set)
        {
            if (OccupyingPiece != null || (forPiece is Pawn pawn && pawn.EnPassantTiles.Contains(this)))
            {
                SetCaptureIndicator(true, forPiece);
            }
            else
            {
                SetMoveIndicator(true, forPiece);
            }
        }
        else
        {
            SetMoveIndicator(false);
            SetCaptureIndicator(false);
        }
    }

    public void SetMoveIndicator(bool set, Chesspiece forPiece = null)
    {
        _moveIndicator.SetActive(set);

        if (set && forPiece != null)
        {
            SpriteRenderer moveIndicatorRenderer = _moveIndicator.GetComponent<SpriteRenderer>();
            moveIndicatorRenderer.color = forPiece.Color;
        }
    }

    public void SetCaptureIndicator(bool set, Chesspiece forPiece = null)
    {
        _captureIndicator.SetActive(set);

        if (set && forPiece != null)
        {
            SpriteRenderer moveIndicatorRenderer = _moveIndicator.GetComponent<SpriteRenderer>();
            moveIndicatorRenderer.color = new Color(forPiece.Color.r, forPiece.Color.g, forPiece.Color.b, moveIndicatorRenderer.color.a);
        }
    }

    public bool IsTileSafeForPlayer(Player player)
    {
        foreach (Player otherPlayer in PlayerManager.instance.Players.Where(p => p != player))
        {
            if (otherPlayer.Pieces != null)
            foreach (Chesspiece piece in otherPlayer.Pieces)
            {
                if (piece.LegalMoves != null)
                foreach (Move move in piece.LegalMoves)
                {
                    if (move.ToTile == this)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void OnMouseDown()
    {
        Player currentPlayer = PlayerManager.instance.CurrentActivePlayer;

        if (currentPlayer.IsHuman)
        {
            Chesspiece selectedPiece = PieceManager.instance.SelectedPiece;

            if (selectedPiece is null)
            {
                if (OccupyingPiece is null)
                {
                    return;
                }

                if (OccupyingPiece.Player != currentPlayer)
                {
                    return;
                }

                PieceManager.instance.SetSelectedPiece(OccupyingPiece);
            }
            else
            {
                currentPlayer.LegalMoves.Where(m => m.FromTile == selectedPiece.Tile && m.ToTile == this).FirstOrDefault()?.PerformMove();
                PieceManager.instance.SetSelectedPiece(null);
            }
        }
    }

    private void OnMouseEnter()
    {
        IsMouseOver = true;
        // #TODO: MenuManager.instance.ShowTileInfo(this); maybe show tile name?
    }

    private void OnMouseExit()
    {
        IsMouseOver = false;
    }
}