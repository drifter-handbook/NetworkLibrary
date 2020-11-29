using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTest : MonoBehaviour
{
    NetworkSync sync;

    public bool exampleToggle
    {
        get { return (bool)sync["example"]; }
        set { sync["example"] = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        if (GameController.Instance.IsHost)
        {
            exampleToggle = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.IsHost)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                exampleToggle = !exampleToggle;
            }
        }
        else
        {
            Debug.Log(exampleToggle);
        }
    }
}
