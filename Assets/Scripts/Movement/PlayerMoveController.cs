using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMoveController : MoveController, PlayerActions.ITileActions
    {
        [SerializeField] private Camera cam;
        [SerializeField] private bool invertControls = true;
        [SerializeField] private float tileMoveFrequency = 0.15f;
        [SerializeField] private float initialTileMoveDelay = 0.3f;
        private PlayerActions _playerActions;

        private Tile _currentlySelectedTile;
        private float _startTileMoveTime;

        private void Start()
        {
            _playerActions = new PlayerActions();
            _playerActions.Tile.SetCallbacks(this);
            _playerActions.Enable();
            Init();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!turnTaker.turn) return;
            UIController.ShowMenu();
            if (!canMove) return;

            if (moving)
            {
                Move();
            }
            else if (selectableTiles.Count == 0)
            {
                FindSelectableTiles();
                if (_currentlySelectedTile != null) return;
                _currentlySelectedTile = currentTile;
                _currentlySelectedTile.currentlySelecting = true;
            }
            
        }

        private void FixedUpdate()
        {
            MoveSelection();
        }


        public void OnSelect(InputAction.CallbackContext context)
        {
            if (!turnTaker.turn || !context.started) return;
            if (_currentlySelectedTile.current || !_currentlySelectedTile.selectable) return;
            MoveToTile(_currentlySelectedTile);
        }

        private void MoveSelection()
        {
            if (!turnTaker.turn) return;
            if (Time.fixedTime - _startTileMoveTime < tileMoveFrequency) return;
            _startTileMoveTime = Time.fixedTime;
            var inputMovement = _playerActions.Tile.Move.ReadValue<Vector2>();
            if (inputMovement == Vector2.zero) return;
            
            var direction = new Vector3(inputMovement.x, 0, inputMovement.y);
            if (invertControls)
            {
                direction = new Vector3(inputMovement.y, 0, -inputMovement.x);
            }
            
            var halfExtents = new Vector3(0.25f, (1 + 999) / 2.0f, 0.25f);
            var colliders = Physics.OverlapBox(_currentlySelectedTile.transform.position + direction, halfExtents);

            foreach (var item in colliders)
            {
                var tile = item.GetComponent<Tile>();
                if (tile == null) continue;
                _currentlySelectedTile.currentlySelecting = false;
                _currentlySelectedTile = tile;
                _currentlySelectedTile.currentlySelecting = true;
                break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _startTileMoveTime = Time.fixedTime + initialTileMoveDelay;
            if (!turnTaker.turn || !context.performed) return;
            
            var inputMovement = context.ReadValue<Vector2>();
            var direction = new Vector3(inputMovement.x, 0, inputMovement.y);
            if (invertControls)
            {
             direction = new Vector3(inputMovement.y, 0, -inputMovement.x);
            }
            
            var halfExtents = new Vector3(0.25f, (1 + 999) / 2.0f, 0.25f);
            var colliders = Physics.OverlapBox(_currentlySelectedTile.transform.position + direction, halfExtents);
            
            foreach (var item in colliders)
            {
                var tile = item.GetComponent<Tile>();
                if (tile == null) continue;
                _currentlySelectedTile.currentlySelecting = false;
                _currentlySelectedTile = tile;
                _currentlySelectedTile.currentlySelecting = true;
                break;
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!turnTaker.turn || !context.ReadValueAsButton()) return; // Only want mouse-down
            var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit) || !hit.collider.CompareTag("Tile")) return;

            var t = hit.collider.GetComponent<Tile>();
            if (!t.selectable) return;
            MoveToTile(t);
            _currentlySelectedTile = t;
            _currentlySelectedTile.currentlySelecting = true;
        }
    }
}