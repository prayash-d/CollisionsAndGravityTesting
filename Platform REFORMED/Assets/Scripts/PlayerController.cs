using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //gives access to text and UI stuff

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    /*[SerializeField]*/ private Animator anim; //serializing keeps instance visible in manager (ie. rb no longer visible, anim is)
    private Collider2D coll;

    

    //FSM
    private enum State { idle, running, jumping, falling, hurt }
    private State state = State.idle;

    //Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private int coins = 0;
    [SerializeField] private Text coinText;
    [SerializeField] private float hurtForce = 10f;

    private void Start()//runs when object becomes active/script becomes active
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {

        if (state != State.hurt)
        {
            Movement();
        }

        //Movement();
        AnimationState(); //method to change states of movement
        anim.SetInteger("state", (int)state); //sets the animation based on Enumerator state

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable") //checks if a gameObject of tag "Collectable" is hit (equals it)
        {
            Destroy(collision.gameObject); //destroy the collectable
            coins += 1;
            coinText.text = coins.ToString(); //ToString converts anything that isn't a string into a string!
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            if (state == State.falling)
            {
                Destroy(other.gameObject);
                Jump(); //jump after landing on top of enemy
            }

            else
            {
                state = State.hurt;

                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //enermy is to my right, therefore i should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }

                else
                {
                    //enemy is to my left, therefore i should be damaged and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }

            }
            
        }
    }

    private void Movement()
    {
        //before we used: if (Input.GetKey(KeyCode.A))
        float hDirection = Input.GetAxis("Horizontal");
        if (hDirection < 0)
        { //Input->class, GetKey->method, KeyCode.A->parameters for the method
            rb.velocity = new Vector2(-speed, rb.velocity.y); //y velocity in effect (w.o this, if player held a or d, the body would not fall down if there was no ground
            transform.localScale = new Vector2(-1, 1);
            //anim.SetBool("running", true); (dont need anymore bc we implemented state enum)

        }

        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            //anim.SetBool("running", true);

        }

        //jump condition
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;
    }

    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if(rb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }

        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        //hurt condition
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1f) //checks if velocity is close to zero (abs val bc entity may move right OR left)
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 2f) //mathf.epsilon is smallest number possible
        {
            //now moving!
            state = State.running;
        }

        else
        {
            state = State.idle;
        }
    }

}
