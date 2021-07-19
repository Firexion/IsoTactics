
using Movement;

namespace Attack
{
    public class PlayerAttack : Attack
    {
        private TileInputController _tileMovementController;

        protected override void Awake()
        {
            _tileMovementController = GetComponent<TileInputController>();
        }

        public override void FindSelectableTiles()
        {
            _tileMovementController.FindSelectableTiles(range, heightAllowance, true, Type.Attack);
        }

        public void Attack(Tile tile)
        {
            if (tile.OccupiedBy == null) return;
            DoDamage(tile.OccupiedBy);
        }
    }
}