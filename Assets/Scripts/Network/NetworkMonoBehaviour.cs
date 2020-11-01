using System;
using UnityEngine;

public abstract class NetworkMonoBehaviour : MonoBehaviour
{
    bool started = false;

    void Update()
    {
        if (!started)
        {
            started = true;
            NetworkStart();
        }
        NetworkUpdate();
    }

    protected abstract void NetworkStart();

    protected abstract void NetworkUpdate();
}