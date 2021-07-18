using System.Collections.Generic;
using DefaultNamespace;
using Turn;
using UnityEngine;

namespace Movement
{
    public abstract class MoveController : MonoBehaviour
    {
        protected const int MoveStraightCost = 10;
        private const int MoveDiagonalCost = 14;

        protected TurnTaker turnTaker;
        
        public TurnTakerVariable activeTurnTaker;
        protected enum State
        {
            Falling,
            Jumping,
            MovingToEdge,
            None
        };


        private readonly Stack<Tile> _path = new Stack<Tile>();

        public bool moving;
        public bool canMove = true;
        public float moveSpeed = 2;
        public float jumpVelocity = 4.5f;

        private Vector3 _velocity;
        private Vector3 _heading;

        private float _halfHeight;

        protected State state = State.None;
        private Vector3 _jumpTarget;

        public Tile actualTargetTile;
        protected SelectableTiles SelectableTiles; 

        protected void Awake()
        {
            SelectableTiles = GetComponent<SelectableTiles>();
            turnTaker = GetComponent<TurnTaker>();
            _halfHeight = GetComponent<Collider>().bounds.extents.y;
        }

        public virtual void StartTurn()
        {
            canMove = true;
            moving = false;
        }

        public virtual void EndTurn()
        {
            canMove = false;
            moving = false;
        }

        protected void MoveToTile(Tile tile)
        {
            _path.Clear();
            tile.target = true;
            moving = true;

            var next = tile;
            while (next != null)
            {
                _path.Push(next);
                next = next.parent;
            }
        }

        protected void Move()
        {
            if (_path.Count > 0)
            {
                var t = _path.Peek();
                var target = t.transform.position;

                // Calculate the unit's position
                target.y += _halfHeight + t.GetComponent<Collider>().bounds.extents.y;
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
                    transform.forward = _heading;
                    transform.position += _velocity * Time.deltaTime;
                }
                else
                {
                    // Tile centered reached
                    transform.position = target;
                    _path.Pop();
                }
            }
            else
            {
                FinishedMoving();
            }
        }

        protected virtual void FinishedMoving()
        {
            SelectableTiles.Remove();
            moving = false;
            canMove = false;
        }
       

        private void CalculateHeading(Vector3 target)
        {
            _heading = target - transform.position;
            _heading.Normalize();
        }

        private void SetHorizontalVelocity()
        {
            _velocity = _heading * moveSpeed;
        }

        private void Jump(Vector3 target)
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

        private void PrepareJump(Vector3 target)
        {
            var targetY = target.y;
            target.y = transform.position.y;
            CalculateHeading(target);

            // Going down
            if (transform.position.y > targetY)
            {
                state = State.MovingToEdge;
                _jumpTarget = transform.position + (target - transform.position) / 2.0f;
            }
            else // Going up
            {
                state = State.Jumping;

                _velocity = _heading * moveSpeed / 3.0f;

                var difference = targetY - transform.position.y;
                _velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
            }
        }

        private void FallDownward(Vector3 target)
        {
            _velocity += Physics.gravity * Time.deltaTime;

            if (!(transform.position.y <= target.y)) return;
            state = State.None;

            var p = transform.position;
            p.y = target.y;
            transform.position = p;

            _velocity = new Vector3();
        }

        private void JumpUpward(Vector3 target)
        {
            _velocity += Physics.gravity * Time.deltaTime;

            if (transform.position.y > target.y)
            {
                state = State.Falling;
            }
        }

        private void MoveToEdge()
        {
            if (Vector3.Distance(transform.position, _jumpTarget) >= 0.05f)
            {
                SetHorizontalVelocity();
            }
            else
            {
                state = State.Falling;

                _velocity /= 4.0f;
                _velocity.y = 1.5f;
            }
        }

        private static Tile FindLowestF(IList<Tile> list)
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
        private Tile FindEndTile(Tile t)
        {
            var tempPath = new Stack<Tile>();
            var next = t.parent;
            while (next != null)
            {
                tempPath.Push(next);
                next = next.parent;
            }

            // End tile is reachable.
            if (tempPath.Count <= turnTaker.Stats.unit.Move / MoveStraightCost)
            {
                return t.parent;
            }

            // End tile isn't reachable. Find closest tile to end that is reachable
            Tile endTile = null;
            for (var i = 0; i <= turnTaker.Stats.unit.Move / MoveStraightCost; i++)
            {
                endTile = tempPath.Pop();
            }

            return endTile;
        }

        protected void FindPath(Tile target)
        {
            SelectableTiles.ComputeAdjacencyLists(target, turnTaker.Stats.unit.Jump, false);
            var currentTile = SelectableTiles.GetCurrentTile();

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

        protected bool IsActive()
        {
            return activeTurnTaker.IsActive(turnTaker);
        }

        public virtual void FindSelectableTiles()
        {
            SelectableTiles.Find(turnTaker.Stats.unit.Move / MoveStraightCost, turnTaker.Stats.unit.Jump, false);
        }
    }
}