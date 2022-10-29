using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] int _initialTurnLimit;
    public int InitialTurnLimit => _initialTurnLimit;
    [InspectorReadOnly] public int TurnLimit;
    public int RemainingTurns => TurnLimit - CurrentTurn;
    public int CurrentTurn;

    public bool instantMoves;
    public int pieceSpeed;
    public float turnTime;

    public GameState GameState;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        TurnLimit = _initialTurnLimit;
        CurrentTurn = 0;
        ChangeState(GameState.PickPlayers);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (GameState)
        {
            case GameState.PickPlayers:
                UIManager.instance.ShowStartupPanel();
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

    public void ResetTurnLimit()
    {
        TurnLimit = InitialTurnLimit + CurrentTurn;
    }
}

public enum GameState
{
    PickPlayers,
    GenerateGrid,
    MakeMoves,
    EndGame
}