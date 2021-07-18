using UnityEngine;

namespace Unit
{
    public class RuntimeUnitStats : MonoBehaviour
    {
        public int maxHealth;
        public int currentHealth;
        
        public HealthBarUI healthbar;
        public UnitObject unit;
        private void Start()
        {
            maxHealth = unit.Hp;
            currentHealth = unit.Hp;
            healthbar.SetMaxHealth(maxHealth);
        }

        private void Update()
        {
          
        }

        private void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthbar.SetMaxHealth(maxHealth);
        }
    }
}