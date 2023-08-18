using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;
    public static GridManager Instance { get => instance; private set => instance = value; }

    [SerializeField] private int width = 5;
    [SerializeField] private int height = 7;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private GameObject nodeParent;
    [SerializeField] private GameObject currentNodePreviewPrefab;
    [SerializeField] private MeshRenderer boardPrefab;
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }


    Node[,] nodes;
    Transform currentNodePos;
    Transform incomingNodePos;

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

    #region SceneSetUp
    public void GenerateGrid()
    {
        nodes = new Node[Width, Height]; // Initialize the 2D array
        for (int x = 0; x < Width; x++)
        {
            //CreateClickDetector(x);
            for (int y = 0; y < Height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                nodes[x, y] = node; // Assign to the 2D array
                node.name = $"Node x: {x} y: {y}";
                node.transform.SetParent(nodeParent.transform, false);
            }
        }
        /*Nodes are centered on each whole vector, so for this case the sprite will extend from 0,0 to -0.5 to 0.5*/
        Vector3 center = new Vector3((float)Width / 2 - 0.5f, (float)Height / 2 - 0.5f, 5f);
        PlacingBoardAndCubePreview(center);
        RepositionCamera(center);
    }
    void PlacingBoardAndCubePreview(Vector3 center)
    {
        var boardObject = Instantiate(boardPrefab, center, Quaternion.identity);
        var currentNode = Instantiate(currentNodePreviewPrefab, center, Quaternion.identity);
        var incomingNode = Instantiate(currentNodePreviewPrefab, center, Quaternion.identity);

        // Adjust the localScale of the boardObject
        boardObject.transform.localScale = new Vector3(Width + 0.5f, Height + 0.5f, 0.1f);

        // Additional positioning and adjustments as needed
        boardObject.transform.position = center;
        currentNode.transform.position = new Vector3(center.x, center.y + 5.0f, center.z);
        incomingNode.transform.position = new Vector3(center.x + 1f, center.y + 5.0f, center.z);
        incomingNode.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        currentNodePos = currentNode.transform;
        incomingNodePos = incomingNode.transform;
    }
    void RepositionCamera(Vector3 center) => Camera.main.transform.position = new Vector3(center.x, center.y, -10);
    #endregion

    public Transform GetCurrentNodePos() => currentNodePos;
    public Transform GetIncomingNodePos() => incomingNodePos;
    public Node[,] GetNodes() => nodes;
    public Node GetHighestNodeInColumn(int column)
    {
        Node highestNode = nodes[column, 0];
        for (int y = 0; y < Height; y++)
        {
            if (nodes[column, y].transform.position.y > highestNode.transform.position.y)
            {
                highestNode = nodes[column, y];
            }
        }
        return highestNode;
    }

    public Node GetLowestFreeNodeInColumn(int column)
    {
        Node lowestNode = null;
        for (int y = Height-1; y >= 0; y--)
        {
            if (!nodes[column, y].OccupiedCube)
            {
                lowestNode = nodes[column, y];
            }
        }
        return lowestNode;
    }

    public void AllDropDown()
    {
        for(int x=0;x <Width; x++)
        {
            for(int y=1; x< Height; y++)
            {
                if (nodes[x,y].OccupiedCube)
                {

                }
            }
        }
    }

    public Vector2Int GetNodePosition(Node targetNode)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (nodes[x, y] == targetNode)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        // Node not found, return an invalid position or handle it as needed
        return new Vector2Int(-1, -1);
    }
    //void DropDown()
    //{
    //    var width = GridManager.Instance.Width;
    //    var height = GridManager.Instance.Height;
    //    var nodes = GridManager.Instance.GetNodes();
    //    for (int x = 0; x < width; x++)
    //    {
    //        // Start from the second bottom row and go upwards
    //        for (int y = 1; y < height; y++)
    //        {
    //            if (nodes[x, y].OccupiedCube)
    //            {
    //                int targetY = y; //1

    //                while (targetY - 1 >= 0 && !nodes[x, targetY - 1].OccupiedCube)
    //                {
    //                    targetY--; //0
    //                }
    //                if (targetY != y)
    //                {
    //                    Cube cubeToMove = nodes[x, y].OccupiedCube;

    //                    // Set the target node's occupied block and trigger animation
    //                    nodes[x, targetY].OccupiedCube = cubeToMove;
    //                    //StartCoroutine(movingBlock.Animate(movingBlock, nodes[x, targetY]));

    //                    // Clear the current node's occupied block
    //                    nodes[x, y].OccupiedCube = null;

    //                    Debug.Log("Im " + nodes[x, y].name + " I just move down to" + nodes[x, targetY].name);
    //                }
    //            }
    //        }
    //    }
    //}
}
