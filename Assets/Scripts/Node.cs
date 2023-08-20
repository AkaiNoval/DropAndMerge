using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private Cube occupiedCube;
    public static Action DelaySpammingTime;
    public Cube OccupiedCube { get => occupiedCube; set => occupiedCube = value; }
    [SerializeField] Cube cubePrefab;
    private void OnEnable()=> DelaySpammingTime += InCreaseSpawnTime;

    private void OnDisable() => DelaySpammingTime -= InCreaseSpawnTime;
    public float lastSpawnTime { get; set; } // Store the time of the last cube spawn

    private float spawnCooldown = 0.2f; // The minimum time between cube spawns in seconds
    private void OnMouseDown()
    {
        if(GameManager.Instance.CurrentState == GameState.WaitingForInput)
        {
            if (Time.time - lastSpawnTime >= spawnCooldown)
            {
                //GameState Waiting For Input
                if (!SpawnCubeAtHighest()) return;
                lastSpawnTime = Time.time;
                GameManager.Instance.Round++;
            }
        }
    }

    //TODO: Object pooling 
    bool SpawnCubeAtHighest()
    {
        /* Get the value of the incoming cube for the current round */
        int value = CubeManager.Instance.GetIncomingCube()[GameManager.Instance.Round].Value;

        /* Get the highest node in the column */
        Node highestNode = GridManager.Instance.GetHighestNodeInColumn((int)transform.position.x);

        /* Check if it's not possible to spawn a cube at the highest node then return */
        if (!CanSpawnAtHighestNode(highestNode, value)) return false;
        /*Increase the time before receiving next input*/
        InCreaseSpawnTime();
        Vector3 spawnPos = new Vector3(transform.position.x, GridManager.Instance.Height - 1, transform.position.z);

        var spawnedCube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

        /* Initialize the cube's type based on the incoming cube's value */
        if (ShouldMergeWithHighestNode(highestNode, value))
        {
            value *= 2;
            highestNode.OccupiedCube.SetNode(null);
            Destroy(highestNode.OccupiedCube.gameObject);
                 
        }
        spawnedCube.InitCubeType(value);
        /* Check if the cube can fall down or not*/
        if (spawnedCube.CanFallDown())
        {
            /* If it falls then we will change its state to falling*/
            spawnedCube.ChangeCubeState(CubeState.Falling);
        }
        else
        {
            /* Assign the spawned cube to the highest node */
            highestNode.OccupiedCube = spawnedCube;
            spawnedCube.SetNode(highestNode);
            /* If it cant fall then we will change its state to checking*/
            spawnedCube.ChangeCubeState(CubeState.Checking);
            Debug.Log("Look like i can spawn on the top if this column");
        }
        return true;
    }

    /* Check if it's possible to spawn a cube at the highest node */
    bool CanSpawnAtHighestNode(Node highestNode, int value)
    {
        /* Check if the highest node is unoccupied */
        return !highestNode.OccupiedCube || highestNode.OccupiedCube.Value == value;
    }
    bool ShouldMergeWithHighestNode(Node highestNode, int value)
    {
        if (highestNode.OccupiedCube != null)
        {
            return highestNode.OccupiedCube.Value == value;
        }
        return false;
    }

    void InCreaseSpawnTime()=> lastSpawnTime = Time.time + spawnCooldown;
}
