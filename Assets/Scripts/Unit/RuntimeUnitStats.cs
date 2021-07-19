using UnityEngine;

namespace Unit
{
    public class RuntimeUnitStats : MonoBehaviour
    {
        private int _maxHealth;
        private int _currentHealth;
        
        public HealthBarUI healthbar;
        public UnitObject unit;
        private void Start()
        {
            _maxHealth = unit.Hp;
            _currentHealth = unit.Hp;
            healthbar.SetMaxHealth(_maxHealth);
        }

        public void TakeDamage(int damage)
        {
            Debug.Log("Taking " + damage + " damage");
            _currentHealth -= damage;
            healthbar.SetHealth(_currentHealth);

            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}