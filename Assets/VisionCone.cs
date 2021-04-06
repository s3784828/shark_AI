using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float locateRadius;
    public Collider2D collider;

    private int playerColliderHashCode;

    // Start is called before the first frame update
    void Start()
    {
        playerColliderHashCode = collider.GetHashCode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetObjectInVisionCone(float forwardDistance)
    {
        GameObject foundObject = null;
        RaycastHit2D hit;

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, locateRadius))
        {
            if (collider.GetHashCode() != playerColliderHashCode && collider.gameObject.CompareTag("Player"))
                foundObject = collider.gameObject;
        }
        return foundObject;
    }

    

}
