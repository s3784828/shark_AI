using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public LineRenderer lr;
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

    public Vector2 GetObjectPosInVisionCone()
    {
        Vector2 objectPos = Vector2.zero;
        List<Collider2D> inCircle = new List<Collider2D>();

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, locateRadius))
        {
            if (collider.CompareTag("Player"))
            {
                float angle = Vector2.Angle((Vector2)(transform.position + transform.up), (Vector2) (transform.position + collider.transform.position));


                if (angle < visionConeWidth)
                {
                    lr.SetColors(Color.green, Color.green);
                    lr.SetPosition(0, transform.position);
                    lr.SetPosition(1, collider.transform.position);
                    objectPos = (Vector2)collider.transform.position;
                }
                else
                {
                    lr.SetColors(Color.red, Color.red);
                    lr.SetPosition(0, transform.position);
                    lr.SetPosition(1, collider.transform.position);
                }
            }
            else
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.up * 0.25f);
            }
        }
        return objectPos;
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
