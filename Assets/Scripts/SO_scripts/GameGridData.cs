using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGridData", menuName = "SO/Grid/Data", order = 51)]
public class GameGridData : ScriptableObject
{
    [SerializeField] List<GameObject> _generatedCells = new List<GameObject>();

    public List<GameObject> GeneratedCells
    {
        get => _generatedCells;
        set => _generatedCells = value;
    }
}
