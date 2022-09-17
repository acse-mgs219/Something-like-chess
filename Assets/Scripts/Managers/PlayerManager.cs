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
        Player activePlayer = Players[CurrentActivePlayerIndex];

        activePlayer.StartTurn();
    }

    public void OnPlayerEndTurn()
    {
        CurrentActivePlayerIndex++;
        if (CurrentActivePlayerIndex >= Players.Count)
        {
            CurrentActivePlayerIndex = 0;
        }

        StartPlayerTurn();
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
}