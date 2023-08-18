using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private Cube occupiedCube;

    public Cube OccupiedCube
    {
        get => occupiedCube;
        set
        {
            occupiedCube = value;
        }
    }
    [SerializeField] Cube cubePrefab;
    private void OnMouseDown()
    {
        //GameState Waiting For Input
        Debug.Log("Node: X: " + transform.position.x);
        Debug.Log(CubeManager.Instance.GetIncomingCube()[GameManager.Instance.Round].Value);
        SpawnCubeAtHighest();
    }

    //TODO: Object pooling 
    void SpawnCubeAtHighest()
    {
        /* Get the value of the incoming cube for the current round */
        int value = CubeManager.Instance.GetIncomingCube()[GameManager.Instance.Round].Value;

        /* Get the highest node in the column */
        Node highestNode = GridManager.Instance.GetHighestNodeInColumn((int)transform.position.x);

        /* Check if it's not possible to spawn a cube at the highest node then return */
        if (!CanSpawnAtHighestNode(highestNode, value)) return;

        Vector3 spawnPos = new Vector3(transform.position.x, GridManager.Instance.Height - 1, transform.position.z);

        var spawnedCube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

        /* Initialize the cube's type based on the incoming cube's value */
        spawnedCube.InitCubeType(value);
        /* Check if the node can fall down or not*/
        if (!spawnedCube.CanFallDown())
        {
            /* Assign the spawned cube to the highest node */
            highestNode.OccupiedCube = spawnedCube;
            /* If it cant fall then we will change its state to checking*/
            spawnedCube.ChangeCubeState(CubeState.Checking);
        }
        else
        {
            /* If it falls then we will change its state to falling*/
            spawnedCube.ChangeCubeState(CubeState.Falling);
        }

        
        
    }

    /* Check if it's possible to spawn a cube at the highest node */
    bool CanSpawnAtHighestNode(Node highestNode, int value)
    {
        /* Check if the highest node is unoccupied and the existing cube, if any, has the same value */
        return !highestNode.OccupiedCube || highestNode.OccupiedCube.Value == value;
    }


}
