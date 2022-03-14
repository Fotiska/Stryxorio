using System;
using System.Collections.Generic;
using UnityEngine;

public class Claiming : MonoBehaviour
{
    private static Claiming instance;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool[,] claimMap;
    public Vector2Int mapSize;
    public Texture2D texture;
    public Dictionary<Vector2Int, List<int>> mapClaimed = new Dictionary<Vector2Int, List<int>>();

    private void Awake()
    {
        instance = this;
        mapSize = GameManage.getMapSize();
        claimMap = new bool[mapSize.x, mapSize.y];
        for (int x = 0; x <= mapSize.x; x++)
        {
            for (int y = 0; y <= mapSize.y; y++)
            {
                mapClaimed.Add(new Vector2Int(x, y), new List<int>());
            }
        }
        updateMap();
    }
    
    public static Claiming getInstance()
    {
        return instance;
    }

    public void claimZone(Vector2Int centerPos, int zoneScale, bool claim, int id)
    {
        for (int x = -zoneScale - 1; x < zoneScale; x++)
        {
            for (int y = -zoneScale - 1; y < zoneScale; y++)
            {
                int x2 = Mathf.Clamp(centerPos.x + x + 1, 0, mapSize.x - 1);
                int y2 = Mathf.Clamp(centerPos.y + y + 1, 0, mapSize.y - 1);
                Vector2Int mapPos = new Vector2Int(x2, y2);
                bool containsMap = mapClaimed.ContainsKey(mapPos);
                
                if (claim)
                {
                    if (containsMap)
                    {
                        mapClaimed[mapPos].Add(id);
                        claimMap[x2, y2] = true;
                    }
                }
                else
                {
                    bool containsId = mapClaimed[mapPos].Contains(id);

                    if (containsId && containsMap)
                    {
                        if (mapClaimed[mapPos].Count == 1)
                        {
                            claimMap[x2, y2] = false;
                        }

                        if (!claimMap[x2, y2])
                        {
                            OneBlock block = BuildManager.getMap()[x2, y2];
                            block.Occupied = false;
                            block.Type = OneBlock.BlockType.None;
                            Destroy(block.Block);
                        }

                        mapClaimed[mapPos].Remove(id);

                    }
                    
                }
            }
        }

        updateZone(centerPos, zoneScale);
    }
    
    public void updateZone(Vector2Int centerPos, int zoneScale)
    {
        for (int x = -zoneScale - 1; x <= zoneScale; x++)
        {
            for (int y = -zoneScale - 1; y <= zoneScale; y++)
            {
                int x2 = Mathf.Clamp(centerPos.x + x + 1, 0, mapSize.x - 1);
                int y2 = Mathf.Clamp(centerPos.y + y + 1, 0, mapSize.y - 1);
                
                texture.SetPixel(x2, y2, claimMap[x2, y2] ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 0.6f));
            }
        }
        
        texture.Apply(); //Apply Changes
        
        //Create Sprite
        var spr = Sprite.Create(texture, new Rect(0, 0, mapSize.x, mapSize.y), new Vector2(0, 0));
        spriteRenderer.sprite = spr;
    }
    
    public void updateMap()
    {
        var map = new Texture2D(mapSize.x, mapSize.y); //Create Texture2D
        print(map);
        
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map.SetPixel(x, y, claimMap[x, y] ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 0.6f));
            }
        }
        
        map.filterMode = FilterMode.Point; //Switch FilterMode To Point (For Pixels)
        map.wrapMode = TextureWrapMode.Clamp; //Switch WrapMode To Clamp
        map.Apply(); //Apply Changes

        texture = map;
                
        //Create Sprite
        var spr = Sprite.Create(map, new Rect(0, 0, mapSize.x, mapSize.y), new Vector2(0, 0));
        spriteRenderer.sprite = spr;
    }
    
}
