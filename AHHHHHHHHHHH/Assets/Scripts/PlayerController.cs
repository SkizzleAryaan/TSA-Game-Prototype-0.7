using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Start()
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;



    //FSM
    private enum State { idle, running, jumping, falling, hurt, attack, airattack }
    private State state = State.idle;


    //Inspector
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int cherry = 0;
    [SerializeField] private float hurtForce = .1f;


    private bool canJump;

    //Attacking
    public Transform attackPoint;
    public Transform DairPoint; 
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackRate = 2f;
    float nextAttackTime = 1f;

    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        PermUI.perm.healthAmount.text = PermUI.perm.health.ToString();
    }

    private void Update()
    {
        if (state != State.hurt)
        {
            Movement();

            if (Time.time >= nextAttackTime)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;

                }
            }

        }



        VelocityState();
        anim.SetInteger("state", (int)state); //Set anim based on Inum state
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectible")
        {
            Destroy(collision.gameObject);
            PermUI.perm.gems++;
            PermUI.perm.gemText.text = PermUI.perm.gems.ToString();
        }

        if (collision.tag == "Powerup")
        {
            Destroy(collision.gameObject);
            cherry++;
            canJump = true;
            //Could also do speed: speed = 20f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower()); //Timer method
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            state = State.hurt;
            HandleHealth();

            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                Invoke("Zero", .5f);
            }
            else
            {
                rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                Invoke("Zero", .5f);
            }
        }
    }

    private void HandleHealth()
    {
        PermUI.perm.health--;
        PermUI.perm.healthAmount.text = PermUI.perm.health.ToString();
        if (PermUI.perm.health <= 0)
        {
            PermUI.perm.ResetAll();
            SceneManager.LoadScene("StartMenu");

        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");
        if (hDirection < 0) //Left
        {
            transform.localScale = new Vector2(-1, 1);

        }
        else if (hDirection > 0) //Right
        {
            transform.localScale = new Vector2(1, 1);
        }

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * speed;


        //Jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
            if (cherry > 0)
            {
                canJump = true;
            }
        }
        else if (canJump && Input.GetButtonDown("Jump"))
        {
            Jump();

            canJump = false;
        }


    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void VelocityState()
    {
        if (state == State.jumping || (state != State.jumping && coll.IsTouchingLayers(ground) == false))
        {
            if (rb.velocity.y < .1f)
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
        else if (Input.GetAxis("Horizontal") != 0 && coll.IsTouchingLayers(ground) == true)
        {
            //Moving
            state = State.running;
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else
        {
            state = State.idle;
        }

    }

    void Attack()
    {
        
        if (coll.IsTouchingLayers(ground) == false)
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                anim.SetTrigger("downdair");
            }
            else
            {
                anim.SetTrigger("inair");
            }
                
        }
        else if (state == State.running)
        {
            anim.SetTrigger("dashatk");
            //float x = transform.position.x;
            //float y = transform.position.y;
            //transform.position = new Vector2(x + 1, y);
        }
        else
        {
            anim.SetTrigger("atk");
        }



 

        if (Input.GetAxis("Vertical") < 0 && coll.IsTouchingLayers(ground) == false)
        {
            Collider2D[] downhitEnem = Physics2D.OverlapCircleAll(DairPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in downhitEnem)
            {
                Jump();
                canJump = true; 
                Enemy enemy1 = enemy.gameObject.GetComponent<Enemy>();
                enemy1.Attacked();

            }
        }
        else
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                Enemy enemy1 = enemy.gameObject.GetComponent<Enemy>();
                enemy1.Attacked();

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        
         if (DairPoint == null)
        {
            return; 
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(DairPoint.position, attackRange);
    }



    void Zero()
    {
        rb.velocity = Vector3.zero;
    }

    private IEnumerator ResetPower() //timer
    {
        yield return new WaitForSeconds(10);
        cherry = 0;
        GetComponent<SpriteRenderer>().color = Color.white; //white is default
    }


}
