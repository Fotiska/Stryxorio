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
	public int size;
	public string color;
}

[Serializable]
public struct ExtraMod
{
	public string name;
	public string description;
	public string author;
	public ModBlock[] modBlocks;
}

[Serializable]
public struct ModBlock
{
	public CONSTANTS.ModBlockType modBlockType;
	public string name;
	public string description;
	public CONSTANTS.Tile allowedTile;
	public CONSTANTS.Category category;
	public string type;
	public int health;
	public TurretStruct turret;
	public BlockStats BlockStats;
	public CONSTANTS.openScripts openScriptType;
	public CONSTANTS.IconStat iconStat;
	public int count;
	public string blockType;
}

[Serializable]
public struct EnemyStruct
{
	public GameObject Enemy;
	public Vector2 Position;
	public float Speed;
}

[Serializable]
public struct OneBlock
{
	public bool Occupied;
	public GameObject Block;
	public string Type;
}

[Serializable]
public struct TurretStruct
{
	public float maxRange;
	public float bulletTime;
	public float speed;
	public float damage;
	public ShootType shootType;
	public float maxSpread;
	public int bulletsInShoot;
	public float spread;
	public enum ShootType
	{
		Basic,
		Blast,
		MiniGun
	}
}

[Serializable]
public class inventoryItem
{
	public GameObject prefab;
	public CONSTANTS.Tile allowTile;
	public string type;
	public CONSTANTS.Category category;
	public string description;
	public string name;
	public CONSTANTS.openScripts openScriptType;
	public int count;
	public string blockType;
}

public class CONSTANTS : MonoBehaviour
{

	public static string savePath = "Mods";

	public static Tile[,] tileMap;

	public static int newId;

	public static Dictionary<GameObject, int> _electroTowers = new Dictionary<GameObject, int>();

	private static GameObject Square;

	private static GameObject Shape;

	[SerializeField] private GameObject Square2;
	[SerializeField] private GameObject Shape2;
	public static Vector2Int mapSize = new Vector2Int(300, 300);
	public static Dictionary<string, int> typeCount = new Dictionary<string, int>();
	public static List<string> BlockTypes = new List<string>
	{
		"None",
		"White Turret",
		"Old Turret",
		"Gold Miner",
		"Gold WareHouse",
		"Stone Wall",
		"Iron Wall",
		"Electro Tower",
		"Water Turret",
		"Uchiha Turret"
	};

	public static BlockStats[] blocks = new BlockStats[]
	{
		new BlockStats { gold = 0, maxGold = 0, amethyst = 0, maxAmethyst = 0 }, //None
		new BlockStats { gold = 65, maxGold = 0, amethyst = 0, maxAmethyst = 0, size = 7 }, //White Turret
		new BlockStats { gold = 110, maxGold = 0, amethyst = 0, maxAmethyst = 0, size = 14 }, //Old Turret
		new BlockStats { gold = 65, maxGold = 250, amethyst = 0, maxAmethyst = 0 }, //Gold Miner
		new BlockStats { gold = 35, maxGold = 1500, amethyst = 0, maxAmethyst = 0 }, //Gold WareHouse
		new BlockStats { gold = 5, maxGold = 0, amethyst = 0, maxAmethyst = 0 }, //Stone Wall
		new BlockStats { gold = 15, maxGold = 0, amethyst = 0, maxAmethyst = 0 }, //Iron Wall
		new BlockStats { gold = 175, maxGold = 0, amethyst = 0, maxAmethyst = 0, size = 5, claimZone = 5, color = "97dde2" }, //Electro Tower
		new BlockStats { gold = 320, maxGold = 0, amethyst = 0, maxAmethyst = 0, size = 5 }, //Water Turret
		new BlockStats { gold = 800, maxGold = 0, amethyst = 0, maxAmethyst = 0, size = 14 } //Uchiha Turret
	};

	public static IconStat[] icons = new IconStat[]
	{
		IconStat.None, //None
		IconStat.Shape, //White Turret
		IconStat.Shape, //Old Turret
		IconStat.None, //Gold Miner
		IconStat.None, //Gold WareHouse
		IconStat.None, //Stone Wall
		IconStat.None, //Iron Wall
		IconStat.Square, //Electro Tower
		IconStat.Shape, //Water Turret
		IconStat.Shape, //Uchiha Turret
		IconStat.Square, //
		IconStat.Shape //
	};

	public enum ModBlockType
	{
		ModTurret,
		ModBlock
	}

	public enum openScripts
	{
		None,
		PlaceBlockScript
	}

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
	
	public void Awake()
	{
		Square = Square2;
		Shape = Shape2;
	}

	public static BlockStats GetBS(string type)
	{
		Debug.Log(BlockTypes);
		Debug.Log(BlockTypes.IndexOf(type));
		Debug.Log(type);
		
		return BlockTypes.IndexOf(type) == -1 ? blocks[0] : blocks[BlockTypes.IndexOf(type)];
	}

	public static GameObject GetIcon(string type)
	{
		GameObject result = null;
		if (icons.Length >= BlockTypes.IndexOf(type)) return result;
		if (icons[BlockTypes.IndexOf(type)] == IconStat.Shape) result = Shape;
		if (icons[BlockTypes.IndexOf(type)] == IconStat.Square) result = Square;
		return result;
	}

	public static int getId(GameObject block)
	{
		if (block != null && _electroTowers.ContainsKey(block))
		{
			return _electroTowers[block];
		}
		newId++;
		return newId;
	}

	public static int getTCount(string type)
	{
		if (!typeCount.ContainsKey(type))
		{
			typeCount.Add(type, 0);
			return 0;
		}
		return typeCount[type];
	}

	public static void addTCount(string type, int count)
	{
		if (!typeCount.ContainsKey(type))
		{
			typeCount.Add(type, count);
			return;
		}
		Dictionary<string, int> dictionary = typeCount;
		dictionary[type] += count;
	}

	public static Vector2Int getMapSize()
	{
		return new Vector2Int(mapSize.x, mapSize.y);
	}

	public static inventoryItem getItemByType(string type)
	{
		foreach (inventoryItem inventoryItem in InventoryManager.instance.items)
		{
			if (inventoryItem.type == type)
			{
				return inventoryItem;
			}
		}
		return null;
	}
}
