using System;
using System.Collections.Generic;
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
}


public class InventoryManager : MonoBehaviour
{
    public GameObject itemPrefab; //make
    public Transform[] categories; //make
    public List<inventoryItem> items; //dont
    public GameObject infoImage; //make
    public Text infoDesc; //make
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
            Transform imageInst = itemInst.transform.Find("Image");

            GameObject showingPrefab = getShowingPrefab(item.prefab);
            GameObject texturePrefab = getTexturePrefab(showingPrefab);
            
            Instantiate(texturePrefab, imageInst);
            itemInst.transform.Find("Name").Find("NameText").GetComponent<Text>().text = item.name;
            itemInst.GetComponentInChildren<Button>().onClick.AddListener( delegate { click(item); });
        }
    }

    private GameObject getShowingPrefab(GameObject gameObject)
    {
        foreach (var stats in gameObject.GetComponentsInChildren<Stats>())
        {
            Destroy(stats);
        }
        
        foreach (var collider in gameObject.GetComponentsInChildren<Collider2D>())
        {
            Destroy(collider);
        }
        
        foreach (var turret in gameObject.GetComponentsInChildren<Turret>())
        {
            Destroy(turret);
        }
        
        return gameObject;
    }
    
    private GameObject getTexturePrefab(GameObject gameObject)
    {
        foreach (var sprRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            GameObject obj = sprRenderer.gameObject;
            Image img = obj.gameObject.AddComponent<Image>();

            img.sprite = sprRenderer.sprite;
            
            Destroy(sprRenderer);
        }
        
        return gameObject;
    }
    
    private void click(inventoryItem item)
    {
        Debug.Log("Selected: " + item.prefab.name);
        BuildManager buildMan = Camera.main.GetComponent<BuildManager>();
        buildMan.prefab = item.prefab;
        buildMan.allowTile = item.allowTile;
        buildMan.type = item.type;
        
        foreach (Transform o in infoImage.transform)
        {
            Destroy(o.gameObject);
        }

        BlockStats bs = GameManage.GetBlockStats(item.type);
        int goldC = 0, amethystC = 0;

        goldC = bs.gold;
        amethystC = bs.amethyst;

        GameObject showingPrefab = getShowingPrefab(item.prefab);
        GameObject texturePrefab = getTexturePrefab(showingPrefab);
        
        
        GameObject inst = Instantiate(texturePrefab, infoImage.transform);
        infoDesc.text = item.description;
        gold.text = goldC.ToString();
        amethyst.text = amethystC.ToString();
        buildMan.texture = showingPrefab;
        buildMan.size = GameManage.GetBlockStats(item.type).claimZone;
    }
    
}
