using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncAnimator : MonoBehaviour, INetworkMessageReceiver
{
    NetworkSync sync;

    List<SyncAnimatorParameter> parameters = new List<SyncAnimatorParameter>();

    Animator anim;

    SyncAnimatorData animatorParameters
    {
        get { return NetworkUtils.GetNetworkData<SyncAnimatorData>(sync["animator_parameters"]); }
        set { sync["animator_parameters"] = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        anim = GetComponent<Animator>();
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.type != AnimatorControllerParameterType.Trigger)
            {
                parameters.Add(new SyncAnimatorParameter()
                {
                    name = parameter.name,
                    type = parameter.type,
                    value = GetAnimatorParameterValue(parameter.type, parameter.name)
                });
            }
        }
    }

    object GetAnimatorParameterValue(AnimatorControllerParameterType type, string name)
    {
        switch(type)
        {
            case AnimatorControllerParameterType.Bool:
                return anim.GetBool(name);
            case AnimatorControllerParameterType.Int:
                return anim.GetInteger(name);
            case AnimatorControllerParameterType.Float:
                return anim.GetFloat(name);
        }
        return null;
    }

    void SetAnimatorParameterValue(AnimatorControllerParameterType type, string name, object value)
    {
        switch (type)
        {
            case AnimatorControllerParameterType.Bool:
                anim.SetBool(name, (bool)value);
                break;
            case AnimatorControllerParameterType.Int:
                anim.SetInteger(name, (int)(long)value);
                break;
            case AnimatorControllerParameterType.Float:
                anim.SetFloat(name, (float)(double)value);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.IsHost)
        {
            foreach (SyncAnimatorParameter parameter in parameters)
            {
                parameter.value = GetAnimatorParameterValue(parameter.type, parameter.name);
            }
            animatorParameters = new SyncAnimatorData() { parameters = parameters };
        }
        else if (animatorParameters != null)
        {
            try
            {
                foreach (SyncAnimatorParameter parameter in animatorParameters.parameters)
                {
                    SetAnimatorParameterValue(parameter.type, parameter.name, parameter.value);
                }
            }
            catch (KeyNotFoundException)
            {
                // host hasn't sent anything yet
            }
        }
    }

    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
        if (GameController.Instance.IsHost)
        {
            sync.SendNetworkMessage(new SyncAnimatorTriggerMessage() { name = name });
        }
    }

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        SyncAnimatorTriggerMessage trigger = NetworkUtils.GetNetworkData<SyncAnimatorTriggerMessage>(message.contents);
        if (trigger != null)
        {
            anim.SetTrigger(trigger.name);
        }
    }
}

public class SyncAnimatorParameter
{
    public string name;
    public AnimatorControllerParameterType type;
    public object value;
}

public class SyncAnimatorData : INetworkData
{
    public string Type { get; set; }
    public List<SyncAnimatorParameter> parameters;
}

public class SyncAnimatorTriggerMessage : INetworkData
{
    public string Type { get; set; }
    public string name;
}
