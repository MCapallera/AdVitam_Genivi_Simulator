﻿/*
 * Copyright (C) 2016, Jaguar Land Rover
 * This program is licensed under the terms and conditions of the
 * Mozilla Public License, version 2.0.  The full text of the
 * Mozilla Public License is at https://www.mozilla.org/MPL/2.0/
 */

using UnityEngine;
using System.Collections;

public class RockfallObstacle : BaseObstacle
{

    public float vertical = 10f;
    public float horizForce = 100f;
    public void Start()
    {
        transform.Translate(0f, vertical, 0f);
        triggered = true;
        OnTrigger();
    }

    public override void OnTrigger()
    {
        foreach(Transform t in transform)
        {
            t.GetComponent<Rigidbody>().isKinematic = false;
            t.GetComponent<Rigidbody>().useGravity = true;
            t.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * horizForce, ForceMode.Impulse);
        }
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }

}
