using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    private Vector2Int mapSize = CONSTANTS.getMapSize();
    [SerializeField] private float spawnTimer , moveTimer;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Slider slider;
    [SerializeField] private Animator laserBeam;
    [SerializeField] private CameraManager cameraMan;
    public List<EnemyStruct> enemies = new List<EnemyStruct>();
    private Transform _tr;
    private GameManage gameManage;
    public int blocksBoss = 250;
    private int boss;
    
    private void Start()
    {
        gameManage = FindObjectOfType<GameManage>();
        _tr = transform;
        Random.InitState(PlayerPrefs.GetInt("Seed")); //Set and Get Seed
        StartCoroutine(SpawnEnemies()); //Start Coroutine
        StartCoroutine(MoveEnemies()); //Start Coroutine
    }

    private IEnumerator SpawnEnemies()
    {
        blocksBoss = 250;
        while (true)
        {
            yield return new WaitForSeconds(spawnTimer);
            GameObject prefab = null;
            int blocks = gameManage.blockCount;
            
            if (blocks <= 100)
            {
                prefab = enemyPrefabs[0];
            } else if (blocks >= 100 && blocks <= 200)
            {
                prefab = enemyPrefabs[Random.Range(0, 2)];
            } else if (blocks >= 200 && blocks <= 250)
            {
                prefab = enemyPrefabs[Random.Range(0, 3)];
            } else if (blocks >= 250 && blocks <= 350)
            {
                if (boss == 0)
                {
                    StartCoroutine(SpawnBoss(enemyPrefabs[3], 4));
                    blocksBoss = 999999;
                    boss = 1;
                }
                prefab = enemyPrefabs[4];
            } else if (blocks >= 350)
            {
                prefab = enemyPrefabs[Random.Range(4, 6)];
            }

            SpawnEnemy(prefab);
        }
    }
    
    private IEnumerator SpawnBoss(GameObject bossPrefab, int pos)
    {
        Vector3 spawnPos;
        
        switch (pos)
        { 
            case 1: //Left
                spawnPos = new Vector3(-mapSize.x * 0.5f, 0);
                break;
                
            case 2:  //Right
                spawnPos = new Vector3(mapSize.x * 0.5f, 0);
                break;
                
            case 3: //Down
                spawnPos = new Vector3(0, -mapSize.y * 0.5f);
                break;
                
            default: //Up
                spawnPos = new Vector3(0, mapSize.y * 0.5f);
                break;
        }
        
        GameObject inst = Instantiate(bossPrefab, _tr);
        Enemy comp = inst.GetComponent<Enemy>();
        inst.transform.localPosition = spawnPos;
        EnemyStruct boss = new EnemyStruct();
        boss.Position = spawnPos; //Set SpawnPos
        boss.Enemy = inst; //Set Prefab
        boss.Speed = comp.speed; //Set Speed
        enemies.Add(boss); //Add Boss
        gameManage.enemyCount = enemies.Count; //Get Count of Enemy
        
        laserBeam.gameObject.SetActive(true);
        laserBeam.Play("LaserBeam");

        cameraMan.StartCoroutine("bossSpawning");
        
        float maxHealth = comp.health;
        
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (inst == null)
            {
                slider.gameObject.SetActive(false);
                break;
            }
            slider.gameObject.SetActive(true);
            slider.maxValue = maxHealth;
            slider.value = comp.health;
        }
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        var rand = Random.Range(1, 5);
        Vector3 spawnPos;
            
        float rand2 = Mathf.RoundToInt(Random.Range(-6 * mapSize.x, 6 * mapSize.y)); //Round SpawnPos
        rand2 *= 0.08333f; //Get Pixel SpawnPos
        switch (rand)
        { 
            case 1: //Left
                spawnPos = new Vector3(-0.5f * mapSize.x, rand2);
                break;
                
            case 2:  //Right
                spawnPos = new Vector3(0.5f * mapSize.x, rand2);
                break;
                
            case 3: //Down
                spawnPos = new Vector3(rand2, -0.5f * mapSize.y);
                break;
                
            default: //Up
                spawnPos = new Vector3(rand2, 0.5f * mapSize.y);
                break;
        }
            
        GameObject inst = Instantiate(enemyPrefab, _tr);
        inst.transform.localPosition = spawnPos;
        EnemyStruct enem = new EnemyStruct();
        enem.Position = spawnPos; //Set SpawnPos
        enem.Enemy = inst; //Set Prefab
        enem.Speed = inst.GetComponent<Enemy>().speed; //Set Speed
        enemies.Add(enem); //Add Enemy
        gameManage.enemyCount = enemies.Count; //Get Count of Enemy
    }

    private IEnumerator MoveEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTimer);
            for (int x = 0; x < enemies.Count; x++)
            {
                EnemyStruct enemy = enemies[x];
                if (enemy.Enemy == null)
                {
                    enemies.Remove(enemy);
                    continue;
                }

                Transform entr = enemy.Enemy.transform;
                Vector2 locPos = entr.localPosition;
                float angle = Mathf.Atan2(locPos.y, locPos.x) * Mathf.Rad2Deg;

                enemy.Enemy.GetComponent<Rigidbody2D>().rotation = angle + 90;

                locPos.Normalize();
                enemy.Position -= locPos * 0.08333f * enemy.Speed;

                float newX = Mathf.Round(enemy.Position.x / 0.08333f) * 0.08333f;
                float newY = Mathf.Round(enemy.Position.y / 0.08333f) * 0.08333f;

                entr.localPosition = new Vector3(newX, newY);
                enemies[x] = enemy;
                
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < enemies.Count; x++)
        {
            EnemyStruct enemy = enemies[x];
            Gizmos.color = new Color(220, 0, 0, 255);
            Gizmos.DrawLine(enemy.Position + new Vector2(mapSize.x / 2, mapSize.y / 2), new Vector2(mapSize.x / 2, mapSize.y / 2));
            Gizmos.DrawWireSphere(enemy.Position + new Vector2(mapSize.x / 2, mapSize.y / 2), 1.5f);
        }
    }
}
