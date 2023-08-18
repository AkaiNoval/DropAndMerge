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
    [SerializeField] private Node node;
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
    CubeType FindCubeTypeBasedOnValue(int value) => CubeManager.Instance.AllCubeTypes.Find(cubeType => cubeType.Value == value);
    public bool CanFallDown()
    {
        Node lowestNode = GridManager.Instance.GetLowestFreeNodeInColumn((int)transform.position.x);
        if (lowestNode)
        {
            transform.DOMoveY(lowestNode.transform.position.y, 0.3f)
                .SetEase(Ease.InCirc)
                .OnComplete(() =>
                {
                    lowestNode.OccupiedCube = this;
                    SetNode(lowestNode);
                    ChangeCubeState(CubeState.Checking);
                });
            return true;
        }
        return false;
    }
    public void SetNode(Node newNode)
    {
        this.node = newNode;
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
                //Check => Change other IDLE to Merging state => Change my value => if i can fall => Check again => NO? = change state to Idle
                break;
            case CubeState.Merging:
                break;
            default:
                break;
        }
        currentCubeState = newState;
    }
    void UpdateCubeBasedMergingValue()
    {
        int times = CheckForMerging();
        InitCubeType(Value * CalculateValueBasedOnMergeTime(times));
    }
    void DoTweenMoveMergingNode(Node targetNode)
    {
        ChangeCubeState(CubeState.Merging);
        node.OccupiedCube = null;
        SetNode(null);

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetNode.transform.position, 0.3f).SetEase(Ease.InOutCirc));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    int CheckForMerging()
    {
        /* Get the 2D array of nodes from the grid manager */
        var nodeArray = GridManager.Instance.GetNodes();

        /* Get the height and width of the grid */
        var Height = GridManager.Instance.Height;
        var Width = GridManager.Instance.Width;
        /* Counter to track the number of same cube for merging */
        int sameCube = 1;
        /* Function to check if two blocks can be merged */
        bool CanMerge(Cube otherCube) => otherCube != null && otherCube.Value == this.Value;

        int nodePosX = GridManager.Instance.GetNodePosition(node).x;
        int nodePosY = GridManager.Instance.GetNodePosition(node).y;
        /* Check above */
        /* Sidenode, looks like there are absolutely no chance of "above" to happen*/
        if (nodePosY  > 0)
        {
            Cube aboveCube = nodeArray[nodePosX, nodePosY - 1].OccupiedCube;
            if (CanMerge(aboveCube))
            {
                aboveCube.DoTweenMoveMergingNode(node);
                sameCube++;
            }
        }
        /* Check below */
        if (nodePosY < Height - 1)
        {
            Cube belowCube = nodeArray[nodePosX, nodePosY + 1].OccupiedCube;
            if (CanMerge(belowCube))
            {
                belowCube.DoTweenMoveMergingNode(node);
                sameCube++;
            }
        }

        /* Check left */
        if (nodePosX > 0)
        {
            Cube leftNode = nodeArray[nodePosX - 1, nodePosY].OccupiedCube;
            if (CanMerge(leftNode))
            {
                leftNode.DoTweenMoveMergingNode(node);
                sameCube++;
            }
        }

        /* Check right */
        if (nodePosX < Width - 1)
        {
            Cube rightNode = nodeArray[nodePosX + 1, nodePosY].OccupiedCube;
            if (CanMerge(rightNode))
            {
                rightNode.DoTweenMoveMergingNode(node);
                sameCube++;
            }
        }
        /* Return true if at least two same blocks are found for merging */
        return sameCube;
    }
    int CalculateValueBasedOnMergeTime(int times) => times <= 4 ? 1 << (times - 1) : 1;
}
