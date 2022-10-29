using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEssentials.Extensions;
using static System.Collections.Specialized.BitVector32;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    private List<ScriptablePlayer> _playerPrefabs;

    private List<Player> _players = new List<Player>();
    public List<Player> Players => _players;

    public int CurrentActivePlayerIndex = 0;
    public Player CurrentActivePlayer => Players[CurrentActivePlayerIndex];

    private void Awake()
    {
        instance = this;

        _playerPrefabs = Resources.LoadAll<ScriptablePlayer>("Players").ToList();

        foreach (ScriptablePlayer Player in _playerPrefabs)
        {
            Player instancePlayer = Instantiate(Player.PlayerPrefab);
            Players.Add(instancePlayer);
        }
    }

    public void Init()
    {
        foreach (Player player in _players)
        {
            player.Init();
        }

        GameManager.instance.ChangeState(GameState.MakeMoves);
    }

    public void SetPlayerHuman(int playerIndex, bool human)
    {
        _players[playerIndex].SetIsHuman(human);
    }

    public void SetPlayerColor(int playerIndex, ColorHelper.NamedColor color)
    {
        _players[playerIndex].SetColor(color);
    }

    public void SetPlayerAI(int playerIndex, AITypes.AIType aiType)
    {
        _players[playerIndex].SetAI(aiType);
    }

    public void StartPlayerTurn()
    {
        CalculateAllPiecesLegalMoves();
        // #TODO: Maybe on a check move we play both the move sound and the check sound?
        SoundManager.instance.Play(CurrentActivePlayer.IsInCheck ? SoundManager.instance.checkSound : SoundManager.instance.moveSound);
        CurrentActivePlayer.StartTurn();
    }

    public void OnPlayerEndTurn()
    {
        CurrentActivePlayer.IsInCheck = false;

        CurrentActivePlayerIndex++;
        if (CurrentActivePlayerIndex >= Players.Count)
        {
            CurrentActivePlayerIndex = 0;
            GameManager.instance.CurrentTurn++;
        }

        GameManager.instance.ChangeState(GameState.MakeMoves);
    }

    public void CalculateAllPiecesLegalMoves(bool prediction = false)
    {
        Tile[,] board = prediction ? GridManager.instance.PredictionBoard : GridManager.instance.Board;

        foreach (Player player in Players.Where(p => p != CurrentActivePlayer))
        {
            player.LegalMoves.Clear();
            foreach (Chesspiece piece in player.Pieces)
            {
                piece.CalculateLegalMoves(board);
                player.LegalMoves.AddRange(piece.LegalMoves);
            }
        }

        CurrentActivePlayer.LegalMoves.Clear();
        // We must calculate the moves of the current active player last, so that they will know if they are in check or can castle.
        foreach (Chesspiece piece in CurrentActivePlayer.Pieces)
        {
            piece.CalculateLegalMoves(board);
            CurrentActivePlayer.LegalMoves.AddRange(piece.LegalMoves);
        }
    }

    public bool CalculateChecksAgainstPlayer(Player player)
    {
        bool wasInCheck = player.IsInCheck;
        player.IsInCheck = false;
        foreach (Player otherPlayer in Players.Where(p => p != player))
        {
            List<Chesspiece> predictionPieces = otherPlayer.Pieces.Select(p => p.PredictionCopy).Where(p => p != null).ToList();

            foreach (Chesspiece piece in predictionPieces)
            {
                piece.CalculateLegalMoves(GridManager.instance.PredictionBoard);
            }
        }

        bool wouldBeInCheck = player.IsInCheck;
        player.IsInCheck = wasInCheck;
        return wouldBeInCheck;
    }

    public bool HasPlayer2MovedSincePlayer1Turn(Player player1, Player player2, int turn)
    {
        int currentTurn = GameManager.instance.CurrentTurn;

        if (turn > currentTurn || (turn == currentTurn && CurrentActivePlayerIndex < Players.IndexOf(player1)))
        {
            return false;
        }

        if (turn == currentTurn)
        {
            return CurrentActivePlayerIndex > Players.IndexOf(player2);
        }

        if (turn == currentTurn - 1)
        {
            return Players.IndexOf(player2) > Players.IndexOf(player1);
        }

        return true;
    }
}