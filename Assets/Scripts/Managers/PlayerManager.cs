using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
        // #TODO: Nothing to be done when setting up players yet?

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
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
        CalculateAllPiecesLegalMoves();

        GameManager.instance.ChangeState(GameState.MakeMoves);
    }

    public void CalculateAllPiecesLegalMoves()
    {
        foreach (Player player in Players)
        {
            foreach (Chesspiece piece in player.Pieces)
            {
                piece.CalculateLegalMoves();
            }
        }
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