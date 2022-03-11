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

            Vector2 pos = other.transform.position;
            
            Vector2Int mapPos = new Vector2Int(Mathf.RoundToInt(pos.x - 0.5f), Mathf.RoundToInt(pos.y - 0.5f));
            
            Claiming.getInstance().claimZone(mapPos, 6, false, BuildManager.getId(other.gameObject));
            
            Destroy(other.gameObject);
            
            BuildManager.getMap()[mapPos.x, mapPos.y].Occupied = false;
        }
        
        if (health <= 0 && gameObject) Destroy(gameObject);
    }
    
}
