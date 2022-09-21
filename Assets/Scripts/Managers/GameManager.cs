using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int TurnLimit;
    public int RemainingTurns;
    public int CurrentTurn;

    public GameState GameState;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RemainingTurns = TurnLimit;
        ChangeState(GameState.PickPlayers);
    }

    public void ChangeState(GameState newState)
    {
        // The game has ended, do nothing.
        if (GameState == GameState.EndGame)
        {
            return;
        }

        GameState = newState;
        switch (GameState)
        {
            case GameState.PickPlayers:
                break;
            case GameState.GenerateGrid:
                GridManager.instance.Init();
                break;
            case GameState.MakeMoves:
                if (RemainingTurns > 0)
                {
                    PlayerManager.instance.StartPlayerTurn();
                }
                else
                {
                    ChangeState(GameState.EndGame);
                }
                break;
            case GameState.EndGame:
                UIManager.instance.AnnounceWinner();
                break;
        }
    }
}

public enum GameState
{
    PickPlayers,
    GenerateGrid,
    MakeMoves,
    EndGame
}