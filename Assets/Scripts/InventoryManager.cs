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
    public GameObject texturePrefab;
    public String description; //make
    public String name; //make
    public GameObject showTexture;
    public int size;
}


public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform[] categories;
    public List<inventoryItem> items;
    [SerializeField] private GameObject infoImage;
    [SerializeField] private Text infoDesc;
    [SerializeField] private Text gold, amethyst;
    private GameManage _gameManage;
    
    public void Awake()
    {
        _gameManage = FindObjectOfType<GameManage>(); //Get GameManage
        
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
            Instantiate(item.texturePrefab, imageInst);
            itemInst.transform.Find("Name").Find("NameText").GetComponent<Text>().text = item.name;
            itemInst.GetComponentInChildren<Button>().onClick.AddListener( delegate { click(item); });
        }
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

        BlockStats bs = _gameManage.GetBlockStats(item.type);
        int goldC = 0, amethystC = 0;

        goldC = bs.gold;
        amethystC = bs.amethyst;
        
        Instantiate(item.texturePrefab, infoImage.transform);
        infoDesc.text = item.description;
        gold.text = goldC.ToString();
        amethyst.text = amethystC.ToString();
        buildMan.texture = item.showTexture;
        buildMan.size = item.size;
    }
    
}
