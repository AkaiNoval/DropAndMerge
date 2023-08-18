using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    private static CubeManager instance;
    public static CubeManager Instance { get => instance; private set => instance = value; }

    /* Ref: Manager => to generate a whole list of cube type*/
    public List<CubeType> AllCubeTypes { get => allCubeTypes; private set => allCubeTypes = value; }

    [SerializeField] private List<CubeType> allCubeTypes;

    [SerializeField] CubePreview currentCubePrefab;
    [SerializeField] CubePreview incomingCubePrefab;

    [SerializeField] int incomingCubeAmount;
    [SerializeField] List<CubeType> incomingCubeType;


    void Awake()
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

    void Start()
    {
        GenerateIncomingCubeType();
    }
    void GenerateIncomingCubeType()
    {
        incomingCubeType = new List<CubeType>();

        List<CubeType> sortedCuteTypes = AllCubeTypes.OrderByDescending(bt => bt.SpawnChance).ToList();

        int x = 0;
        while (incomingCubeType.Count() < incomingCubeAmount)
        {
            float chance = Random.value;
            if (x < sortedCuteTypes.Count)
            {
                if (chance <= sortedCuteTypes[x].SpawnChance)
                {
                    incomingCubeType.Add(sortedCuteTypes[x]);
                    x = 0; //Reset the list
                }
                else if (chance > sortedCuteTypes[x].SpawnChance)
                {
                    x++;
                }
            }
            else 
            {
                x = 0; 
            }
        }
    }
    public List<CubeType> GetIncomingCube()=> incomingCubeType;
    public void InitPreviewCubes()
    {
        var gridManager = GridManager.Instance;
        var currentCube = Instantiate(currentCubePrefab, gridManager.GetCurrentNodePos().position + new Vector3(0, 0, -1f), Quaternion.identity);
        var incomingCube = Instantiate(incomingCubePrefab, gridManager.GetIncomingNodePos().position + new Vector3(0, 0, -1f), Quaternion.identity);

        Vector3 localScaleCurrentCube = gridManager.GetCurrentNodePos().localScale;
        Vector3 localScaleIncomingCube = gridManager.GetIncomingNodePos().localScale;
        currentCube.gameObject.transform.localScale = localScaleCurrentCube;
        incomingCube.gameObject.transform.localScale = localScaleIncomingCube;
    }


}
