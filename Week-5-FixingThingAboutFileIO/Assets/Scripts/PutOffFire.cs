using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

public class PutOffFire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("Fire put off!");

        // Random position
        transform.position = new Vector3(
            Random.Range(-8.0f, 8.0f), 
            Random.Range(-4.0f, 4.0f));
        
        // Random rotation
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));

        GameManager.instance.Score++;
    }
}
