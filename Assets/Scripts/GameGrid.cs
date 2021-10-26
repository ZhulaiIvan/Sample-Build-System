using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class GameGrid : MonoBehaviour
{
    [SerializeField] private GameObject _gridCellPrefab;
    private BaseBuilding[,] _buildingGrid;
    private GameObject[,] _gameGrid;
    [SerializeField] private BaseBuilding _flyingBuilding;
    [SerializeField] private Transform buildingHolder;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _button1;//_button2
    private Camera mainCamera;
    private Dictionary<Vector2Int, bool> _cellDictionary = new Dictionary<Vector2Int, bool>();
    //private List<GameObject> _gridCells = new List<GameObject>();
    [SerializeField] private GameGridData _gameGridData;
    [Space(20)] //[Header("Grid Generator")]
    [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
    private Vector2Int _selectedArea = new Vector2Int();
    
    private void Awake()
    {
        _buildingGrid = new BaseBuilding[_gridSize.x, _gridSize.y];
        mainCamera = Camera.main;
    }

    private void Start()
    {
        CreateGrid();//Костыль, на тот случай, если _gridSize был изменен, но не сгенерирована новая сетка в Эдиторе (свойста бы в инспекторе использовать)
    }

    public void StartPlacingFlyingBuilding(BaseBuilding building)
    {
        if(_flyingBuilding != null)
            Destroy(_flyingBuilding.gameObject);
        _flyingBuilding = Instantiate(building, buildingHolder);
    }

    public void PlaceBuildingFromCell(BaseBuilding building)
    {
        var cloneBuilding = Instantiate(building, buildingHolder);
        for (int i = 0; i < cloneBuilding.Size.x; i++)
        {
            for (int j = 0; j < cloneBuilding.Size.y; j++)
            {
                _buildingGrid[_selectedArea.x + i, _selectedArea.y + j] = cloneBuilding;
                _cellDictionary[new Vector2Int(_selectedArea.x + i, _selectedArea.y + j)] = false;
            }
        }
        cloneBuilding.transform.position = new Vector3(_selectedArea.x, 0, _selectedArea.y);
        if (building._myType == BuildingTypes.TypeB)
        {
            TypeBBuilding tmp = (TypeBBuilding)cloneBuilding;
            tmp.IsBuildingPlaced = true;
        }
        _canvas.enabled = false;
        _button1.interactable = false;
    }
    
    public void CreateGrid()
    {
        foreach (var generatedCell in _gameGridData.GeneratedCells)
        {
            DestroyImmediate(generatedCell);
        }
        _cellDictionary.Clear();
        if(!Application.isPlaying)
            _gameGridData.GeneratedCells.Clear();
        //_gridCells.Clear();
        
        _gameGrid = new GameObject[_gridSize.x, _gridSize.y];
        if (_gridCellPrefab != null)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    _gameGrid[x, y] = Instantiate(_gridCellPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                    _gameGrid[x, y].gameObject.name = "Grid Space (X: " + x.ToString() + " , Y: " + y.ToString() + ")";
                    if(!Application.isPlaying)
                        _gameGridData.GeneratedCells.Add(_gameGrid[x, y]);
                    _cellDictionary.Add(new Vector2Int(x,y), true);
                    //_gridCells.Add(_gameGrid[x,y]);
                }
            }
        }
    }
    void Update()
    {
        if (_flyingBuilding != null)
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                bool available = true;

                if(x < 0 || x > _gridSize.x - _flyingBuilding.Size.x)
                    available = false;
                if(y < 0 || y > _gridSize.y - _flyingBuilding.Size.y)
                    available = false;
                
                if(available && IsPlaceTaken(x, y))
                    available = false;
                
                _flyingBuilding.transform.position = new Vector3(x, 0, y);
                _flyingBuilding.SetTransperent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x, y);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                OnMousePressed();
        }
    }
    
    private void OnMousePressed()
    {
        _button1.interactable = false;
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector2Int clickedPosition = new Vector2Int();
        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);

            int x = Mathf.RoundToInt(worldPosition.x);
            int y = Mathf.RoundToInt(worldPosition.z);
            clickedPosition = new Vector2Int(x, y);
        }
        _selectedArea = clickedPosition;
        if (_cellDictionary.ContainsKey(clickedPosition) && _cellDictionary[clickedPosition])
        {
            _canvas.enabled = true;
            _canvas.transform.forward = mainCamera.transform.forward;
            _canvas.transform.position = new Vector3(clickedPosition.x, 0.5f, clickedPosition.y);
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if(x == 0 && y == 0)
                        continue;
                    var tmp = new Vector2Int(clickedPosition.x + x, clickedPosition.y + y);
                    if (_cellDictionary.ContainsKey(tmp))
                    {
                        if (!_cellDictionary[tmp])
                            return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            _button1.interactable = true;
        }
        else
        {
            _canvas.enabled = false;
            _button1.interactable = false;
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int i = 0; i < _flyingBuilding.Size.x; i++)
        {
            for (int j = 0; j < _flyingBuilding.Size.y; j++)
            {
                if (_buildingGrid[placeX + i, placeY + j] != null)
                    return true;
            }
        }

        return false;
    }
    
    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int i = 0; i < _flyingBuilding.Size.x; i++)
        {
            for (int j = 0; j < _flyingBuilding.Size.y; j++)
            {
                _buildingGrid[placeX + i, placeY + j] = _flyingBuilding;//Можно заменить и использовать словарь _cellDictionary вместо _buildingGrid
                _cellDictionary[new Vector2Int(placeX + i, placeY + j)] = false;
            }
        }
        
        _flyingBuilding.ResetTransperent();
        _flyingBuilding = null;
    }
}
