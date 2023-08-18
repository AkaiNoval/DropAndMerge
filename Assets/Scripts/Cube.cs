using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum CubeState
{
    Idle,
    Falling,
    Checking,
    Merging
}
public class Cube : MonoBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private CubeState currentCubeState;

    private MeshRenderer meshRenderer;
    private TextMeshPro textMeshPro; 
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        textMeshPro = GetComponentInChildren<TextMeshPro>();
    }

    public void InitCubeType(int value)
    {
        CubeType type = FindCubeTypeBasedOnValue(value);

        if (type != null)
        {
            //this.cubeType = type;
            this.meshRenderer.material = type.material;
            this.textMeshPro.text = type.Value.ToString();
            this.Value = value;
        }
        else
        {
            this.meshRenderer.material = new Material(Shader.Find("Standard"));
            this.meshRenderer.material.color = Color.black;
            this.textMeshPro.text = value.ToString();
            this.Value = value;
        }
    }
    private CubeType FindCubeTypeBasedOnValue(int value) => CubeManager.Instance.AllCubeTypes.Find(cubeType => cubeType.Value == value);
    public bool CanFallDown()
    {
        Node lowestNode = GridManager.Instance.GetLowestFreeNodeInColumn((int)transform.position.x);
        if (lowestNode)
        {
            transform.DOMoveY(lowestNode.transform.position.y, 0.3f)
                .SetEase(Ease.InCirc)
                .OnComplete(() => lowestNode.OccupiedCube = this);
            return true;
        }
        return false;
    }

    public void ChangeCubeState(CubeState newState)
    {
        switch (newState)
        {
            case CubeState.Idle:
                break;
            case CubeState.Falling:
                break;
            case CubeState.Checking:
                break;
            case CubeState.Merging:
                break;
            default:
                break;
        }
        currentCubeState = newState;
    }
    int CalculateValueBasedOnMergeTime(int times) => times <= 4 ? 1 << (times - 1) : 1;

    int CheckForMerging(Cube occupiedCube, int posX, int posY)
    {
        /* Get the 2D array of nodes from the grid manager */
        var nodeArray = GridManager.Instance.GetNodes();

        /* Get the height and width of the grid */
        var Height = GridManager.Instance.Height;
        var Width = GridManager.Instance.Width;

        /* Counter to track the number of same blocks for merging */
        int sameCube = 1;

        /* Function to check if two blocks can be merged */
        bool CanMerge(Cube otherCube) => otherCube != null && otherCube.Value == occupiedCube.Value;

        /* Check for merging with neighboring blocks */
        /* Check above */
        if (posY > 0)
        {
            if (CanMerge(nodeArray[posX, posY - 1].OccupiedCube))
            {

                sameCube++;
            }

        }

        /* Check below */
        if (posY < Height - 1)
        {
            if (CanMerge(nodeArray[posX, posY + 1].OccupiedCube))
            {
                sameCube++;
            }

        }

        /* Check left */
        if (posX > 0)
        {
            if (CanMerge(nodeArray[posX - 1, posY].OccupiedCube))
            {
                sameCube++;
            }

        }

        /* Check right */
        if (posX < Width - 1)
        {
            if (CanMerge(nodeArray[posX + 1, posY].OccupiedCube))
            {
                sameCube++;
            }

        }

        /* Return true if at least two same blocks are found for merging */
        return sameCube;
    }

}
