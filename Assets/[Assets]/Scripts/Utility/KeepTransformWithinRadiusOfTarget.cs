using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepTransformWithinRadiusOfTarget : MonoBehaviour
{
    public Transform target = null;
    public Vector2 maxDistance = new Vector2(250, 200);
    Vector3 oldPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Mathf.Abs(transform.position.x - target.position.x) > maxDistance.x)
        {
            transform.position = new Vector3(oldPosition.x, transform.position.y, transform.position.z);
        }

        if (Mathf.Abs(transform.position.y - target.position.y) > maxDistance.y)
        {
            transform.position = new Vector3(transform.position.x, oldPosition.y, transform.position.z);
        }

        oldPosition = transform.position;
    }
}
