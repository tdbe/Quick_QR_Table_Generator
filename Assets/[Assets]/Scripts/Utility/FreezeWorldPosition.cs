using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeWorldPosition : MonoBehaviour
{
    public Vector3 freezeZeroedAxies = new Vector3(1, 1, 0);
    Vector3 startWorldPos;


    // Start is called before the first frame update
    void Start()
    {
        startWorldPos = transform.position;  
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position != startWorldPos)
        {
            Vector3 newWPos = transform.position;
            newWPos.x = freezeZeroedAxies.x > 0 ? newWPos.x : startWorldPos.x;
            newWPos.y = freezeZeroedAxies.y > 0 ? newWPos.y : startWorldPos.y;
            newWPos.z = freezeZeroedAxies.z > 0 ? newWPos.z : startWorldPos.z;
            transform.position = newWPos;
        }
    }
}
