using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMNAgent : MultiNetAgent {
    protected override void Awake()
    {
        manager = new LinearMultiNetManager(this, timeKeep);
        base.Awake();
    }
}