using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14;

    public bool turn = false;

    protected enum State
    {
        Falling,
        Jumping,
        MovingToEdge,
        None
    };

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;

    public bool moving = false;
    public int moveRange = 5;
    public float jumpHeight = 2;
    public float moveSpeed = 2;
    public float jumpVelocity = 4.5f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight = 0;

    protected State state = State.None;
    Vector3 jumpTarget;

    public Tile actualTargetTile;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;
        TurnManager.AddUnit(this);
    }

    private void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    protected static Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    public void ComputeAdjacencyLists(float jumpHeight, Tile target)
    {
        //tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (var tile in tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyLists(jumpHeight, null);
        GetCurrentTile();

        var process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        //currentTile.parent = ??  leave as null 

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < moveRange)
            {
                foreach (var tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        var next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    protected void Move()
    {
        if (path.Count > 0)
        {
            var t = path.Peek();
            var target = t.transform.position;

            // Calculate the unit's position
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;
            // ignore jumping and falling for now...complicated

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                if (transform.position.y != target.y)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }

                // Locomotion (Add animation here)
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // Tile centered reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;
            TurnManager.EndTurn();
        }
    }

    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (var tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    void Jump(Vector3 target)
    {
        // State machine
        switch (state)
        {
            case (State.Falling):
                FallDownward(target);
                break;
            case (State.Jumping):
                JumpUpward(target);
                break;
            case (State.MovingToEdge):
                MoveToEdge();
                break;
            default:
                PrepareJump(target);
                break;
        }
    }

    void PrepareJump(Vector3 target)
    {
        var targetY = target.y;
        target.y = transform.position.y;
        CalculateHeading(target);

        // Going down
        if (transform.position.y > targetY)
        {
            state = State.MovingToEdge;
            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        }
        else // Going up
        {
            state = State.Jumping;

            velocity = heading * moveSpeed / 3.0f;

            var difference = targetY - transform.position.y;
            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            state = State.None;

            var p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            state = State.Falling;
        }
    }

    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            state = State.Falling;

            velocity /= 4.0f;
            velocity.y = 1.5f;
        }
    }

    protected Tile FindLowestF(List<Tile> list)
    {
        var lowest = list[0];
        foreach (var t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);

        return lowest;
    }

    /**
     * Checks if the NPC can reach the end tile based on the movement range
     */
    protected Tile FindEndTile(Tile t)
    {
        var tempPath = new Stack<Tile>();
        var next = t.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        // End tile is reachable.
        if (tempPath.Count <= moveRange)
        {
            return t.parent;
        }

        // End tile isn't reachable. Find closest tile to end that is reachable
        Tile endTile = null;
        for (var i = 0; i <= moveRange; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected void FindPath(Tile target)
    {
        ComputeAdjacencyLists(jumpHeight, target);
        GetCurrentTile();

        var openList = new List<Tile>();
        var closedList = new List<Tile>();

        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            var t = FindLowestF(openList);
            closedList.Add(t);

            // Reached the target, Just move there
            if (t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            // Didn't reach the tile, need to check all the adjacent tiles to this one
            foreach (var tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    // Do nothing, already processed
                }
                else if (openList.Contains(tile))
                {
                    // Already on the list but check if this one is a faster path
                    var tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    if (tempG >= tile.g) continue;

                    tile.parent = t;
                    tile.g = tempG;
                    tile.f = tile.g + tile.h;
                }
                else
                {
                    // Calc ghf and add to open list
                    tile.parent = t;
                    var position = tile.transform.position;
                    tile.g = t.g + Vector3.Distance(position, t.transform.position);
                    tile.h = Vector3.Distance(position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }
        }

        // TODO What if there is no path to target tile
        Debug.Log("Path not found");
    }

    public void BeginTurn()
    {
        turn = true;
    }

    public void EndTurn()
    {
        turn = false;
    }
}