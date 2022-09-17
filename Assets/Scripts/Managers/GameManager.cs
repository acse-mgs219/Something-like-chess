using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int TurnLimit;
    private int _currentTurn = 0;

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
        GameState = newState;
        switch (GameState)
        {
            case GameState.GenerateGrid:
                GridManager.instance.Init();
                break;
            case GameState.PositionPieces:
                PlayerManager.instance.Init();
                break;
            case GameState.MakeMoves:
                // #TODO: alternate player turns
                break;
            case GameState.GoogGame:
                break;
        }
    }
}

public enum GameState
{
    GenerateGrid,
    PositionPieces,
    MakeMoves,
    GoogGame
}