using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect;
    
    private void OnDestroy()
    {
        GameObject inst = Instantiate(destroyEffect, GameObject.FindWithTag("Effects").transform);
        inst.transform.position = transform.position;
        Destroy(inst, 5f);
    }
}
