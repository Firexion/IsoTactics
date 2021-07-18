using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using Turn;
using UnityEngine;

namespace DefaultNamespace
{
    public class SelectableTiles : MonoBehaviour
    {
        private const int MoveStraightCost = 10;
        private Tile _currentTile;
        private readonly List<Tile> _selectableTiles = new List<Tile>();
        
        public TileRuntimeSet tiles;
        
        public void Find(int range, int heightAllowance, bool allowOccupied)
        {
            ComputeAdjacencyLists(null, heightAllowance, allowOccupied);
            GetCurrentTile();

            var process = new Queue<Tile>();

            process.Enqueue(_currentTile);
            _currentTile.visited = true;

            while (process.Count > 0)
            {
                var t = process.Dequeue();

                _selectableTiles.Add(t);
                t.selectable = true;

                if (t.distance >= range) continue;
                foreach (var tile in t.adjacencyList.Where(tile => !tile.visited))
                {
                    tile.parent = t;
                    tile.visited = true;
                    tile.distance = 1 + t.distance;
                    process.Enqueue(tile);
                }
            }

            _currentTile.selectable = false;
        }

        public void ComputeAdjacencyLists(Tile target, int heightAllowance, bool allowOccupied)
        {
            foreach (var tile in tiles.Items)
            {
                tile.FindNeighbors(heightAllowance, target, allowOccupied);
            }
        }

        public Tile GetCurrentTile()
        {
            _currentTile = GetTargetTile(gameObject);
            _currentTile.current = true;
            
            return _currentTile;
        }

        public static Tile GetTargetTile(GameObject target)
        {
            Tile tile = null;

            if (Physics.Raycast(target.transform.position, -Vector3.up, out var hit, 1))
            {
                tile = hit.collider.GetComponent<Tile>();
            }

            return tile;
        }
        
        public void Remove()
        {
            if (_currentTile != null)
            {
                _currentTile.current = false;
                GetCurrentTile();
            }

            foreach (var tile in _selectableTiles)
            {
                tile.Reset();
            }

            _selectableTiles.Clear();
        }
    }
}