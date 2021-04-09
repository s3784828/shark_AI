using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfollowing : MonoBehaviour
{
    public enum PathState
    {
        CIRCLE,
        HEAD_TO
    }

    [Header("Inspector values")]
    public Rigidbody2D rb;
    public SteeringBehaviour sb;
    public LineRenderer pathLR;
    public LineRenderer desiredVelocityLR;
    public LineRenderer pathNormalLR;

    [Header("Path following attributes")]
    public float predictionDistance;
    public float directionDistance;
    public float startingBestDistance;
    public float radius;

    [Header("Patrol path generation")]
    public float pathRadius;
    public float pathTheta;
    public float positionModifierRange;

    [Header("HeadTo path generation")]
    public float yOffset;
    public float chunkSize;

    private Path[] path;
    private int pathSize;
    private PathState pathState;

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

    public void GeneratePath()
    {
        path = new Path[100];
        for (int i = 0; i < 100; i++)
        {
            path[i] = new Path(Vector2.zero, Vector2.zero);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateCirclePath(Vector2 center)
    {
        int numVertices = 360 / (int) pathTheta;
        pathLR.positionCount = numVertices + 1;
        pathLR.loop = true;
        pathSize = numVertices + 1;

        for (int i = 0; i <= numVertices; i++)
        {
            float theta = (float) i / (float)numVertices * 2.0f * Mathf.PI;
            
            float x = center.x + (pathRadius * Mathf.Cos(theta));
            float xPos = Random.Range(x - positionModifierRange, x + positionModifierRange);
            float y = center.y + (pathRadius * Mathf.Sin(theta));
            float yPos = Random.Range(y - positionModifierRange, y + positionModifierRange);
            Vector2 pos = new Vector2(xPos, yPos);
            //pathLR.SetPosition(i, pos);

            //if (path[i] == null)
            //{
            //    path[i] = new Path(pos, Vector2.zero);
            //}
            //else
            //{
            //    path[i].start = pos;
            //}

            //if (i == 0)
            //{
            //    path[numVertices] = new Path(Vector2.zero, (Vector2) pos);
            //    //pathLR.SetPosition(numVertices, pos);
            //}
            //else
            //{
            //    path[i - 1].end = pos;
            //}

            path[i].start = pos;

            if (i == 0)
            {
                path[numVertices].end = pos;
                
            }
            else
            {
                path[i - 1].end = pos;
            }
        }



        for (int i = 0; i <= numVertices; i++)
        {
            
            pathLR.SetPosition(i, (Vector3) path[i].start);
        }

        pathState = PathState.CIRCLE;
    }

    public void GenerateHeadToPath(Vector2 headToPosition)
    {
        
        Vector2 dir = (headToPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance((Vector2)transform.position, headToPosition);
        int numChunks = Mathf.RoundToInt(distance / chunkSize);
        pathSize = numChunks;
        pathLR.positionCount = numChunks;
        pathLR.loop = false;
        
        //path = new Path[numChunks];
        if (numChunks > 0)
        {
            path[0] = new Path((Vector2)transform.position, Vector2.zero);
            for (int i = 0; i < numChunks; i++)
            {
                path[i].end = path[i].start + dir * chunkSize + new Vector2(0f, Random.Range(-yOffset, yOffset));
                

                if (i < numChunks - 1)
                {
                    //path[i + 1] = new Path(path[i].end, Vector2.zero);
                    path[i + 1].start = path[i].end;
                }
            }
            path[numChunks - 1].end = headToPosition;
        }
        else
        {
            //path[0] = new Path((Vector2)transform.position, headToPosition);
            path[0].start = (Vector2)transform.position;
            path[0].end = headToPosition;
        }
        
        for (int i = 0; i < numChunks; i++)
        {
            pathLR.SetPosition(i, path[i].start);
        }

        pathState = PathState.HEAD_TO;
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
        for (int i = 0; i < pathSize; i++)
        {
            pointA = path[i].start;
            pointA = path[i].start;
            pointB = path[i].end;
            /*
            * normalPoint = the point where the shark should be in the path, from its predicted location.
            */
            normalPoint = GetNormalPoint(predictedLoc, pointA, pointB);

            if (!InPath(normalPoint, pointA, pointB))
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
            sb.Move(target);
        }
        else
        {
            pathNormalLR.SetColors(Color.green, Color.green);
        }

    }

    bool InPath(Vector2 normalPoint, Vector2 pointA, Vector2 pointB)
    {
        bool inX = true;
        bool inY = true;

        if (pointB.x > pointA.x)
        {
            if (normalPoint.x < pointA.x || normalPoint.x > pointB.x)
            {
                inX = false;
            }
        }
        else
        {
            if (normalPoint.x > pointA.x || normalPoint.x < pointB.x)
            {
                inX = false;
            }
        }

        if (pointB.y > pointA.y)
        {
            if (normalPoint.y < pointA.y || normalPoint.y > pointB.y)
            {
                inY = false;
            }
        }
        else
        {
            if (normalPoint.y > pointA.y || normalPoint.y < pointB.y)
            {
                inY = false;
            }
        }

        return (inX == false && inY == false) ? false : true;
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

    public PathState CurrentPath()
    {
        return pathState;
    }

}
