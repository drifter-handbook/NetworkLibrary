﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncAnimatorClient : MonoBehaviour, ISyncClient
{
    NetworkSync sync;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        anim = GetComponent<Animator>();
    }

    void SetAnimatorParameterValue(AnimatorControllerParameterType type, string name, object value)
    {
        switch (type)
        {
            case AnimatorControllerParameterType.Bool:
                anim.SetBool(name, (bool)value);
                break;
            case AnimatorControllerParameterType.Int:
                anim.SetInteger(name, (int)value);
                break;
            case AnimatorControllerParameterType.Float:
                anim.SetFloat(name, (float)value);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        List<SyncAnimatorParameter> parameters = NetworkUtils.GetNetworkData<List<SyncAnimatorParameter>>(sync["animator_parameters"].ToString());
        foreach (SyncAnimatorParameter parameter in parameters)
        {
            SetAnimatorParameterValue(parameter.type, parameter.name, parameter.value);
        }
    }
}
