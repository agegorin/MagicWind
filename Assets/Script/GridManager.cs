using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private int gridCols;
    private int gridRows;
    private Vector3 gridPos;
    private Vector3 gridOffset;
    private Vector3 gridCellSize;

    void Start()
    {
        SpriteRenderer gridRenderer = GetComponent<SpriteRenderer>();
        gridCols = (int)gridRenderer.size.x;
        gridRows = (int)gridRenderer.size.y;
        float gridHeight = gridRenderer.bounds.size.y;
        float gridWidth = gridRenderer.bounds.size.x;
        gridPos = new Vector3(gameObject.transform.position.x - gridWidth / 2, gameObject.transform.position.y + gridHeight / 2, 0);
        gridCellSize = new Vector3(gridWidth / gridCols, gridHeight / gridRows, 0);
        gridOffset = new Vector3(gridCellSize.x / 2, -gridCellSize.y / 2, 0);
    }

    public Vector3 toWorldPos(Vector2Int pos)
    {
        return new Vector3(gridCellSize.x * pos[0], gridCellSize.y * pos[1] * -1f, 0) + gridPos + gridOffset;
    }

    public Vector2Int toGridPos(Vector3 pos)
    {

        return new Vector2Int(Mathf.FloorToInt((pos.x - gridPos.x) / gridCellSize.x), Mathf.FloorToInt((gridPos.y - pos.y) / gridCellSize.y));
    }

    public bool isInGrid(Vector2Int coord)
    {
        if (
            coord.x >= 0 && coord.x < gridCols &&
            coord.y >= 0 && coord.y < gridRows
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int getRandomCol()
    {
        return Random.Range(0, gridCols);
    }

    public int getGridCols()
    {
        return gridCols;
    }

    public int getGridRows()
    {
        return gridRows;
    }
}
