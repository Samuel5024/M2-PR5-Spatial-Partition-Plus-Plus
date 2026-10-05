using UnityEngine;

public class Damage : MonoBehaviour
{
    public int enemyDamage = 1;
    public int soldierDamage = 2;
    private Health health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Friendly" && health != null)
        {
            health.TakeDamage(enemyDamage);
        }
        if (other.gameObject.tag == "Enemy" && health != null)
        {
            health.TakeDamage(soldierDamage);
        }
        else
        {
            health = other.gameObject.gameObject.GetComponent<Health>();
        }
    }
}
