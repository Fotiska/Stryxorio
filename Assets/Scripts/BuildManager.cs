using System;
using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BuildManager : MonoBehaviour
{
    private Vector2Int mapSize;
    [SerializeField] public GameObject prefab;
    [SerializeField] private Transform trBlocks;
    [SerializeField] private CameraManager cameraMan;
    private static OneBlock[,] _map;
    private Camera _camera;
    [SerializeField] public CONSTANTS.Tile allowTile;
    [SerializeField] public String type;
    public GameObject texture;
    [SerializeField] private GameObject showing;
    [SerializeField] private Transform effects;
    [SerializeField] private GameObject breakEffect;
    private GameManage gameManage;
    public int size;

    public static OneBlock[,] getMap()
    {
        return _map;
    }
    
    private void Awake()
    {
        mapSize = CONSTANTS.getMapSize();
        _map = new OneBlock[mapSize.x, mapSize.y];
        gameManage = FindObjectOfType<GameManage>();
    }

    private void Start()
    {
        _camera = Camera.main;
        
        int x2 = mapSize.x / 2;
        int x3 = x2 - 1;
        int y2 = mapSize.y / 2;
        int y3 = y2 - 1;
        
        _map[x3, y3].Occupied = true; //Set Occupied In Base
        _map[x3, y2].Occupied = true; //Set Occupied In Base
        _map[x2, y3].Occupied = true; //Set Occupied In Base
        _map[x2, y2].Occupied = true; //Set Occupied In Base

        StartCoroutine(showBlock());
        CONSTANTS.newId = 0;
        Claiming.getInstance().claimZone(new Vector2Int(x2, y2), 20, true, CONSTANTS.getId(null));
    }

    private IEnumerator showBlock()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            
            foreach (Transform sh in showing.transform)
            {
                Destroy(sh.gameObject);
            }
            
            if (texture != null)
            {
                var buildPos = _camera.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
                buildPos = new Vector3(Mathf.Round(buildPos.x - 0.5f) + 0.5f,
                    Mathf.Round(buildPos.y - 0.5f) + 0.5f); //Round Mouse Position
                buildPos = new Vector3(Mathf.Clamp(buildPos.x, 0.5f, 299.5f), Mathf.Clamp(buildPos.y, 0.5f, 299.5f));
                
                showing.transform.position = new Vector3(buildPos.x, buildPos.y, 2);
            
                GameObject blockInst = Instantiate(texture, showing.transform);
                blockInst.SetActive(true);
                foreach (var sprite in blockInst.GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.color = new Color(255, 255, 255, 0.3f);
                }

                GameObject iconPrefab = CONSTANTS.GetIcon(type);

                if (iconPrefab != null)
                {
                    foreach (var sprite in iconPrefab.GetComponentsInChildren<SpriteRenderer>())
                    {
                        sprite.color = new Color(255, 255, 255, 0.3f);
                    }
                
                    GameObject iconInst = Instantiate(iconPrefab, showing.transform);
                    int s = 1 + size * 2;
                    iconInst.transform.localScale = new Vector3(s, s, s);
                }
            }
        }
    }

    private bool testClaim(Vector2Int mapPos, int claimSize)
    {
        if (Claiming.getInstance().claimMap[mapPos.x, mapPos.y]) return true;

        if (claimSize == 0) return false;
        
        for (int x = -claimSize - 1; x <= claimSize + 1; x++)
        {
            for (int y = -claimSize - 1; y <= claimSize + 1; y++)
            {
                int x2 = Mathf.Clamp(mapPos.x + x, 0,  mapSize.x - 1);
                int y2 = Mathf.Clamp(mapPos.y + y, 0, mapSize.y - 1);

                if (Claiming.getInstance().claimMap[x2, y2]) return true;
            }
        }

        return false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            prefab = null;
            texture = null;
            type = "None";
            allowTile = CONSTANTS.Tile.All;
        }
        
        var buildPos = _camera.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
        buildPos = new Vector3(Mathf.Round(buildPos.x - 0.5f) + 0.5f,
            Mathf.Round(buildPos.y - 0.5f) + 0.5f); //Round Mouse Position
        buildPos = new Vector3(Mathf.Clamp(buildPos.x, 0.5f, mapSize.x - 0.5f), Mathf.Clamp(buildPos.y, 0.5f, mapSize.y - 0.5f));

        var mapPos =
            new Vector2Int(
                Mathf.RoundToInt(buildPos.x - 0.5f),
                Mathf.RoundToInt(buildPos.y - 0.5f)); //Get Map Position
        OneBlock block = _map[mapPos.x, mapPos.y]; //Get Block
        if (!cameraMan.inventoryOpened)
        {
            if (Input.GetMouseButton(0) && testClaim(mapPos, CONSTANTS.GetBS(type).claimZone)) //Place Block
            {
                if (prefab == null || block.Occupied) return; //If block occupied return
                if (CONSTANTS.tileMap[mapPos.x, mapPos.y] != allowTile && allowTile != CONSTANTS.Tile.All) 
                {
                                        
                    Debug.Log("{GameLog} => [BuildManager] <color=red>Tile not allowed</color>");
                    
                    return; //If tile not allowed for build block return
                }

                block.Type = type;
                BlockStats blockStats = CONSTANTS.GetBS(block.Type);
                if (gameManage.gold < blockStats.gold || gameManage.amethyst < blockStats.amethyst)
                {
                    block.Type = "None";
                    
                    Debug.Log("{GameLog} => [BuildManager] <color=red>Not enough gold or amethyst</color>");

                    return; //If not enough gold or amethyst return
                }

                gameManage.gold -= blockStats.gold; //Remove gold
                gameManage.maxGold += blockStats.maxGold; //Add max gold
                
                gameManage.amethyst -= blockStats.amethyst; //Remove amethyst
                gameManage.maxAmethyst += blockStats.maxAmethyst; //Add max amethyst

                GameObject inst = Instantiate(prefab, trBlocks); //Spawn block
                block.Occupied = true;
                inst.transform.localPosition = buildPos;
                block.Block = inst;
                block.Type = type;
                gameManage.blockCount += 1;
                CONSTANTS.addTCount(type, 1);
                
                if (blockStats.claimZone != 0)
                {
                    Claiming.getInstance().claimZone(mapPos, blockStats.claimZone, true, CONSTANTS.getId(block.Block));
                    CONSTANTS._electroTowers.Add(block.Block, CONSTANTS.newId);
                }
                
            }
            if (Input.GetMouseButton(1) && block.Block) //Remove Block
            {
                BlockStats blockStats = CONSTANTS.GetBS(block.Type);

                gameManage.maxGold -= blockStats.maxGold;
                gameManage.gold += Mathf.RoundToInt(blockStats.gold * 0.4f);
                
                gameManage.maxAmethyst -= blockStats.maxAmethyst;
                gameManage.amethyst += Mathf.RoundToInt(blockStats.amethyst * 0.4f);

                if (blockStats.claimZone != 0)
                {
                    Claiming.getInstance().claimZone(mapPos, blockStats.claimZone, false, CONSTANTS.getId(block.Block));
                    CONSTANTS._electroTowers.Remove(block.Block);
                }
                
                block.Occupied = false;
                Destroy(block.Block);
                gameManage.blockCount -= 1;
                CONSTANTS.addTCount(type, -1);

                if (blockStats.color != null)
                {
                    GameObject inst = Instantiate(breakEffect, effects);
                    ColorUtility.TryParseHtmlString(blockStats.color, out Color color);

                    inst.transform.position = new Vector3(mapPos.x + 0.5f, mapPos.y + 0.5f, 2);
                    var mainModule = inst.GetComponent<ParticleSystem>().main;
                    mainModule.startColor = color;
                    
                    Destroy(inst, 3f);
                }
                
            }

            _map[mapPos.x, mapPos.y] = block;
            gameManage.block = block;
        }
    }
}
