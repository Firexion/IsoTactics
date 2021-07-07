using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool walkable = true;

    public List<Tile> adjacencyList = new List<Tile>();

    // Needed for BFS (Breadth first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;  // how far each tile is from the start tile
    
    // Needed for A*
    public float f = 0;  // Cost from parent to current tile
    public float g = 0;  // Cost from processed tile to destination
    public float h = 0;  // Hueristic cost = G + H

    private Renderer selectableRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        selectableRenderer = transform.GetChild(0).GetComponent<Renderer>();
        selectableRenderer.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (current)
        {
            selectableRenderer.enabled = true;
            selectableRenderer.material.color = Color.magenta;
        }
        else if (target)
        {
            selectableRenderer.enabled = true;
            selectableRenderer.material.color = Color.green;
        }
        else if (selectable)
        {
            selectableRenderer.enabled = true;
            selectableRenderer.material.color = Color.blue;
        }
        else
        {
            selectableRenderer.enabled = false;
        }
    }

    public void Reset()
    {
        adjacencyList.Clear();
        current = false;
        target = false;
        selectable = false;
        walkable = true;
        visited = false;
        parent = null;
        distance = 0;
        f = g = h = 0;
    }

    public void FindNeighbors(float jumpHeight, Tile target)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight, target);
        CheckTile(-Vector3.forward, jumpHeight, target);
        CheckTile(Vector3.right, jumpHeight, target);
        CheckTile(-Vector3.right, jumpHeight, target);
    }

    public void CheckTile(Vector3 direction, float jumpHeight, Tile target)
    {
        var halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        var colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (var item in colliders)
        {
            var tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                {
                    adjacencyList.Add(tile);
                }
            }
        }
    }
}