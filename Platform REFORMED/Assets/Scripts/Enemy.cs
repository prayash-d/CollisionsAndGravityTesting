using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float moveLength = 10f;
    [SerializeField] private float moveHeight = 0f;

    [SerializeField] private LayerMask ground;

    private Collider2D coll;
    private Rigidbody2D rb;
    private Animator anim;

    private bool facingRight = true; //if facing right, value is x=1, if facing right the value is x=-1

    private void Start() //sort of like a get method in Java (constantly updating and getting the new values
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();

        if (anim.GetBool("Aggravate"))
        {

            if (rb.velocity.x < 0.1f)
            {
                anim.SetBool("Aggravate", false);

                if (rb.position.x > 0)
                {
                    anim.SetBool("Warp", true);
                    //anim.SetBool("Aggravate", true);

                }

            }

            if(coll.IsTouchingLayers(ground) && anim.GetBool("Warp"))
            {
                anim.SetBool("Warp", false);
            }

        }
   
        else if (anim.GetBool("Warp"))
        {
            if (rb.velocity.x > 0.1f)
            {
                anim.SetBool("Warp", false);
                if (rb.position.x < 0)
                {
                    anim.SetBool("Aggravate", true);
                    //anim.SetBool("Aggravate", true);

                }
            }
        }
        //transition from move to idle
        /*if (coll.IsTouchingLayers(ground) && anim.GetBool("Moving"))
        {
            anim.SetBool("Moving", false);
        }*/
    }

    private void Move()
    {
        if (facingRight)
        {
            if (transform.position.x < rightCap) //to the left of rightCap
            {
                //makes sure sprite is facing right direction, and if it is not, then face right direction
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1); //facing right
                }
                //test to see if on ground
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(moveLength, moveHeight); //move right
                    anim.SetBool("Aggravate", true);
                    /*if (rb.transform.position.x < 0)
                    {
                        anim.SetBool("Aggravate", true);
                    }*/


                }
            }
            else //since position > rightCap, we dont want to move to the right anymore; want to move left now
            {
                facingRight = false; //if beyond right cap, no longer face right --> now face left
            }

        }

        else
        {
            if (transform.position.x > leftCap) //to the right of leftCap
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1); //facing left
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-moveLength, moveHeight); //move left
                    //anim.SetBool("Aggravate", true);
                    anim.SetBool("Warp", true);
                    /*if (rb.transform.position.x > 0)
                    {
                        anim.SetBool("Aggravate", true);
                    }*/
                }
            }
            else //since position < leftCap, want to move right now
            {
                facingRight = true; //goes back to facing right
            }
        }
    }


}
