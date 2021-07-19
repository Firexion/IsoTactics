using Events;
using Unit;
using UnityEngine;

namespace Attack
{
    public abstract class Attack : MonoBehaviour
    {
        public int range;
        public int heightAllowance;
        public int damage;

        public GameEvent damageDone;

        public abstract void FindSelectableTiles();
        protected abstract void RemoveSelectableTiles();
        
        protected void DoDamage(GameObject target)
        {
            target.GetComponent<RuntimeUnitStats>().TakeDamage(damage);
            RemoveSelectableTiles();
            damageDone.Raise();
        }
    }
}