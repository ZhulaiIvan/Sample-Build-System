using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseBuilding : MonoBehaviour
{
    [SerializeField] internal BuildingTypes _myType = BuildingTypes.TypeA;
    [SerializeField] private Vector2Int _size = Vector2Int.one;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Canvas _canvas;
    [SerializeField] protected TMP_Text _TMP_Text;
    
    public Vector2Int Size
    {
        get => _size;
    }
    
    protected virtual void Start()
    {
        _canvas.transform.forward = Camera.main.transform.forward;
        // switch (_myType) // Ранее все дома имели общий единственный класс, где логика разделялась проверкой типа строения, а не отдельными классами
        // {
        //     case BuildingTypes.TypeB:
        //         _currentBuildingLevel = 1;
        //         _maxBuldingLevel = 5;
        //         _TMP_Text.text = _currentBuildingLevel + "/" + _maxBuldingLevel;
        //         break;
        //     default:
        //         break;
        // }
    }

    public void SetTransperent(bool available)
    {
        if (available)
            _renderer.material.color = Color.green;
        else
            _renderer.material.color = Color.red;
    }

    public virtual void ResetTransperent()
    {
        _renderer.material.color = _baseColor;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _size.x; i++)
        {
            for (int j = 0; j < _size.y; j++)
            {
                if ((i + j) % 2 == 0)
                    Gizmos.color = new Color(0.3f, 0.5f, 0.8f, 0.3f);
                else
                    Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.3f);
                Gizmos.DrawCube(transform.position + new Vector3(i,0,j), new Vector3(1, 0.1f, 1));
            }
        }
    }
}
