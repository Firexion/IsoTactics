using Events;

namespace Movement
{
    public class PlayerMoveController : MoveController
    {
        public TileVariable currentlySelectedTile;
        public GameEvent finishedMoving;
        private TileInputController _tileMovementController;

        protected override void Awake()
        {
            base.Awake();
            _tileMovementController = GetComponent<TileInputController>();
        }

        private void Update()
        {
            if (!IsActive() || !canMove) return;
            if (moving)
            {
                Move();
            }
        }

        public override void StartTurn()
        {
            base.StartTurn();
            _tileMovementController.StartTurn();
        }

        public override void EndTurn()
        {
            base.EndTurn();
            _tileMovementController.EndTurn();
        }

        protected override void FinishedMoving()
        {
            base.FinishedMoving();
            finishedMoving.Raise();
            var currentTile = SelectableTiles.GetCurrentTile();
            currentlySelectedTile.SetValue(currentTile);
        }

        public override void FindSelectableTiles()
        {
            _tileMovementController.FindSelectableTiles(turnTaker.Stats.unit.Move / MoveStraightCost, turnTaker.Stats.unit.Jump, false, Type.Movement);
        }

        public void StartMove(Tile tile)
        {
            _tileMovementController.DisableInput();
            tile.target = true;
            currentlySelectedTile.Remove();

            MoveToTile(tile);
        }

    }
}