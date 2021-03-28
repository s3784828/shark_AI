using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    public float locateRadius;
    public float visionConeWidth;

    private List<Transform> watching;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckRadius()
    {
        List<Collider2D> inCircle = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, locateRadius);

        foreach (Collider2D collider in inCircle)
        {
            if (collider.transform.CompareTag("Player"))
            {
                Debug.Log("did it");
            }
        }
    }

    bool InSight(Transform lookAt)
    {
        bool inSight = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position + lookAt.position, locateRadius);
        if (hit.transform.CompareTag("Player") && Vector2.Angle(transform.up, transform.position + lookAt.position) <= visionConeWidth)
        {
            inSight = true;
        }
        return inSight;
    }
}
