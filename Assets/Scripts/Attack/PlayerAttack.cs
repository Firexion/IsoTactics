
using Movement;

namespace Attack
{
    public class PlayerAttack : Attack
    {
        private TileInputController _tileMovementController;

        private void Awake()
        {
            _tileMovementController = GetComponent<TileInputController>();
        }

        public override void FindSelectableTiles()
        {
            _tileMovementController.FindSelectableTiles(range, heightAllowance, true, Type.Attack);
        }

        protected override void RemoveSelectableTiles()
        {
            _tileMovementController.RemoveSelectableTiles();
        }

        public void Attack(Tile tile)
        {
            if (tile.OccupiedBy == null) return;
            DoDamage(tile.OccupiedBy);
        }
    }
}