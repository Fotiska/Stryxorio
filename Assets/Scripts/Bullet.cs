using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect;
    
    private void OnDestroy()
    {
        Destroy(Instantiate(destroyEffect, GameObject.FindWithTag("Effects").transform), 5f);
    }
}
