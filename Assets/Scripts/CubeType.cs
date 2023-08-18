using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Cube Type")]
public class CubeType : ScriptableObject
{
    public int Value;
    public Material material;
    public float SpawnChance;
}
