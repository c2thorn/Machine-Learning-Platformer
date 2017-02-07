using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizingMNAgent : MultiNetAgent {
    protected override void Awake()
    {
        manager = new OptimizingMultiNetManager(this, timeKeep);
        base.Awake();
    }
}
