using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth = 10;

    public void TakeDamage(float inDamage)
    {
        currentHealth -= inDamage;
        Debug.Log("Health: " + currentHealth);
    }
}
