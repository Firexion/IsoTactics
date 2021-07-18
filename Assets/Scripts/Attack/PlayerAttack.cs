using DefaultNamespace;
using Movement;
using Turn;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Attack
{
    public class PlayerAttack : Attack, PlayerActions.ITileActions
    {
        public PlayerActionsVariable playerActions;
        public TileVariable currentlySelectedTile;
        public TurnTakerVariable activeTurnTaker;
        private TurnTaker _turnTaker;

        private float _startTileMoveTime;
        [SerializeField] private bool invertControls = true;
        [SerializeField] private float tileMoveFrequency = 0.15f;
        [SerializeField] private float initialTileMoveDelay = 0.3f;

        private void Awake()
        {
            _turnTaker = GetComponent<TurnTaker>();
            SelectableTiles = GetComponent<SelectableTiles>();
        }

        private void FixedUpdate()
        {
            if (Time.fixedTime - _startTileMoveTime < tileMoveFrequency) return;
            _startTileMoveTime = Time.fixedTime;
            MoveSelection();
        }

        /**
         * This moves the Selection reticule one tile on a single press
         * If the button is held down it moves more tiles after a small delay
         */
        private void MoveSelection()
        {
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

        public override void FindSelectableTiles()
        {
            playerActions.Value.Tile.SetCallbacks(this);
            playerActions.Value.Tile.Enable();
            SelectableTiles.Find(range, heightAllowance, true);
            currentlySelectedTile.SetValue(SelectableTiles.GetCurrentTile());
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (!IsActive() || !context.started) return;
            if (currentlySelectedTile.Value.current || !currentlySelectedTile.Value.selectable) return;
            DoDamage(currentlySelectedTile.Value.OccupiedBy);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _startTileMoveTime = Time.fixedTime + initialTileMoveDelay;
            if (!IsActive() || !context.performed) return;
            MoveSelection();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!IsActive() || !context.ReadValueAsButton()) return; // Only want mouse-down
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit) || !hit.collider.CompareTag("Tile")) return;

            var t = hit.collider.GetComponent<Tile>();
            if (!t.selectable) return;
            DoDamage(t.OccupiedBy);
        }

        private bool IsActive()
        {
            return activeTurnTaker.IsActive(_turnTaker);
        }
    }
}