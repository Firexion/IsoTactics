using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMoveController : MoveController
    {
        [SerializeField] private bool invertControls = true;
        [SerializeField] private float tileMoveFrequency = 0.15f;
        [SerializeField] private float initialTileMoveDelay = 0.3f;

        private Tile _currentlySelectedTile;
        private float _startTileMoveTime;

        private void Start()
        {
            Init();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!turnTaker.turn || !canMove) return;
            if (moving) Move();
        }

        private void FixedUpdate()
        {
            MoveSelection();
        }
        
        public override void StartTurn()
        {
            base.StartTurn();
            UIController.ShowMenu();
            GetCurrentTile();
            _currentlySelectedTile = currentTile;
            _currentlySelectedTile.currentlySelecting = true;
        }

        public override void EndTurn()
        {
            base.EndTurn();
            if (_currentlySelectedTile == null) return;
            _currentlySelectedTile.currentlySelecting = false;
            _currentlySelectedTile = null;
        }

        public override void FinishedMoving()
        {
            base.FinishedMoving();
            UIController.ShowMenu();
            UIController.DisableMove();
            GetCurrentTile();
            _currentlySelectedTile = currentTile;
            _currentlySelectedTile.currentlySelecting = true;
        }

        public override void FindSelectableTiles()
        {
            base.FindSelectableTiles();
            _currentlySelectedTile = currentTile;
            _currentlySelectedTile.currentlySelecting = true;
        }


        public override void OnSelect(InputAction.CallbackContext context)
        {
            if (!turnTaker.turn || !context.started) return;
            if (_currentlySelectedTile.current || !_currentlySelectedTile.selectable) return;
            StartMove(_currentlySelectedTile);
        }

        /**
         * This moves the Selection reticule one tile on a single press
         * If the button is held down it moves more tiles after a small delay
         */
        private void MoveSelection()
        {
            if (!turnTaker.turn) return;
            if (Time.fixedTime - _startTileMoveTime < tileMoveFrequency) return;
            _startTileMoveTime = Time.fixedTime;
            var inputMovement = InputController.playerActions.Tile.Move.ReadValue<Vector2>();
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

        public override void OnMove(InputAction.CallbackContext context)
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

        public override void OnClick(InputAction.CallbackContext context)
        {
            if (!turnTaker.turn || !context.ReadValueAsButton()) return; // Only want mouse-down
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit) || !hit.collider.CompareTag("Tile")) return;

            var t = hit.collider.GetComponent<Tile>();
            if (!t.selectable) return;
            StartMove(t);
        }

        private void StartMove(Tile tile)
        {
            _currentlySelectedTile.currentlySelecting = false;
            tile.currentlySelecting = false;
            tile.target = true;
            
            MoveToTile(tile);
        }
    }
}