using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeBBuilding : BaseBuilding
{
    [SerializeField] private Collider _collider;
    //[SerializeField] private MeshFilter _meshFilter;
    //[SerializeField] private List<Mesh> _meshes;//должен меняться меш, а не префабы примитивов
    [SerializeField] private GameObject _button;
    [SerializeField] private int _currentBuildingLevel = 1;
    [SerializeField] private int _maxBuldingLevel = 5;
    private bool _isBuildingPlaced = false;
    public bool IsBuildingPlaced
    {
        set => _isBuildingPlaced = value;
    }
    
    //private bool _isSelected;
    
    protected override void Start()
    {
        base.Start();
        _TMP_Text.text = _currentBuildingLevel + "/" + _maxBuldingLevel;
    }
    
    public override void ResetTransperent()
    {
        base.ResetTransperent();
        _isBuildingPlaced = true;
    }
    
    private void ToggleButton()
    {
        if (_button.activeSelf)
            _button.SetActive(false);
        else
            _button.SetActive(true);
    }
    
    public void IncreaseLevel()
    { 
        if (_currentBuildingLevel < _maxBuldingLevel)
        {
            _currentBuildingLevel += 1;
            //_meshFilter.mesh = _meshes[_currentBuildingLevel];
            _TMP_Text.text = _currentBuildingLevel + "/" + _maxBuldingLevel;
        }
    }
    
    private void Update()
    {
        if (_isBuildingPlaced && Input.GetMouseButtonDown(0))// && _myType == BuildingTypes.TypeB
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (_collider.Raycast(ray, out hit, 100) && !_button.activeSelf)
            {
                ToggleButton();
            }
            else if (!_collider.Raycast(ray, out hit, 100) && _button.activeSelf)
            {
                ToggleButton();
            }
        }
    }
}
