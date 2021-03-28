using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFiniteStateMachine : MonoBehaviour
{

    enum EnemyState { PATROL, HEAD_TO, ATTACK };

    public Pathfollowing pf;
    private EnemyState state;

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
        }
    }
}
