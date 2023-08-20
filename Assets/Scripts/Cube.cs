using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEditor.Experimental.GraphView;

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
    [SerializeField] private Node cubeNode;
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
    public void SetNode(Node newNode) => this.cubeNode = newNode;

    public void ChangeCubeState(CubeState newState)
    {
        switch (newState)
        {
            case CubeState.Idle:
                GameManager.Instance.ChanceState(GameState.WaitingForInput);
                break;
            case CubeState.Falling:
                GameManager.Instance.ChanceState(GameState.Merging);
                break;
            case CubeState.Checking:
                GameManager.Instance.ChanceState(GameState.Merging);
                UpdateCubeBasedOnMergingValue();
                //Check => Change other IDLE to Merging state => Change my value => if i can fall => Check again => NO? = change state to Idle
                break;
            case CubeState.Merging:
                GameManager.Instance.ChanceState(GameState.Merging);
                break;
            default:
                break;
        }
        currentCubeState = newState;
    }
    void UpdateCubeBasedOnMergingValue()
    {
        int times = CheckForMerging(true);
        int newValue = Value * CalculateValueBasedOnMergeTime(times);
        InitCubeType(newValue);
        var sequence1 = DOTween.Sequence();
        sequence1.Append(transform.DOScale(transform.localScale + new Vector3(0.3f, 0.3f, 0.3f), 0.15f));
        sequence1.Append(transform.DOScale(transform.localScale, 0.1f).SetEase(Ease.OutSine));
        sequence1.OnComplete(() => 
        {
            Node lowestNode = GridManager.Instance.GetLowestFreeNodeInColumn((int)transform.position.x);
            if (lowestNode.transform.position.y >= transform.position.y)
            {
                if (CheckForMerging(false) != 1)
                {
                    ChangeCubeState(CubeState.Checking);
                }
            }
            GridManager.Instance.AllDropDown();
        });
        GameManager.Instance.ChanceState(GameState.WaitingForInput);
    }
    void DoTweenMoveMergingNode(Node targetNode)
    {
        ChangeCubeState(CubeState.Merging);
        cubeNode.OccupiedCube = null;
        SetNode(null);
        Node.DelaySpammingTime?.Invoke();
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetNode.transform.position, 0.2f).SetEase(Ease.InOutCirc));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    public void DoTweenMoveToAnotherNode(Node targetNode)
    {
        ChangeCubeState(CubeState.Falling);
        /* Clear the current node you are sitting on */
        cubeNode.OccupiedCube = null;
        /* Set the target nodes cube to this */
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetNode.transform.position, 0.2f).SetEase(Ease.OutQuad));
        /* for the cube that moved check for merging again*/
        cubeNode = targetNode;
        targetNode.OccupiedCube = this;
        sequence.OnComplete(() =>
        {
            ChangeCubeState(CubeState.Idle);
            GridManager.Instance.AllDropDown();
            if (CheckForMerging(false) == 1) return;
            ChangeCubeState(CubeState.Checking);
        });
    }
    int CheckForMerging(bool shouldMergeNeighbourCube)
    {
        /* Get the 2D array of nodes from the grid manager */
        var nodeArray = GridManager.Instance.GetNodes();

        /* Get the height and width of the grid */
        var Height = GridManager.Instance.Height;
        var Width = GridManager.Instance.Width;
        /* Counter to track the number of same cube for merging */
        int sameCube = 1;
        /* Function to check if two cubes can be merged */
        bool CanMerge(Cube otherCube) => otherCube != null && otherCube.Value == this.Value;
        /* Function to merging neighbourCube */
        void MergeNeighbourCube(Cube neighbourCube)
        {
            if (!shouldMergeNeighbourCube) return;
            neighbourCube.DoTweenMoveMergingNode(cubeNode);
        }
        int nodePosX = GridManager.Instance.GetNodePosition(cubeNode).x;
        int nodePosY = GridManager.Instance.GetNodePosition(cubeNode).y;
        /* Check above */
        /* Sidenode, looks like there are absolutely no chance of "above" to happen*/
        if (nodePosY  > 0)
        {
            Cube aboveCube = nodeArray[nodePosX, nodePosY - 1].OccupiedCube;
            if (CanMerge(aboveCube))
            {
                MergeNeighbourCube(aboveCube);
                sameCube++;
            }
        }
        /* Check below */
        if (nodePosY < Height - 1)
        {
            Cube belowCube = nodeArray[nodePosX, nodePosY + 1].OccupiedCube;
            if (CanMerge(belowCube))
            {
                MergeNeighbourCube(belowCube);
                sameCube++;
            }
        }

        /* Check left */
        if (nodePosX > 0)
        {
            Cube leftNode = nodeArray[nodePosX - 1, nodePosY].OccupiedCube;
            if (CanMerge(leftNode))
            {
                MergeNeighbourCube(leftNode);
                sameCube++;
            }
        }

        /* Check right */
        if (nodePosX < Width - 1)
        {
            Cube rightNode = nodeArray[nodePosX + 1, nodePosY].OccupiedCube;
            if (CanMerge(rightNode))
            {
                MergeNeighbourCube(rightNode);
                sameCube++;
            }
        }
        /* Return true if at least two same blocks are found for merging */
        return sameCube;
    }
    int CalculateValueBasedOnMergeTime(int times) => times <= 4 ? 1 << (times - 1) : 1;
}
