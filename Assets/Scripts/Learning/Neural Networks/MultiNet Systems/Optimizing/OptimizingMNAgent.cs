using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizingMNAgent : MultiNetAgent {
    protected override void SetManager()
    {
        manager = new OptimizingMultiNetManager(this, timeKeep);
    }
}
