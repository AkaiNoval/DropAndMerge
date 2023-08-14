using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 7;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private GameObject nodeParent;
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }


    Node[,] nodes;

    public Node[,] GetNodes() => nodes;
    private void Start()
    {
        GenerateGrid();
    }
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
        Vector2 center = new Vector2((float)Width / 2 - 0.5f, (float)Height / 2 - 0.5f);
        //RescalingBoard(center);
        //RepositionCamera(center);
        //CreateBlockHolders();
        //SetBlockShowCasePosition();
    }
}
