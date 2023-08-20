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
    public GameState CurrentState;
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
    public void ChanceState(GameState newState)
    {
        switch (newState)
        {
            case GameState.GenerateScene:
                GridManager.Instance.GenerateGrid();
                CubeManager.Instance.InitPreviewCubes();
                Round = 0;
                newState = GameState.WaitingForInput;
                break;
            case GameState.WaitingForInput:
                if (CheckGameOver())
                {
                    newState = GameState.Lose;
                }
                break;
            case GameState.Merging:
                break;            
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                break;
        }
        CurrentState = newState;
    }

    bool CheckGameOver()
    {
        var nodes = GridManager.Instance.GetNodes();
        int gridWidth = GridManager.Instance.Width; // Assuming your grid width is 5
        int gridHeight = GridManager.Instance.Height;
        int value = CubeManager.Instance.GetIncomingCube()[Round].Value;
        for (int x = 0; x < gridWidth; x++)
        {
            Node highestNode = nodes[x, gridHeight - 1]; // Get the node at the top row and current column
            if (highestNode.OccupiedCube == null) return false;
            /* If 1 of 5 column have the same value of the current cube, its not GAMEOVER */
            if (highestNode.OccupiedCube.Value == value) return false;
        }
        return true;
        
    }
}
