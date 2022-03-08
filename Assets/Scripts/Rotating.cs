using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class Rotating : MonoBehaviour
{
    [SerializeField] public float speed = 0; // Speed
    [SerializeField] public Axis currentAxis = Axis.X; // Axis
    [SerializeField] public bool invert = false; //Invert Axis
    
    public enum Axis
    {
        X,
        Y,
        Z
    }
    
    void Update()
    {
        float x = 0f, y = 0f, z = 0f;

        switch (currentAxis)
        {
            case Axis.X:
                x = speed;
                break;
            
            case Axis.Y:
                y = speed;
                break;
            
            case Axis.Z:
                z = speed;
                break;
        }

        int invertion = invert ? -1 : 1;
        transform.Rotate(x * invertion, y * invertion, z * invertion);
    }
}