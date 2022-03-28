
using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private bool isTesting;
    [SerializeField] private Vector3 testPos;
    private float time = 0f;
    private void Update()
    {
        if (!isTesting) return;

        rot();

        if (time < Time.time)
        {
            time = Time.time + turret.speed;
            shoot();
        }
        
    }
    
    private void shoot()
    {
        switch (turret.shootType)
        {
            case TurretStruct.ShootType.Basic:

                SpawnBullet(0f);
                
                break;
            
            case TurretStruct.ShootType.Blast:

                for (int x = 0; x < turret.bulletsInShoot; x++)
                {
                    SpawnBullet(turret.spread * x - turret.spread / 2 * turret.bulletsInShoot);
                }
                
                break;
            
            case TurretStruct.ShootType.MiniGun:
                
                SpawnBullet(Random.Range(-turret.maxSpread, turret.maxSpread));
                break;
        }
    }

    private void rot()
    {
        Debug.Log(testPos);

        Vector3 pos = testPos;
        
        float angle = Mathf.Atan2(pos.x, -pos.y) * Mathf.Rad2Deg;
        rigidBodyTurret.rotation = angle;
    }
    
    //Game

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, turret.maxRange);
        if (_nearestEnemyGameObject == null) return;
        Gizmos.DrawLine(transform.position, _nearestEnemyGameObject.transform.position);
    }
    
    private EnemySpawner _enemySpawner;
    private float _nearestEnemy = float.MinValue;
    private GameObject _nearestEnemyGameObject;
    public TurretStruct turret;
    [SerializeField] private Rigidbody2D rigidBodyTurret;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private GameObject bulletPrefab;
    private Collider2D[] nearestEnemies = new Collider2D[25];
    private Transform bulletsTransform;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        if (isTesting) return;

        Random.InitState(PlayerPrefs.GetInt("Seed")); 
        bulletPrefab.GetComponent<Stats>().health = turret.damage;
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        InvokeRepeating(nameof(GetEnemy), 0f, 1f);
        InvokeRepeating(nameof(Rotate), 0f, 0.1f);
        StartCoroutine(nameof(Shoot));
        bulletsTransform = GameObject.FindGameObjectWithTag("Bullets").transform;
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(turret.speed);

            if (_nearestEnemyGameObject != null)
            {
                switch (turret.shootType)
                {
                    case TurretStruct.ShootType.Basic:

                        SpawnBullet(0f);
                        
                        break;
                    
                    case TurretStruct.ShootType.Blast:

                        for (int x = 0; x < turret.bulletsInShoot; x++)
                        {
                            SpawnBullet(turret.spread * x - turret.spread / 2 * turret.bulletsInShoot);
                        }
                        
                        break;
                    
                    case TurretStruct.ShootType.MiniGun:
                        
                        SpawnBullet(Random.Range(-turret.maxSpread, turret.maxSpread));
                        break;
                }
            }
        }
    }

    private void SpawnBullet(float angle)
    {
        GameObject inst;
        if (bulletsTransform != null) inst = Instantiate(bulletPrefab, bulletsTransform);
        else inst = Instantiate(bulletPrefab);
        Rigidbody2D rigidBodyInst = inst.GetComponent<Rigidbody2D>();
        inst.transform.position = bulletSpawn.position;
        rigidBodyInst.rotation = rigidBodyTurret.rotation + angle;
        float a = rigidBodyInst.rotation + 180;
        rigidBodyInst.velocity = new Vector2(Mathf.Sin(-a * Mathf.Deg2Rad), Mathf.Cos(-a * Mathf.Deg2Rad)) * 32;
        Destroy(inst, turret.bulletTime);
        
    }

    private void Rotate()
    {
        if (_nearestEnemyGameObject == null) return;
        Vector3 enemyPos = transform.position - _nearestEnemyGameObject.transform.position;
        float angle = Mathf.Atan2(enemyPos.x, enemyPos.y) * Mathf.Rad2Deg;
        rigidBodyTurret.rotation = -angle;
    }
    
    private void GetEnemy()
    {
        _nearestEnemy = float.MaxValue;
        _nearestEnemyGameObject = null;

        Physics2D.OverlapCircleNonAlloc(transform.position, turret.maxRange, nearestEnemies, 6);
        
        for (var x = 0; x < nearestEnemies.Length; x++)
        {
            if (nearestEnemies[x] == null) return;
            var enemyPos = nearestEnemies[x].gameObject.transform.position;
            enemyPos.x += 150;
            enemyPos.y += 150;
            var enemyDist = Vector3.Distance(transform.position, enemyPos);
            if (enemyDist < turret.maxRange && _nearestEnemy > enemyDist)
            {
                _nearestEnemy = enemyDist;
                _nearestEnemyGameObject = _enemySpawner.enemies[x].Enemy;
            }
        }
    }
}
