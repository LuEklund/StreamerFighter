using UnityEngine;
using UnityEngine.UI;

namespace Stickman {
    public class Health : MonoBehaviour
    {
        public float currentHealth = 10;
        public float maxHealth = 10;
        public Slider healthSlider;

        private void Awake()
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        public void TakeDamage(float inDamage)
        {
            currentHealth -= inDamage;
            healthSlider.value = currentHealth;
            Debug.Log("Health: " + currentHealth);
        }
    }
}
