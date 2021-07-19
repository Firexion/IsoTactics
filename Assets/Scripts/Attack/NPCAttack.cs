using DefaultNamespace;

namespace Attack
{
    public class NPCAttack : Attack
    {
        private SelectableTiles _selectableTiles;
        private void Awake()
        {
            _selectableTiles = GetComponent<SelectableTiles>();
        }
        
        public override void FindSelectableTiles()
        {
            _selectableTiles.Find(range, heightAllowance, true);
        }

        protected override void RemoveSelectableTiles()
        {
            _selectableTiles.Remove();
        }
    }
}