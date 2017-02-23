using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMNAgent : MultiNetAgent {
    protected override void SetManager()
    {
        manager = new LinearMultiNetManager(this, timeKeep);
    }
}