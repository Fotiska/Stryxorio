using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class ModLoader : MonoBehaviour
{
	public void loadModsInventory(InventoryManager im)
	{
		String pathDir = CONSTANTS.savePath + Path.DirectorySeparatorChar;
		if (!Directory.Exists(pathDir))
		{
			Debug.Log("{GameLog} => [InventoryManager] <color=red>Folder mods not created</color>");
			Directory.CreateDirectory(pathDir);
			return;
		}
		
		string str = "{GameLog} => [InventoryManager] Mods count: ";
		int i = Directory.GetFiles(pathDir).Length;
		Debug.Log(str + i);
		foreach (string path in Directory.GetFiles(pathDir))
		{
			string json;
			try
			{
				json = File.ReadAllText(path);
			}
			catch (Exception)
			{
				Debug.Log("{GameLog} => [ModLoader] <color=red>Loading json file error</color>");
				throw;
			}
			try
			{
				JsonUtility.FromJson<TurretStruct>(json);
				loadModInventory(im, json);
			}
			catch (Exception)
			{
				try
				{
					JsonUtility.FromJson<ExtraMod>(json);
					loadExtraModInventory(im, json);
				}
				catch (Exception)
				{
					Debug.Log("{GameLog} => [ModLoader] <color=red>Loading ExtraMod error</color>");
					throw;
				}
				Debug.Log("{GameLog} => [ModLoader] <color=red>Loading TurretStruct error</color>");
				throw;
			}
		}
	}

	public static void loadModInventory(InventoryManager im, string json)
	{
		inventoryItem inventoryItem = new inventoryItem();
		GameObject gameObject = Instantiate(im.modTurretPrefab, im.prefabs);
		Debug.Log(gameObject.name);
		try
		{
			gameObject.GetComponent<Turret>().turret = JsonUtility.FromJson<TurretStruct>(json);
		}
		catch (Exception)
		{
			Debug.Log("{GameLog} => [InventoryManager] <color=red>Loading mod error</color>");
			return;
		}
		inventoryItem.prefab = gameObject;
		inventoryItem.allowTile = CONSTANTS.Tile.All;
		inventoryItem.type = im.modType;
		inventoryItem.category = im.modCategory;
		inventoryItem.description = im.modDescription;
		inventoryItem.name = im.modName;
		inventoryItem.openScriptType = CONSTANTS.openScripts.None;
		CONSTANTS.BlockTypes.Add(inventoryItem.type);
		CONSTANTS.icons = CONSTANTS.icons.Concat(new [] { CONSTANTS.IconStat.Shape }).ToArray();
		BlockStats blockStats = new BlockStats
		{
			gold = 350,
			maxGold = 0,
			amethyst = 0,
			maxAmethyst = 0,
			size = Mathf.RoundToInt(JsonUtility.FromJson<TurretStruct>(json).maxRange)
		};
		CONSTANTS.blocks = CONSTANTS.blocks.Concat(new BlockStats[] { blockStats }).ToArray();
		im.items.Add(inventoryItem);
	}

	public static void loadExtraModInventory(InventoryManager im, string json)
	{
		foreach (ModBlock modBlock in JsonUtility.FromJson<ExtraMod>(json).modBlocks)
		{
			inventoryItem inventoryItem = new inventoryItem();
			GameObject gameObject = Instantiate(modBlock.modBlockType == CONSTANTS.ModBlockType.ModTurret ? im.modTurretPrefab : im.modBlockPrefab, im.prefabs);
			Debug.Log(gameObject.name);
			gameObject.GetComponent<Turret>().turret = modBlock.turret;
			gameObject.GetComponent<Stats>().health = (float)modBlock.health;
			inventoryItem.prefab = gameObject;
			inventoryItem.allowTile = modBlock.allowedTile;
			inventoryItem.type = modBlock.type;
			inventoryItem.category = modBlock.category;
			inventoryItem.description = modBlock.description;
			inventoryItem.name = modBlock.name;
			inventoryItem.openScriptType = modBlock.openScriptType;
			inventoryItem.count = modBlock.count;
			inventoryItem.blockType = modBlock.blockType;
			CONSTANTS.BlockTypes.Add(inventoryItem.type);
			CONSTANTS.icons = CONSTANTS.icons.Concat(new [] { modBlock.iconStat }).ToArray<CONSTANTS.IconStat>();
			CONSTANTS.blocks = CONSTANTS.blocks.Concat(new [] { modBlock.BlockStats }).ToArray();
			im.items.Add(inventoryItem);
		}
	}
}
