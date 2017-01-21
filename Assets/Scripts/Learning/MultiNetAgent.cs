using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiNetAgent : NEAgent
{
    private static MultiNetManager manager = new MultiNetManager(); 

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        restart = timeKeep.getRestart();
        net = manager.getCurrentNet();
    }
}