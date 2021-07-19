using DefaultNamespace;
using Events;
using Unit;
using UnityEngine;

namespace Attack
{
    public class Attack : MonoBehaviour
    {
        public int range;
        public int heightAllowance;
        public int damage;

        private SelectableTiles _selectableTiles;
        public GameEvent damageDone;

        protected virtual void Awake()
        {
            _selectableTiles = GetComponent<SelectableTiles>();
        }

        public virtual void FindSelectableTiles()
        {
            _selectableTiles.Find(range, heightAllowance, true);
        }

        protected void DoDamage(GameObject target)
        {
            target.GetComponent<RuntimeUnitStats>().TakeDamage(damage);
            _selectableTiles.Remove();
            damageDone.Raise();
        }
    }
}