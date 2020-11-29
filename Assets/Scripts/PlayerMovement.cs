using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ISyncHost
{
    public int PeerID { get; set; }

    Rigidbody2D rb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void UpdateInput(PlayerInputData input)
    {
        anim.SetBool("Run", input.MoveX != 0 || input.MoveY != 0);
        if (input.MoveX != 0)
        {
            transform.localScale = new Vector3(Mathf.Round(input.MoveX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        rb.velocity = new Vector2(input.MoveX, input.MoveY).normalized * 10f;
    }
}
