using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class inventoryItem
{
    public GameObject prefab;
    public GameManage.Tile allowTile;
    public OneBlock.BlockType type;
    public GameManage.Category category;
    public String description;
    public String name;
    public openScripts openScriptType;

    public int count;
    public OneBlock.BlockType blockType;
}

[Serializable]
public enum openScripts{
    None,
    PlaceBlockScript
}

public class InventoryManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform[] categories;
    public List<inventoryItem> items;
    public GameObject infoImage;
    public Text infoDesc;
    public Text gold, amethyst;
    
    public void Awake()
    {
        foreach (var item in items)
        {
            Transform _transform = item.category switch
            {
                GameManage.Category.Defenses => categories[0],
                GameManage.Category.Buildings => categories[1],
                GameManage.Category.Other => categories[2],
                _ => null
            };
            
            GameObject itemInst = Instantiate(itemPrefab, _transform);
            Transform opened = itemInst.transform.Find("Opened");
            Transform imageInst = opened.Find("Image");
            
            GameObject showingPrefab = getShowingPrefab(Instantiate(item.prefab, imageInst));
            GameObject texturePrefab = getTexturePrefab(showingPrefab);
            
            opened.Find("Name").Find("NameText").GetComponent<Text>().text = item.name;
            opened.GetComponentInChildren<Button>().onClick.AddListener( delegate { click(item); });

            if (item.openScriptType != openScripts.None)
            {
                switch (item.openScriptType)
                {
                    case openScripts.PlaceBlockScript:
                    {
                        PlaceBlockScript script = itemInst.AddComponent<PlaceBlockScript>();
                        
                        script.count = item.count;
                        script.type = item.blockType;
                        
                        Debug.Log(script.count + "/" + script.type);
                        break;
                    }
                }
            }
        }
        gameObject.SetActive(false);
    }

    private GameObject getShowingPrefab(GameObject gameObj)
    {
        foreach (var stats in gameObj.GetComponentsInChildren<Stats>())
        {
            DestroyImmediate(stats, true);
        }
        
        foreach (var collider in gameObj.GetComponentsInChildren<Collider2D>())
        {
            DestroyImmediate(collider, true);
        }
        
        foreach (var turret in gameObj.GetComponentsInChildren<Turret>())
        {
            DestroyImmediate(turret, true);
        }
        
        return gameObj;
    }
    
    private GameObject getTexturePrefab(GameObject gameObj)
    {

        List<SpriteRenderer> spriteRenderers = gameObj.GetComponentsInChildren<SpriteRenderer>().ToList();
        
        gameObj.AddComponent(typeof(RectTransform));
        
        foreach (Transform tr in gameObj.transform)
        {
            RectTransform rectTr = tr.gameObject.AddComponent(typeof(RectTransform)).GetComponent<RectTransform>();
            Vector2 sizeDelta = rectTr.sizeDelta;

            sizeDelta.x *= 35;
            sizeDelta.y *= 35;
            
            rectTr.sizeDelta = sizeDelta;
        }
        
        foreach (var sprRenderer in spriteRenderers)
        {
            GameObject obj = sprRenderer.gameObject;
            Image img = obj.gameObject.AddComponent<Image>();
            
            img.sprite = sprRenderer.sprite;

            DestroyImmediate(sprRenderer, true);
        }

        gameObj.layer = 0;
        
        return gameObj;
    }
    
    private void click(inventoryItem item)
    {
        Debug.Log("Selected: " + item.prefab.name);
        BuildManager buildMan = Camera.main.GetComponent<BuildManager>();
        buildMan.prefab = item.prefab;
        buildMan.allowTile = item.allowTile;
        buildMan.type = item.type;
        
        Destroy(buildMan.texture);
        
        foreach (Transform o in infoImage.transform)
        {
            Destroy(o.gameObject);
        }

        BlockStats bs = GameManage.GetBlockStats(item.type);
        int goldC = 0, amethystC = 0;

        goldC = bs.gold;
        amethystC = bs.amethyst;

        GameObject inst = Instantiate(item.prefab, infoImage.transform);
        
        GameObject showingPrefab = getShowingPrefab(inst);
        GameObject showingCloned = Instantiate(showingPrefab);
        showingCloned.SetActive(false);
        GameObject texturePrefab = getTexturePrefab(showingPrefab);
        
        infoDesc.text = item.description;
        gold.text = goldC.ToString();
        amethyst.text = amethystC.ToString();
        buildMan.texture = showingCloned;
        buildMan.size = GameManage.GetBlockStats(item.type).size;
    }
}
