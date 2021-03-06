using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem ;

public class Character_Controller : MonoBehaviour
{

    float h;
    bool isAlive = true;
    AudioSource audioSource;
    [SerializeField] float  run_speed= 0f ;
    [SerializeField] float jump_speed = 0f;
    [SerializeField] float climb_speed = 0f;
    [SerializeField] Vector2 deathkick = new Vector2(10f, 10f);
    [SerializeField] Sprite climb_sprite ;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    SpriteRenderer spriterenderer;
    float gravity_value;
    Rigidbody2D rb;
    Vector2 v;
    bool jump;
    Animator animator;
    BoxCollider2D col;
    CapsuleCollider2D cap_collider;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity_value = rb.gravityScale;
        animator = GetComponent<Animator>();
        col = GetComponentInChildren<BoxCollider2D>();
        cap_collider = GetComponent<CapsuleCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        // audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isAlive)
            return;
        
        Climbing();
        Die();
    }

    private void FixedUpdate()
    {
        if (!isAlive)
            return;
        run_animation();
        Run();
    }

    void OnMove(InputValue input)
    {
        if (!isAlive)
            return;
        v = input.Get<Vector2>();
    }

    void OnFire(InputValue input)
    {
        if (!isAlive || animator.GetBool("climb_sprite") || animator.GetBool("climb"))
            return ;
        if(input.isPressed)
        {
            Instantiate(bullet, gun.position, transform.rotation);
        }
    }
    void OnJump(InputValue input)
    {
        if (!isAlive)
            return;
        if (!col.IsTouchingLayers(LayerMask.GetMask("platform")))
        {
            return;
        }
        if(input.isPressed )
        {
            rb.velocity += new Vector2(0f, jump_speed);
        }
    }
    void Run()
    {
        if (!col.IsTouchingLayers(LayerMask.GetMask("platform")) && cap_collider.IsTouchingLayers(LayerMask.GetMask("platform")))
            return;
        if (v.x < 0)
        {
            transform.localScale = new Vector2((-1) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (v.x > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        rb.velocity =new Vector2(v.x * run_speed, rb.velocity.y) ;
    }

    void run_animation()
    {
        if(v.x!=0)
            animator.SetBool("run", true);
        else
            animator.SetBool("run", false);
    }

    void Climbing()
    {
        if (!cap_collider.IsTouchingLayers(LayerMask.GetMask("climbing")))
        {
            rb.gravityScale = gravity_value;
            animator.SetBool("climb_sprite", false);
            animator.SetBool("climb", false);
            return;
        }
        else
        {
            animator.SetBool("climb_sprite", true);
        }

        if (v.y != 0 && v.x==0) 
            animator.SetBool("climb", true);
        else
            animator.SetBool("climb", false);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, v.y * climb_speed * Time.deltaTime);
    }

    void Die()
    {
        if(cap_collider.IsTouchingLayers(LayerMask.GetMask("Enemy")) || cap_collider.IsTouchingLayers(LayerMask.GetMask("Hazard")))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            rb.velocity = deathkick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        
    }

}
