using UnityEngine;

public class Health : MonoBehaviour
{
    public int enemyHealth;
    public int friendliesHealth;
    public int enemyMaxHealth = 50;
    public int friendliesMaxHealth = 100;

    public void TakeDamage(int amount)
    {
        enemyHealth -= amount;
        friendliesHealth -= amount;

        if(enemyHealth <= 0 || friendliesMaxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
