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

        protected SelectableTiles SelectableTiles;
        public GameEvent damageDone;

        protected void Awake()
        {
            SelectableTiles = GetComponent<SelectableTiles>();
        }

        public virtual void FindSelectableTiles()
        {
            SelectableTiles.Find(range, heightAllowance, true);
        }

        public void DoDamage(GameObject target)
        {
            target.GetComponent<RuntimeUnitStats>().TakeDamage(damage);
            SelectableTiles.Remove();
            damageDone.Raise();
        }
    }
}