using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    public LineRenderer lr;
    public Rigidbody2D rb;
    public bool avoiding = false;
    public float maxAvoidForce;
    public float avoidanceDistance;
    public float maxSpeed;
    public float maxForce;
    public float minForce;
    public Collider2D collider;

    private Vector2 movement;
    private int playerColliderHashCode;

    public void Start()
    {
        playerColliderHashCode = collider.GetHashCode();
    }

    public void Move(Vector2 target)
    {
        Vector2 avoidanceVector = Avoid();
        
        if (avoidanceVector != Vector2.zero && !avoiding)
        {
            rb.velocity = -rb.velocity;
            //GetComponent<Pathfollowing>().GenerateHeadToPath((Vector2) transform.position + rb.velocity.normalized * avoidanceDistance);
            StartCoroutine(Avoid(0.05f, 1.25f));
            //Seek(avoidanceVector, maxAvoidForce);
            
        }
        else
            Seek(target, maxForce);
    }

    public void Seek(Vector2 target, float maxForce)
    {
        Vector2 desired = target - (Vector2)transform.position;
        desired = desired.normalized;
        desired = desired * maxSpeed;
        Vector2 steer = desired - rb.velocity;
        VectorUtility.Limit(ref steer, minForce, maxForce);

        lr.SetPosition(0, transform.position);
        Vector3 secondPosition = rb.velocity.normalized;
        lr.SetPosition(1, transform.position + secondPosition * avoidanceDistance);
        lr.SetColors(Color.blue, Color.blue);

        Debug.DrawRay(transform.position, transform.up * avoidanceDistance, Color.green);

        rb.velocity += steer * Time.fixedDeltaTime;

        //VectorUtility.Limit(ref rb.velocity, minForce, maxForce);

        //if (!CanAvoid() || avoiding)
        //{
        //    rb.velocity = steer * Time.fixedDeltaTime;
        //}
        //else if (!avoiding)
        //{
        //    StartCoroutine(avoid(1.25f));
        //}

        transform.up = rb.velocity;
    }

    //public void Avoid()
    //{
       
    //    Vector2 avoidanceVector = Vector2.zero;
    //    Vector2 currentVelocityNormalised = rb.velocity.normalized;
    //    //RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.up, avoidanceDistance);

    //    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up * avoidanceDistance);
    //    //RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(collider.bounds.size.x, collider.bounds.size.y),
    //    //    0f, transform.up, avoidanceDistance);
    //    bool hitDetected = false;
    //    foreach (RaycastHit2D hit in hits)
    //    {
    //        if (!hitDetected && !hit.collider.CompareTag("Shark") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Torpedo"))
    //        {
    //            //avoidanceVector = (Vector2)hit.point - (Vector2)hit.collider.transform.position;
    //            //avoidanceVector = avoidanceVector.normalized * maxAvoidForce;
    //            //VectorUtility.Limit(ref avoidanceVector, minForce, maxForce);

    //            gameObject.GetComponent<Pathfollowing>().GenerateCirclePath(hit.point);
                
    //            hitDetected = true;
    //        }
    //    }
    //    avoiding = true;
    //    StartCoroutine(avoid(1.85f));
    //    //rb.velocity += avoidanceVector * Time.fixedDeltaTime;
    //}

    public Vector2 Avoid()
    {
        Vector2 avoidVector = Vector2.zero;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, avoidanceDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                avoidVector = hit.normal * avoidanceDistance;
            }
        }

        return avoidVector;
    }

    public bool CanAvoid()
    {
        bool canAvoid = false;
        //RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(collider.bounds.size.x, collider.bounds.size.y),
        //    0f, transform.up, avoidanceDistance);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up * avoidanceDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (!canAvoid && !hit.collider.CompareTag("Shark") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Torpedo"))
            {
                
                canAvoid = true;
            }
        }
        return canAvoid;
    }

    

    private IEnumerator Avoid(float timeTillGenNewPath, float avoidTime)
    {
        avoiding = true;
        yield return new WaitForSeconds(timeTillGenNewPath);
        GetComponent<Pathfollowing>().GenerateCirclePath(transform.position);
        yield return new WaitForSeconds(avoidTime);
        avoiding = false;
    }

}
