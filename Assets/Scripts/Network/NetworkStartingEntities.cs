using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkStartingEntities : MonoBehaviour
{
    public List<GameObject> startingEntities;

    void Awake()
    {
        if (GameController.Instance.client != null)
        {
            foreach (GameObject obj in startingEntities)
            {
                obj.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
