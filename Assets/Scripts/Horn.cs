using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horn : MonoBehaviour
{
    void Start()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
                break;
        }
    }

    void Update()
    {
        
    }
}
