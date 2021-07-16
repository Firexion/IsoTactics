using System;
using DefaultNamespace;
using Events;
using Turn;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMoveController : MoveController, PlayerActions.ITileActions
    {
        [SerializeField] private bool invertControls = true;
        [SerializeField] private float tileMoveFrequency = 0.15f;
        [SerializeField] private float initialTileMoveDelay = 0.3f;

        public TileVariable currentlySelectedTile;
        private float _startTileMoveTime;

        public PlayerActionsVariable playerActions;
        public GameEvent finishedMoving;

        private void Update()
        {
            if (!IsActive() || !canMove) return;
            if (moving)
            {
                Move();
            }
        }

        private void FixedUpdate()
        {
            MoveSelection();
        }

        public override void StartTurn()
        {
            base.StartTurn();
            GetCurrentTile();
            currentlySelectedTile.SetValue(currentTile);
        }

        public override void EndTurn()
        {
            base.EndTurn();
            playerActions.Value.Tile.SetCallbacks(null);
            playerActions.Value.Tile.Disable();
            if (currentlySelectedTile.Value == null) return;
            currentlySelectedTile.Remove();
        }

        protected override void FinishedMoving()
        {
            base.FinishedMoving();
            finishedMoving.Raise();
            GetCurrentTile();
            currentlySelectedTile.SetValue(currentTile);
        }

        public override void FindSelectableTiles()
        {
            playerActions.Value.Tile.SetCallbacks(this);
            playerActions.Value.Tile.Enable();
            base.FindSelectableTiles();
            currentlySelectedTile.SetValue(currentTile);
        }


        public void OnSelect(InputAction.CallbackContext context)
        {
            if (!IsActive() || !context.started) return;
            if (currentlySelectedTile.Value.current || !currentlySelectedTile.Value.selectable) return;
            StartMove(currentlySelectedTile.Value);
        }

        /**
         * This moves the Selection reticule one tile on a single press
         * If the button is held down it moves more tiles after a small delay
         */
        private void MoveSelection()
        {
            //if (!IsActive()) return;
            if (Time.fixedTime - _startTileMoveTime < tileMoveFrequency) return;
            _startTileMoveTime = Time.fixedTime;
            var inputMovement = playerActions.Value.Tile.Move.ReadValue<Vector2>();
            if (inputMovement == Vector2.zero) return;

            var direction = new Vector3(inputMovement.x, 0, inputMovement.y);
            if (invertControls)
            {
                direction = new Vector3(inputMovement.y, 0, -inputMovement.x);
            }

            var halfExtents = new Vector3(0.25f, (1 + 999) / 2.0f, 0.25f);
            var colliders = Physics.OverlapBox(currentlySelectedTile.Value.transform.position + direction, halfExtents);

            foreach (var item in colliders)
            {
                var tile = item.GetComponent<Tile>();
                if (tile == null) continue;
                if (!tile.selectable && !tile.current) continue;
                currentlySelectedTile.SetValue(tile);
                break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _startTileMoveTime = Time.fixedTime + initialTileMoveDelay;
            if (!IsActive() || !context.performed) return;

            var inputMovement = context.ReadValue<Vector2>();
            var direction = new Vector3(inputMovement.x, 0, inputMovement.y);
            if (invertControls)
            {
                direction = new Vector3(inputMovement.y, 0, -inputMovement.x);
            }

            var halfExtents = new Vector3(0.25f, (1 + 999) / 2.0f, 0.25f);
            var colliders = Physics.OverlapBox(currentlySelectedTile.Value.transform.position + direction, halfExtents);

            foreach (var item in colliders)
            {
                var tile = item.GetComponent<Tile>();
                if (tile == null) continue;
                if (!tile.selectable && !tile.current) continue;
                currentlySelectedTile.SetValue(tile);
                break;
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
     //       if (!IsActive() || !context.ReadValueAsButton()) return; // Only want mouse-down
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit) || !hit.collider.CompareTag("Tile")) return;

            var t = hit.collider.GetComponent<Tile>();
            if (!t.selectable) return;
            StartMove(t);
        }

        private void StartMove(Tile tile)
        {
            tile.target = true;
            currentlySelectedTile.Remove();

            MoveToTile(tile);
        }

        public void DisableInput()
        {
            playerActions.Value.Tile.Disable();
        }
    }
}