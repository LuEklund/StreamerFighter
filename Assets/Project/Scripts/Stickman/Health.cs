using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Stickman {
    public class Health : MonoBehaviour {
        public Slider m_healthSlider;
        
        public float m_currentHealth = 10;
        public float m_maxHealth = 10;
        public float m_damageCooldown = 1f;
        
        bool m_canTakeDamage = true;
        public bool CanTakeDamage => m_canTakeDamage;
        float m_lastDamageTime;

        void Awake() {
            m_healthSlider.maxValue = m_maxHealth;
            m_healthSlider.value = m_maxHealth;
        }

        void Update() {
            if ( m_canTakeDamage == false ) {
                if ( Time.time - m_lastDamageTime > m_damageCooldown ) {
                    m_canTakeDamage = true;
                }
            }
        }

        public void TakeDamage(float inDamage) {
            if (!m_canTakeDamage) return;
            
            m_currentHealth -= inDamage;
            m_healthSlider.value = m_currentHealth;
            m_canTakeDamage = false;
            m_lastDamageTime = Time.time;

            if ( m_currentHealth <= 0 ) {
                Destroy( gameObject );
            }
        }
    }
}