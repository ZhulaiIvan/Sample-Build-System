using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
    private BaseBuilding[,] _grid;
    [SerializeField] private BaseBuilding _flyingBuilding;
    [SerializeField] private Transform buildingHolder;
    private Camera mainCamera;
    [SerializeField] private Texture gridImage;
    

    private void Awake()
    {
        _grid = new BaseBuilding[_gridSize.x, _gridSize.y];
        mainCamera = Camera.main;
        
        // MeshRenderer gridRenderer = GetComponent<MeshRenderer>();
        // gridRenderer.material.mainTexture = gridImage;
        // gridRenderer.material.mainTextureScale = new Vector2(_gridSize.x, _gridSize.y);
        // gridRenderer.material.mainTextureOffset = new Vector2(.5f, .5f);
    }

    public void StartPlacingBuilding(BaseBuilding building)
    {
        if(_flyingBuilding != null)
            Destroy(_flyingBuilding.gameObject);
        _flyingBuilding = Instantiate(building, buildingHolder);
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
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int i = 0; i < _flyingBuilding.Size.x; i++)
        {
            for (int j = 0; j < _flyingBuilding.Size.y; j++)
            {
                if (_grid[placeX + i, placeY + j] != null)
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
                _grid[placeX + i, placeY + j] = _flyingBuilding;
            }
        }
        
        _flyingBuilding.ResetTransperent();
        _flyingBuilding = null;
    }
}
