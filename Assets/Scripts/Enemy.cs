using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float startHealth;
    public float health;
    public float speed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Stats blockStats = other.gameObject.GetComponent<Stats>();
        if (!blockStats) return;
        var hp = blockStats.health;
        health -= hp;
        if (health > 0) blockStats.health -= health;
        if (health < 0) blockStats.health += health;

        if (health > startHealth)
        {
            Destroy(gameObject);
        }
        
        if (blockStats.health <= 0)
        {
            Destroy(other.gameObject);
            BuildManager.getMap()[Mathf.RoundToInt(other.transform.position.x - 0.5f), Mathf.RoundToInt(other.transform.position.y - 0.5f)].Occupied = false;
        }
        
        if (health <= 0 && gameObject) Destroy(gameObject);
    }
    
}
