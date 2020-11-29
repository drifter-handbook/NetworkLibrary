## Warning
Don't use Awake() to perform network-related activities. It happens before network init can happen, meaning if you access network components during Awake(), you will get an error.

## Setting username
Accessible via the inspector, or via GameController.Instance.Username

## NetworkStartingEntities
In every networked scene with existing networked entities in the scene, drag in the NetworkStartingEntities prefab.
Add any existing networked entities into the scene.

## CreateNetworkObject
To create a new networked object, use NetworkUtils.CreateNetworkObject(string networkType)
To register a prefab to work with CreateNetworkObject, drag the prefab into the GameController's NetworkObjects component's "Network Type Prefabs" list.

## Networked Child Objects
When an object begins being tracked by the Network system, it calls NetworkInit on any MonoBehaviours that inherit from INetworkInit. This is useful for registering child objects, which must be done in this way. Registration of child objects must be done in the same order or clients and hosts will have mismatched network object data.
```cs
public class ChildObjectTest : MonoBehaviour, INetworkInit
{
    public void OnNetworkInit()
    {
        NetworkUtils.RegisterChildObject("TestChildObject", transform.Find("TestChild").gameObject);
    }
}
```

## Changing scenes
To change scenes, use the GameController.Instance.host.SetScene(string sceneName) method.
Connected clients will also change scenes.

## Synced properties
Properties can be synced. For example, exampleToggle in the code below:
```cs
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
```

## Network Messages
Instead of a persistent variable, you can instead send a one-time message.
```cs
public class MessageTest : MonoBehaviour, INetworkMessageReceiver
{
    NetworkSync sync;

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        if (GameController.Instance.IsHost)
        {
            sync.SendNetworkMessage("Hello!", DeliveryMethod.ReliableOrdered);
        }
    }

    public void ReceiveNetworkMessage(NetworkMessage message)
    {
        if (!GameController.Instance.IsHost)
        {
            Debug.Log((string)message.contents);
        }
    }
}
```

## Syncing complex objects
You can also sync complex objects, as shown with the built-in network sync script, SyncTransform.
```cs
public class SyncTransform : MonoBehaviour
{
    NetworkSync sync;

    SyncableTransform2D netTransform
    {
        get { return NetworkUtils.GetNetworkData<SyncableTransform2D>(sync["transform"]); }
        set { sync["transform"] = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        sync = GetComponent<NetworkSync>();
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.IsHost)
        {
            netTransform = new SyncableTransform2D()
            {
                position = new SyncableVector3(transform.position),
                rotation = transform.eulerAngles.z,
                scale = new SyncableVector2(transform.localScale)
            };
        }
        else if (netTransform != null)
        {
            transform.position = netTransform.position.ToVector3();
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, netTransform.rotation);
            Vector2 netScale = netTransform.scale.ToVector2();
            transform.localScale = new Vector3(netScale.x, netScale.y, transform.localScale.z);
        }
    }
}
```
NetworkMessages can also send complex objects in the same way.

## Client-to-host sync
Clients can also sync data with the host and send messages to the host, using the NetworkSyncToHost component.
```cs
public class SyncInput : MonoBehaviour, ISyncClient
{
    NetworkSyncToHost syncToHost;

    // Start is called before the first frame update
    void Start()
    {
        syncToHost = GetComponent<NetworkSyncToHost>();
    }

    // Update is called once per frame
    void Update()
    {
        syncToHost["input"] = NetworkPlayers.GetInput();
    }
}
```
The host can then access this data:
```cs
foreach (int peerID in GameController.Instance.host.Peers)
{
    input = NetworkUtils.GetNetworkData<PlayerInputData>(syncFromClients["input", peerID]);
    if (input != null)
    {
        UpdateInput(clientPlayers[peerID], input);
    }
}
```

## Cleanup
To end networking, call GameController.Instance.CleanupNetwork()
Make sure to also use SceneManager.LoadScene(scene) to move to a scene without networked components, since those won't work when networking has been ended.
