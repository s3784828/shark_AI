using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    public LineRenderer lr;
    public Rigidbody2D rb;
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
        //movement = Seek(target);
        ///movement += Avoid();
        rb.velocity += movement;
        transform.up = rb.velocity;
    }

    public void Seek(Vector2 target)
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
        if (!CanAvoid())
        {
            rb.velocity += steer * Time.fixedDeltaTime;
        }
        else
        {
            Avoid();
        }
        
        transform.up = rb.velocity;
    }

    public void Avoid()
    {
       
        Vector2 avoidanceVector = Vector2.zero;
        Vector2 currentVelocityNormalised = rb.velocity.normalized;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.up, avoidanceDistance);
        if (hit && hit.collider.GetHashCode() != playerColliderHashCode)
        {
            avoidanceVector = (Vector2)hit.point - (Vector2)hit.collider.transform.position;
            avoidanceVector = avoidanceVector.normalized * maxAvoidForce;
        }

        rb.velocity += avoidanceVector * Time.fixedDeltaTime;
    }

    public bool CanAvoid()
    {
        bool canAvoid = false;
        Vector2 currentVelocityNormalised = rb.velocity.normalized;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.up, avoidanceDistance);
        canAvoid = (hit != null && hit.collider.GetHashCode() != playerColliderHashCode) ? true : false;
        return canAvoid;
    }


}
