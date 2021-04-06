using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkFSM : FSMObject
{
    enum SharkState
    {
        ROAM,
        PATROL,
        IN_SIGHT,
        LOSS_SIGHT,
    }

    [Header("Standard Attributes")]
    public Pathfollowing pf;
    public VisionCone vc;
    public TrailRenderer tr;
    
    [Header("state attributes")]
    public float circlePathDistance;
    public float roamDistanceMultiplier;

    [Header("Time Attributes")]
    public float stopPatrolTime;
    public float roamTime;
    public float roamUpdatePathTime;
    public float updatePathTime;
    public float lossSightTime;

    private SharkState currentState;
    private float timer;
    private float secondary_timer;
    private GameObject entity;
    private bool lastPathHeadTo;
    private Gradient patrolColour;
    private Gradient roamColour;
    private Gradient inSightColour;
    private Gradient lossSightColour;

    void Start()
    {
        patrolColour = new Gradient();
        patrolColour.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        roamColour = new Gradient();
        roamColour.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        inSightColour = new Gradient();
        inSightColour.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        lossSightColour = new Gradient();
        lossSightColour.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.yellow, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );


        pf.GeneratePath();
        currentState = SharkState.PATROL;
        PatrolStateStart(true);
    }

    private void FixedUpdate()
    {
        if (currentState == SharkState.PATROL)
        {
            PatrolState();
        }

        if (currentState == SharkState.ROAM)
        {
            RoamState();
        }

        if (currentState == SharkState.IN_SIGHT)
        {
            InSightState();
        }

        if (currentState == SharkState.LOSS_SIGHT)
        {
            LossSightState();
        }

        Debug.Log(currentState);
    }

    /*
    * Need to make bool option as loss sight state already generates a 
    * circle, so generating another one is not neccessary.
    */
    void PatrolStateStart(bool generateCircle)
    {
        tr.colorGradient = patrolColour;
        if (generateCircle)
            pf.GenerateCirclePath(transform.position);
        timer = stopPatrolTime;
    }

    void PatrolState()
    {
        pf.FollowPath();

        timer -= Time.deltaTime;
        entity = vc.GetObjectInVisionCone(vc.locateRadius);

        if (timer < 0)
        {
            currentState = SharkState.ROAM;
            RoamStateStart();
        }
        
        if (entity != null)
        {
            currentState = SharkState.IN_SIGHT;
            InSightStateStart();
        }
    }

    void RoamStateStart()
    {
        tr.colorGradient = roamColour;
        pf.GenerateHeadToPath((Vector2)(transform.position + transform.up * roamDistanceMultiplier));
        lastPathHeadTo = true;
        timer = roamTime;
        secondary_timer = roamUpdatePathTime;
    }

    void RoamState()
    {
        pf.FollowPath();

        timer -= Time.deltaTime;
        secondary_timer -= Time.deltaTime;

        entity = vc.GetObjectInVisionCone(vc.locateRadius);

        if (timer < 0)
        {
            currentState = SharkState.PATROL;
            PatrolStateStart(true);
        }

        if (secondary_timer < 0)
        {
            if (lastPathHeadTo)
            {
                pf.GenerateCirclePath((Vector2)(transform.position));
                lastPathHeadTo = false;
            }
            else
            {
                pf.GenerateHeadToPath((Vector2)(transform.position + transform.up * roamDistanceMultiplier));
                lastPathHeadTo = true;
            }
            secondary_timer = roamUpdatePathTime;
        }

        if (entity != null)
        {
            currentState = SharkState.IN_SIGHT;
            InSightStateStart();
        }
    }

    void InSightStateStart()
    {
        tr.colorGradient = inSightColour;
        pf.GenerateHeadToPath(entity.transform.position);
        timer = updatePathTime;
    }

    void InSightState()
    {
        pf.FollowPath();

        timer -= Time.deltaTime;

        if (timer < 0.0)
        {
            timer = updatePathTime;

            entity = (pf.CurrentPath() != Pathfollowing.PathState.CIRCLE) ?
                vc.GetObjectInVisionCone(vc.locateRadius) :
                vc.GetObjectInVisionCone(0f);

            if (entity == null)
            {
                currentState = SharkState.LOSS_SIGHT;
                LossSightStateStart();
            }
            else
            {
                float distance = VectorUtility.distance(transform.position, entity.transform.position);
                if (distance < circlePathDistance && (pf.CurrentPath() != Pathfollowing.PathState.CIRCLE))
                {
                    pf.GenerateCirclePath(entity.transform.position);
                }
                else if (distance > circlePathDistance)
                {
                    pf.GenerateHeadToPath(entity.transform.position);
                }
            }
        }
    }

    void LossSightStateStart()
    {
        tr.colorGradient = lossSightColour;
        pf.GenerateCirclePath(transform.position);
        timer = lossSightTime;
    }

    void LossSightState()
    {
        pf.FollowPath();

        timer -= Time.deltaTime;

        entity = vc.GetObjectInVisionCone(vc.locateRadius);

        if (entity != null)
        {
            currentState = SharkState.IN_SIGHT;
            InSightStateStart();
        }

        if (timer < 0)
        {
            currentState = SharkState.PATROL;
            PatrolStateStart(false);
        }
    }
}
