using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 1;
    private Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Friendly" || other.gameObject.tag == "Enemy")
        {
            if(health == null)
            {
                health = other.gameObject.gameObject.GetComponent<Health>();
            }
            health.TakeDamage(damage);
        }
    }
}
