using System.Collections.Generic;
using UnityEngine;

public class Claiming : MonoBehaviour
{
    private static Claiming instance;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool[,] claimMap;
    public Vector2Int mapSize;
    public Dictionary<Vector2Int, List<int>> mapClaimed = new Dictionary<Vector2Int, List<int>>();
    
    private void Start()
    {
        instance = this;
        mapSize = GameManage.getMapSize();
        claimMap = new bool[mapSize.x, mapSize.y];
    }

    public static Claiming getInstance()
    {
        return instance;
    }

    public void claimZone(Vector2Int centerPos, int zoneScale, bool claim, int id)
    {
        for (int x = -zoneScale; x < zoneScale; x++)
        {
            for (int y = -zoneScale; y < zoneScale; y++)
            {
                int x2 = Mathf.Clamp(centerPos.x + x, 0, mapSize.x);
                int y2 = Mathf.Clamp(centerPos.y + y, 0, mapSize.y);
                
                if (claim)
                {
                    if (!mapClaimed.ContainsKey(new Vector2Int(x2, y2)))
                    {
                        mapClaimed.Add(new Vector2Int(x2, y2), new List<int>(id));
                        claimMap[x2, y2] = true;
                    }

                    if (mapClaimed.ContainsKey(new Vector2Int(x2, y2)))
                    {
                        mapClaimed[new Vector2Int(x2, y2)].Add(id);
                        claimMap[x2, y2] = true;
                    }
                }
                else
                {
                    if (mapClaimed.ContainsKey(new Vector2Int(x2, y2)) && mapClaimed[new Vector2Int(x2, y2)].Count == 1 && mapClaimed[new Vector2Int(x2, y2)].Contains(id))
                    {
                        claimMap[x2, y2] = false;
                    }
                    if (mapClaimed.ContainsKey(new Vector2Int(x2, y2)) && claimMap[x2, y2] == false && mapClaimed[new Vector2Int(x2, y2)].Contains(id))
                    {
                        BuildManager.getMap()[x2, y2].Occupied = false;
                        BuildManager.getMap()[x2, y2].Type = OneBlock.BlockType.None; 
                        Destroy(BuildManager.getMap()[x2, y2].Block);
                    }
                    mapClaimed[new Vector2Int(x2, y2)].Remove(id);
                }
            }
        }

        updateZone();
    }
    
    public void updateZone()
    {
        var map = new Texture2D(mapSize.x, mapSize.y); //Create Texture2D
        
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
        
        //Create Sprite
        var spr = Sprite.Create(map, new Rect(0, 0, mapSize.x, mapSize.y), new Vector2(0, 0));
        spriteRenderer.sprite = spr;
    }
    
}
