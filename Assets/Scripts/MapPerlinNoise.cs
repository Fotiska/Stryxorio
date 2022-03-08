using UnityEngine;
using Random = UnityEngine.Random;

public class MapPerlinNoise : MonoBehaviour
{
    private Vector2Int mapSize;
    [SerializeField] private Texture2D[] tileTextures;
    [SerializeField] private float zoom;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private float balance;
    [SerializeField] private Vector2Int tileSize;
    [SerializeField] private string seed;
    private GameManage.Tile[,] _tileMap;
    private GameManage _gameManage;

    private void Awake()
    {
        mapSize = GameManage.getMapSize();
        _tileMap = new GameManage.Tile[mapSize.x, mapSize.y];
        _gameManage = FindObjectOfType<GameManage>();
        GenerateMap();
    }

    private void GenerateMap()
    {
        int seed2 = PlayerPrefs.GetInt("Seed");
        float balance2 = PlayerPrefs.GetFloat("Balance");

        balance = balance2;
        Random.InitState(seed2); //Set Seed
        
        var rX = Random.Range(-2500, 2500); //Random Offset X
        var rY = Random.Range(-2500, 2500); //Random Offset Y
        
        var map = new Texture2D(mapSize.x * tileSize.x, mapSize.y * tileSize.y); //Create Texture2D

        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                var perlinNoise = Mathf.PerlinNoise(x * zoom + rX, y * zoom + rY); //Get Noise

                perlinNoise = Mathf.Round(perlinNoise * balance); //Round And Multiply With Balance
                perlinNoise = Mathf.Clamp(perlinNoise, 0, 2); //Clamp 0 - 2

                for (var tx = 0; tx < tileSize.x; tx++)
                {
                    for (var ty = 0; ty < tileSize.x; ty++)
                    {
                        var thx = tx + x * tileSize.x; //TilePosition + PixelPosition
                        var thy = ty + y * tileSize.y; //TilePosition + PixelPosition

                        //Save Pixel Color
                        map.SetPixel(thx, thy, tileTextures[Mathf.RoundToInt(perlinNoise)].GetPixel(tx, ty));
                        //Save Tile Type
                        GameManage.Tile tile;
                        switch (Mathf.RoundToInt(perlinNoise))
                        {
                            case 0:
                                tile = GameManage.Tile.Gold;
                                break;

                            case 1:
                                tile = GameManage.Tile.Basic;
                                break;

                            default:
                                tile = GameManage.Tile.Amethyst;
                                break;
                        }

                        _tileMap[x, y] = tile;

                    }
                }
            }
        }

        map.filterMode = FilterMode.Point; //Switch FilterMode To Point(For Pixels)
        map.Apply(); //Apply Changes
        _gameManage.tileMap = _tileMap;
        
        //Create Sprite
        var spr = Sprite.Create(map, new Rect(0, 0, mapSize.x * tileSize.x, mapSize.y * tileSize.y), new Vector2(0, 0));
        renderer.sprite = spr;
    }
    
}
