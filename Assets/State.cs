using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    public State StartAction(SharkFSM shark);
    public State StandardAction(SharkFSM shark);


}
