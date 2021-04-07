using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphedTankFSM : FSMObject
{
    [Header("MorphedTankAttributes")]
    public TrailRenderer secondTR;

    protected override void PatrolStateStart(bool generateCircle)
    {
        secondTR.colorGradient = patrolColour;
        base.PatrolStateStart(generateCircle);
    }

    protected override void RoamStateStart()
    {
        secondTR.colorGradient = roamColour;
        base.RoamStateStart();
    }

    protected override void InSightStateStart()
    {
        secondTR.colorGradient = inSightColour;
        base.InSightStateStart();
    }

    protected override void LossSightStateStart()
    {
        secondTR.colorGradient = lossSightColour;
        base.LossSightStateStart();
    }
}
