using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ISyncHost
{
    public int PeerID { get; set; }

    public int InputX { get; set; }
    public int InputY { get; set; }

    Rigidbody2D rb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Run", InputX != 0 || InputY != 0);
        if (InputX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(InputX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        rb.velocity = new Vector2(InputX, InputY).normalized * 10f;
    }
}
