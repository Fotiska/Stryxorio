using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class GameManage : MonoBehaviour
{
    public int gold;
    public int maxGold;
    public int amethyst;
    public int maxAmethyst;
    public int enemyCount;
    public int blockCount;
    public Stats Base;
    public OneBlock block;

    private void Awake()
    {
        Base = GameObject.FindWithTag("base").GetComponent<Stats>(); //Get a Base
    }
}
