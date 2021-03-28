using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfollowing : MonoBehaviour
{
    public LineRenderer pathLR;
    public LineRenderer desiredVelocityLR;
    public LineRenderer pathNormalLR;
    public List<Transform> positionList;
    private Path[] path;
    public float predictionDistance;
    public float directionDistance;
    public float startingBestDistance;
    public float radius;
    private int ignorePath;
    public Rigidbody2D rb;
    public SteeringBehaviour sb;

    public float pathRadius;
    public float pathTheta;
    public float positionModifierRange;

    class Path
    {
        public Vector2 start;
        public Vector2 end;

        public Path(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GeneratePath();
    }

    // Update is called once per frame
    void Update()
    {
        //GeneratePath();
        //DrawPath();
        //FollowPath();
    }

    void GeneratePath()
    {
        int numVertices = 360 / (int) pathTheta;
        path = new Path[numVertices + 1];
        pathLR.positionCount = numVertices + 1;
        pathLR.loop = true;

        for (int i = 0; i <= numVertices; i++)
        {
            float theta = (float) i / (float)numVertices * 2.0f * Mathf.PI;
            
            float x = transform.position.x + (pathRadius * Mathf.Cos(theta));
            float xPos = Random.Range(x - positionModifierRange, x + positionModifierRange);
            float y = transform.position.y + (pathRadius * Mathf.Sin(theta));
            float yPos = Random.Range(y - positionModifierRange, y + positionModifierRange);
            Vector3 pos = new Vector3(xPos, yPos, 0f);
            //pathLR.SetPosition(i, pos);

            if (path[i] == null)
            {
                path[i] = new Path(pos, Vector2.zero);
            }
            else
            {
                path[i].start = pos;
            }

            if (i == 0)
            {
                path[numVertices] = new Path(Vector2.zero, (Vector2) pos);
                //pathLR.SetPosition(numVertices, pos);
            }
            else
            {
                path[i - 1].end = pos;
            }
        }

        for (int i = 0; i < path.Length; i++)
        {
            pathLR.SetPosition(i, (Vector3) path[i].start);
            Debug.Log(i + " values: " + path[i].start + "  " + path[i].end);
        }
        
    }

    public void FollowPath()
    {
        /*
         * predictedLocation = is the predicted location the gameobject will reach with
         * its current velocity.
         * pointA = is the vector from the start of the path, to the predicted location.
         * pointB = is the vector from the start of the path, to the end of the path.
         */
        Vector2 predictedLoc = GetPredictedLocation(predictionDistance);

        Vector2 pointA = Vector2.zero;
        Vector2 pointB = Vector2.zero;
        Vector2 normalPoint = Vector2.zero;

        float bestDistance = startingBestDistance;
        Vector2 targetNormalPoint = Vector2.zero;
        for (int i = 0; i < path.Length; i++)
        {
            pointA = path[i].start;
            pointB = path[i].end;
            /*
            * normalPoint = the point where the shark should be in the path, from its predicted location.
            */
            normalPoint = GetNormalPoint(predictedLoc, pointA, pointB);

            if (normalPoint.x < pointA.x)
            {
                normalPoint = pointA;
            }
            else if (normalPoint.x > pointB.x)
            {
                normalPoint = pointB;
            }

            float normalDistance = VectorUtility.distance(predictedLoc, normalPoint);

            if (normalDistance < bestDistance)
            {
                bestDistance = normalDistance;
                targetNormalPoint = normalPoint;
            }
        }

        /*
         * normalPoint = the point where the shark should be in the path, from its predicted location.
         */
        Vector2 direction = pointB - pointA;
        direction = direction.normalized;
        direction *= directionDistance;

        

        Vector2 target = targetNormalPoint + direction;
        float distance = VectorUtility.distance(targetNormalPoint, predictedLoc);

        pathNormalLR.SetPosition(0, predictedLoc);
        pathNormalLR.SetPosition(1, targetNormalPoint);

        desiredVelocityLR.SetPosition(0, predictedLoc);
        desiredVelocityLR.SetPosition(1, transform.position);

        if (distance > radius)
        {
            pathNormalLR.SetColors(Color.red, Color.red);
            sb.Seek(target);
        }
        else
        {
            pathNormalLR.SetColors(Color.green, Color.green);
        }

    }

    Vector2 GetNormalPoint(Vector2 predictedLoc, Vector2 pointA, Vector2 pointB)
    {
        Vector2 ap = predictedLoc - pointA;
        Vector2 ab = pointB - pointA;

        ab = ab.normalized;
        ab = ab * (VectorUtility.DotProduct(ap, ab));
        Vector2 normalPoint = pointA + ab;

        return normalPoint;
    }

    Vector2 GetPredictedLocation(float predictionLength)
    {
        Vector2 predict = rb.velocity;
        predict = predict.normalized;

        /*
         * Have the predicted velocity look ahead by a certain amount
         */
        predict *= predictionLength;

        /*
        * Add predict vector to current location to work out predicted
        * location.
        */
        return (Vector2)transform.position + predict;
    }

    

}
