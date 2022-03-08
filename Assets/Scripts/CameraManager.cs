using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float fastSpeed;
    [SerializeField] private GameObject inventory;
    public bool inventoryOpened;
    private Camera _camera;
    private Camera[] cameraChild;

    private void Awake()
    {
        _camera = Camera.main;
        cameraChild = _camera.GetComponentsInChildren<Camera>();
    }

    void Update()
    {
        float fSpeed = speed;
        Transform tr = transform;
        Vector2 pos = tr.position;

        tr.position = new Vector2(Mathf.Clamp(pos.x, 0, 300), Mathf.Clamp(pos.y, 0, 300));
        
        //Fast
        if (Input.GetKey(KeyCode.LeftShift)) fSpeed = fastSpeed;

        //Movement
        if (Input.GetKey(KeyCode.W)) tr.Translate(Vector3.up * (Time.deltaTime * fSpeed)); // 🡡
        if (Input.GetKey(KeyCode.S)) tr.Translate(Vector3.down * (Time.deltaTime * fSpeed)); // 🡣

        if (Input.GetKey(KeyCode.A)) tr.Translate(Vector3.left * (Time.deltaTime * fSpeed)); // 🡠
        if (Input.GetKey(KeyCode.D)) tr.Translate(Vector3.right * (Time.deltaTime * fSpeed)); // 🡢
        
        //Zoom
        if (!inventoryOpened){
            if (Input.GetKey(KeyCode.Escape)) Application.Quit();
            
            float orthographicSize = _camera.orthographicSize;
            orthographicSize -= Input.mouseScrollDelta.y;
            _camera.orthographicSize = Mathf.Clamp(orthographicSize, 3.5f, 40f);
            foreach (var t in cameraChild)
            {
                t.orthographicSize = _camera.orthographicSize;
            }
        }
        
        //Open Inventory    
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.SetActive(!inventory.activeSelf);
            inventoryOpened = inventory.activeSelf;
        }
    }
}