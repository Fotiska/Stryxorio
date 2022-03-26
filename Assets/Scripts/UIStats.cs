using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class UIStats : MonoBehaviour
{
    [SerializeField] private Text gold, amethyst, health, blocks;
    private GameManage _gameManage;
    private EnemySpawner _enemySpawner;

    private void Awake()
    {
        _gameManage = FindObjectOfType<GameManage>(); //Get a Script GameManage
        _enemySpawner = FindObjectOfType<EnemySpawner>(); //Get a Script EnemySpawner
    }

    private void Start()
    {
        StartCoroutine(updateStats());
    }

    private IEnumerator updateStats()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            gold.text = _gameManage.gold + "/" + _gameManage.maxGold;
            amethyst.text = _gameManage.amethyst + "/" + _gameManage.maxAmethyst;
            health.text = _gameManage.Base.health + "/" + 100;
            blocks.text = _gameManage.blockCount + "/" + _enemySpawner.blocksBoss;

            if (_gameManage.Base.health <= 0)
            {
                EventManager.death.Invoke();
            }
            
        }
    }
}
