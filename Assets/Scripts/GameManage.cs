using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BlockStats
{
    public int gold;
    public int maxGold;
    public int amethyst;
    public int maxAmethyst;
    public int claimZone;
    public String color;
}

public class GameManage : MonoBehaviour
{
    public enum IconStat
    {
        Square,
        Shape,
        None
    }
    
    public enum Category
    {
        Defenses,
        Buildings,
        Other
    }
    
    public enum Tile
    {
        Basic,
        Gold,
        Amethyst,
        All
    }

    public static GameManage instance;
    public static Vector2Int mapSize = new Vector2Int(300, 300);
    public int gold;
    public int maxGold;
    public int amethyst;
    public int maxAmethyst;
    public Tile[,] tileMap;
    public int enemyCount;
    public int blockCount;
    public Stats Base;
    public OneBlock block;
    private static GameObject Square; 
    private static GameObject Shape;
    
    private static Dictionary<OneBlock.BlockType, int> typeCount = new Dictionary<OneBlock.BlockType, int>();

    public static int getTypeCount(OneBlock.BlockType type)
    {
        if (!typeCount.ContainsKey(type))
        {
            typeCount.Add(type, 0);
            return 0;
        }

        return typeCount[type];
    }

    public static Vector2Int getMapSize()
    {
        return new Vector2Int(mapSize.x, mapSize.y);
    }

    private static BlockStats[] blocks =
    {
        new BlockStats {gold = 0, maxGold = 0, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //None
        
        new BlockStats {gold = 65, maxGold = 0, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //White Turret
        new BlockStats {gold = 110, maxGold = 0, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //Old Turret
        
        new BlockStats {gold = 65, maxGold = 250, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //Gold Miner
        new BlockStats {gold = 35, maxGold = 1500, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //Gold WareHouse
        
        new BlockStats {gold = 5, maxGold = 0, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //Stone Wall
        new BlockStats {gold = 15, maxGold = 0, amethyst = 0, maxAmethyst = 0, claimZone = 0}, //Iron Wall
        
        new BlockStats {gold = 175, maxGold = 0, amethyst = 0, maxAmethyst = 0, claimZone = 5, color = "97dde2"} //Electro Tower
    };
    
    private static IconStat[] icons =
    {
        IconStat.None, //None
        
        IconStat.Shape, //White Turret
        IconStat.Shape, //Old Turret
        
        IconStat.None, //Gold Miner
        IconStat.None, //Gold WareHouse
        
        IconStat.None, //Stone Wall
        IconStat.None, //Iron Wall
        
        IconStat.Square, //Electro Tower
        IconStat.Square, //Improved Electro Tower
        IconStat.Square //Improved Electro Tower Future
    };

    public static BlockStats GetBlockStats(OneBlock.BlockType type)
    {
        return blocks[(int)type]; //Return Block Stats
    }
    
    public static GameObject GetIcon(OneBlock.BlockType type)
    {
        GameObject pref = null;
        if (icons[(int) type] == IconStat.Shape) pref = Shape;
        if (icons[(int) type] == IconStat.Square) pref = Square;
        
        return pref; //Return Block Stats
    }
    
    private void Awake()
    {
        instance = this;
        Base = GameObject.FindWithTag("base").GetComponent<Stats>(); //Get a Base
    }
}
