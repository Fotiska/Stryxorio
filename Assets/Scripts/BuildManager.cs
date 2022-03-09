using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public struct OneBlock
{

    public enum BlockType
    {
        None,
        WhiteTurret,
        OldTurret,
        GoldMiner,
        GoldWareHouse,
        StoneWall,
        IronWall,
        ElectroTower
    }

    public bool Occupied;
    public GameObject Block;
    public BlockType Type;
}

public class BuildManager : MonoBehaviour
{
    private Vector2Int mapSize;
    [SerializeField] public GameObject prefab;
    [SerializeField] private Transform trBlocks;
    [SerializeField] private CameraManager cameraMan;
    private static OneBlock[,] _map;
    private Camera _camera;
    [SerializeField] public GameManage.Tile allowTile;
    [SerializeField] public OneBlock.BlockType type;
    private GameManage _gameManage;
    public GameObject texture;
    [SerializeField] private GameObject showing;
    public int size;
    private int newId;
    private Dictionary<GameObject, int> electroTowers= new Dictionary<GameObject, int>();

    public static OneBlock[,] getMap()
    {
        return _map;
    }
    
    private void Awake()
    {
        mapSize = GameManage.getMapSize();
        _map = new OneBlock[GameManage.getMapSize().x, GameManage.getMapSize().y];
        _gameManage = FindObjectOfType<GameManage>(); //Get a Script GameManage
    }

    private int getId(GameObject block)
    {
        if (block != null && electroTowers.ContainsKey(block))
        {
            return electroTowers[block];
        }

        newId += 1;
        
        return newId;
    }
    
    private void Start()
    {
        _camera = Camera.main;
        _map[mapSize.x / 2 - 1, mapSize.y / 2 - 1].Occupied = true; //Set Occupied In Base
        _map[mapSize.x / 2 - 1, mapSize.y / 2].Occupied = true; //Set Occupied In Base
        _map[mapSize.x / 2, mapSize.y / 2 - 1].Occupied = true; //Set Occupied In Base
        _map[mapSize.x / 2, mapSize.y / 2].Occupied = true; //Set Occupied In Base

        StartCoroutine(showBlock());
        newId = 0;
        Claiming.getInstance().claimZone(new Vector2Int(mapSize.x / 2, mapSize.y / 2), 20, true, getId(null));
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
                foreach (var sprite in blockInst.GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.color = new Color(255, 255, 255, 0.3f);
                }

                GameObject iconPrefab = _gameManage.GetIcon(type);

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
        
        for (int x = -claimSize - 1; x < claimSize + 1; x++)
        {
            for (int y = -claimSize - 1; y < claimSize + 1; y++)
            {
                int x2 = Mathf.Clamp(mapPos.x + x, 0,  mapSize.x);
                int y2 = Mathf.Clamp(mapPos.y + y, 0, mapSize.y);
                
                print(x2 + "/" + y2 + "//" + Claiming.getInstance().claimMap[x2, y2]);
                
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
            type = OneBlock.BlockType.None;
            allowTile = GameManage.Tile.All;
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
            if (Input.GetMouseButton(0) && testClaim(mapPos, _gameManage.GetBlockStats(type).claimZone)) //Place Block
            {
                if (prefab == null || block.Occupied) return; //If block occupied return
                if (_gameManage.tileMap[mapPos.x, mapPos.y] != allowTile && allowTile != GameManage.Tile.All)
                    return; //If tile not allowed for build block return

                block.Type = type;
                BlockStats blockStats = _gameManage.GetBlockStats(block.Type);
                if (_gameManage.gold < blockStats.gold || _gameManage.amethyst < blockStats.amethyst)
                {
                    block.Type = OneBlock.BlockType.None;
                    return; //If not enough gold or amethyst return
                }

                _gameManage.gold -= blockStats.gold; //Remove gold
                _gameManage.maxGold += blockStats.maxGold; //Add max gold
                
                _gameManage.amethyst -= blockStats.amethyst; //Remove amethyst
                _gameManage.maxAmethyst += blockStats.maxAmethyst; //Add max amethyst

                GameObject inst = Instantiate(prefab, trBlocks); //Spawn block
                block.Occupied = true;
                inst.transform.localPosition = buildPos;
                block.Block = inst;
                block.Type = type;
                _gameManage.blockCount += 1;
                
                if (blockStats.claimZone != 0)
                {
                    Claiming.getInstance().claimZone(mapPos, blockStats.claimZone, true, getId(block.Block));
                    electroTowers.Add(block.Block, newId);
                }
                
            }
            if (Input.GetMouseButton(1) && block.Block) //Remove Block
            {
                BlockStats blockStats = _gameManage.GetBlockStats(block.Type);

                _gameManage.maxGold -= blockStats.maxGold;
                _gameManage.gold += Mathf.RoundToInt(blockStats.gold * 0.4f);
                
                _gameManage.maxAmethyst -= blockStats.maxAmethyst;
                _gameManage.amethyst += Mathf.RoundToInt(blockStats.amethyst * 0.4f);

                if (blockStats.claimZone != 0)
                {
                    Claiming.getInstance().claimZone(mapPos, blockStats.claimZone, false, getId(block.Block));
                    electroTowers.Remove(block.Block);
                }
                
                block.Occupied = false;
                Destroy(block.Block);
                _gameManage.blockCount -= 1;
            }

            _map[mapPos.x, mapPos.y] = block;
            _gameManage.block = block;
        }
    }
}
