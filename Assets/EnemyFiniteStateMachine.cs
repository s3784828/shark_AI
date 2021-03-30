using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFiniteStateMachine : MonoBehaviour
{

    enum EnemyState { PATROL, HEAD_TO, ATTACK };

    public VisionCone vc;
    public Pathfollowing pf;

    private EnemyState state;
    private Vector2 headToPos;

    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.PATROL;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == EnemyState.PATROL)
        {
            pf.FollowPath();
            headToPos = vc.GetObjectPosInVisionCone();

            if (headToPos != Vector2.zero)
            {
                state = EnemyState.HEAD_TO;
                pf.GenerateHeadToPath(headToPos);
            }
            
        }

        if (state == EnemyState.HEAD_TO)
        {
            headToPos = vc.GetObjectPosInVisionCone();
            pf.GenerateHeadToPath(headToPos);
            pf.FollowPath();
        }
    }
}
