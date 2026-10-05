using UnityEngine;

public class Health : MonoBehaviour
{
    public int enemyHealth;
    public int friendliesHealth;
    public int enemyMaxHealth = 15;
    public int friendliesMaxHealth = 25;

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
