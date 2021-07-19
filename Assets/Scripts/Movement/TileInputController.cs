using System;
using Attack;
using DefaultNamespace;
using Events;
using Turn;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public enum Type
    {
        Movement,
        Attack
    }
    public class TileInputController : MonoBehaviour, PlayerActions.ITileActions
    {
        public PlayerActionsVariable playerActions;
        public TurnTakerVariable activeTurnTaker;
        public TileVariable currentlySelectedTile;
        public GameEvent actionCancelled;
        
        [SerializeField] private bool invertControls = true;
        [SerializeField] private float tileMoveFrequency = 0.15f;
        [SerializeField] private float initialTileMoveDelay = 0.3f;
        
        private float _startTileMoveTime = -1f;
        private TurnTaker _turnTaker;
        private SelectableTiles _selectableTiles;
        private Type _type;
        private PlayerMoveController _movementController;
        private PlayerAttack _attackController;

        private void Awake()
        {
            _turnTaker = GetComponent<TurnTaker>();
            _selectableTiles = GetComponent<SelectableTiles>();
            _movementController = GetComponent<PlayerMoveController>();
            _attackController = GetComponent<PlayerAttack>();
        }
        
        private void FixedUpdate()
        {
            if (_startTileMoveTime == -1f) return;
            var inputMovement = playerActions.Value.Tile.Move.ReadValue<Vector2>();
            if (inputMovement == Vector2.zero)
            {
                _startTileMoveTime = -1f;
                return;
            }
            if (Time.fixedTime - _startTileMoveTime < tileMoveFrequency) return;
            _startTileMoveTime = Time.fixedTime;
            MoveSelection(inputMovement);
        }
        
        /**
         * This moves the Selection reticule one tile on a single press
         * If the button is held down it moves more tiles after a small delay
         */
        private void MoveSelection(Vector2 inputMovement)
        {
            
            Debug.Log("Doing Movement");
            var direction = new Vector3(inputMovement.x, 0, inputMovement.y);
            if (invertControls)
            {
                direction = new Vector3(inputMovement.y, 0, -inputMovement.x);
            }
            
            Debug.Log("Direction: " + direction);

            var halfExtents = new Vector3(0.25f, (1 + 999) / 2.0f, 0.25f);
            var colliders = Physics.OverlapBox(currentlySelectedTile.Value.transform.position + direction, halfExtents);

            foreach (var item in colliders)
            {
                var tile = item.GetComponent<Tile>();
                if (tile == null) continue;
                if (!tile.selectable && !tile.current) continue;
                currentlySelectedTile.SetValue(tile);
                Debug.Log("Moved to tile at: " + tile.transform.position);
                break;
            }
        }
        
        public void FindSelectableTiles(int range, int heightAllowance, bool allowOccupied, Type type)
        {
            _type = type;
            playerActions.Value.Tile.SetCallbacks(this);
            playerActions.Value.Tile.Enable();
            _selectableTiles.Find(range, heightAllowance, allowOccupied);
            currentlySelectedTile.SetValue(_selectableTiles.GetCurrentTile());
        }
        
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (!IsActive() || !context.started) return;
            if (currentlySelectedTile.Value.current || !currentlySelectedTile.Value.selectable) return;
            SelectTile(currentlySelectedTile.Value);
        }

        private void SelectTile(Tile tile)
        {
            switch (_type)
            {
                case Type.Movement:
                    _movementController.StartMove(tile);
                    break;
                case Type.Attack:
                    _attackController.Attack(tile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!IsActive() || !context.performed) return;
            if (_startTileMoveTime != -1f) return;
            _startTileMoveTime = Time.fixedTime + initialTileMoveDelay;
            var inputMovement = playerActions.Value.Tile.Move.ReadValue<Vector2>();
            if (inputMovement == Vector2.zero)
            {
                _startTileMoveTime = -1f;
                return;
            }
            MoveSelection(inputMovement);
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!IsActive() || !context.ReadValueAsButton()) return; // Only want mouse-down
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit) || !hit.collider.CompareTag("Tile")) return;

            var t = hit.collider.GetComponent<Tile>();
            if (!t.selectable) return;
            SelectTile(t);
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            _selectableTiles.Remove();
            playerActions.Value.Tile.SetCallbacks(null);
            playerActions.Value.Tile.Disable();
            actionCancelled.Raise();
        }
        
        private bool IsActive()
        {
            return activeTurnTaker.IsActive(_turnTaker);
        }

        public void StartTurn()
        {
            var currentTile = _selectableTiles.GetCurrentTile();
            currentlySelectedTile.SetValue(currentTile);
        }

        public void EndTurn()
        {
            playerActions.Value.Tile.SetCallbacks(null);
            playerActions.Value.Tile.Disable();
            if (currentlySelectedTile.Value == null) return;
            currentlySelectedTile.Remove();
        }

        public void DisableInput()
        {
            playerActions.Value.Tile.Disable();
        }

        public void RemoveSelectableTiles()
        {
            _selectableTiles.Remove();
        }
    }
}