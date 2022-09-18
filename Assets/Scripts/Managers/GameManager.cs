using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int TurnLimit;
    public int CurrentTurn;

    public GameState GameState;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
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
            case GameState.GenerateGrid:
                GridManager.instance.Init();
                break;
            case GameState.MakeMoves:
                if (CurrentTurn < TurnLimit)
                {
                    PlayerManager.instance.StartPlayerTurn();
                }
                break;
            case GameState.EndGame:
                UIManager.instance.AnnounceWinner(PlayerManager.instance.Players.First());
                break;
        }
    }
}

public enum GameState
{
    GenerateGrid,
    MakeMoves,
    EndGame
}