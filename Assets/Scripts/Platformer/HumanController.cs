using UnityEngine;
using System.Collections;
using System;

public class HumanController : Player
{
    protected void Start()
    {
        timeKeep.ChangeTimeScale(1f);
        timeKeep.ChangeTimeOut(60f);
    }

    protected override void GetActions()
    {
        actions[2] = Input.GetButton("Jump");

        float horizontal = Input.GetAxis("Horizontal");

        actions[1] = horizontal < 0;
        actions[0] = horizontal > 0;
    }
}