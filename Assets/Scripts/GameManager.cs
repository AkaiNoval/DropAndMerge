using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public enum GameState
{
    GenerateScene,
    WaitingForInput,
    Falling,
    Merging,
    Win,
    Lose
}
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; private set => instance = value; }
    public static event Action<int> OnRoundChange;
    [SerializeField ] private int round;
    [SerializeField] GameState currentState;
    public int Round 
    { 
        get => round;
        set 
        { 
            round = (value == 1000) ? 0 : Mathf.Clamp(value, 0, 1000);
            OnRoundChange?.Invoke(Round);
        }
    }
    #region Awake
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    private void Start()
    {
        ChanceState(GameState.GenerateScene);
    }
    public void IncreaseRoundTesting()
    {
        Round++;
    }
    public void ChanceState(GameState newState)
    {
        switch (newState)
        {
            case GameState.GenerateScene:
                GridManager.Instance.GenerateGrid();
                CubeManager.Instance.InitPreviewCubes();
                Round = 0;
                break;
            case GameState.WaitingForInput:
                break;
            case GameState.Merging:
                break;            
            case GameState.Falling:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                break;
        }
        currentState = newState;
    }
}
