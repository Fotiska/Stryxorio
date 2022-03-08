using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class CreateGameManager : MonoBehaviour
{

    private Vector2Int mapSize;
    [SerializeField] private float zoom;
    [SerializeField] private Image image;
    [SerializeField] private float balance;
    [SerializeField] private Slider slider;
    [SerializeField] private InputField seedField;
    private String seed;

    private void Awake()
    {
        mapSize = GameManage.getMapSize();
    }

    public void resetBalance()
    {
        slider.value = 2.15f;
        balance = 2.15f;
        updateMap();
    }
    
    public void resetSeed()
    {
        seedField.text = Random.seed.ToString();
        updateMap();
    }
    
    public void saveValues()
    {
        Int32.TryParse(seed, out var seed2); //Try Parse String To Int
        if (seed2 == 0) seed2 = Random.Range(0, 999999999); //If Don't Parse Generate Seed
        PlayerPrefs.SetInt("Seed", seed2); //Save Seed
        PlayerPrefs.SetFloat("Balance", balance); //Save Balance
    }

    public void updateMap()
    {
        seed = seedField.text;
        balance = slider.value;

        Int32.TryParse(seed, out var seed2); //Try Parse String To Int
        if (seed2 == 0) seed2 = Random.Range(0, 999999999); //If Don't Parse Generate Seed
        PlayerPrefs.SetInt("Seed", seed2); //Save Seed
        
        Random.InitState(seed2); //Set Seed
        
        var rX = Random.Range(-2500, 2500); //Seed Offset X
        var rY = Random.Range(-2500, 2500); //Seed Offset Y
        
        var map = new Texture2D(mapSize.x, mapSize.y); //Create Texture2D

        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                var perlinNoise = Mathf.PerlinNoise(x * zoom + rX, y * zoom + rY); //Get Noise

                perlinNoise = Mathf.Round(perlinNoise * balance); //Round And Multiply With Balance
                perlinNoise = Mathf.Clamp(perlinNoise, 0, 2); //Clamp 0 - 2
                Color color = new Color(0, 0, 0, 0);
                switch (perlinNoise)
                {
                    case 0f:
                        ColorUtility.TryParseHtmlString("#d69529", out color); //Gold
                        break;
                    case 1f:
                        ColorUtility.TryParseHtmlString("#6a6a6a", out color); //Basic
                        break;
                    case 2f:
                        ColorUtility.TryParseHtmlString("#9e5db3", out color); //Amethyst
                        break;
                }
                
                map.SetPixel(x, y, color);
            }
        }

        map.filterMode = FilterMode.Point; //Switch FilterMode To Point (For Pixels)
        map.wrapMode = TextureWrapMode.Clamp; //Switch WrapMode To Clamp
        map.Apply(); //Apply Changes
        
        //Create Sprite
        var spr = Sprite.Create(map, new Rect(0, 0, mapSize.x, mapSize.y), new Vector2(0, 0));
        image.sprite = spr;
    }
}
