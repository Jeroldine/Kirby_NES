using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer) )]
public class PlayerController : MonoBehaviour
{
    // Component References
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    // Pysics Materials
    [SerializeField] PhysicsMaterial2D pmRough;
    [SerializeField] PhysicsMaterial2D pmSlippery;

    // Movement Variables
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float jumpForce = 300.0f;
    [SerializeField] float flyForce = 150.0f;

    // Ground check stuff
    public bool isGrounded;
    public Transform groundCheck;
    public LayerMask isGroundLayer;
    [SerializeField] float groundCheckRadius = 0.02f;

    // Animations
    [SerializeField] bool isCrouching;
    [SerializeField] bool isSlideKicking;
    [SerializeField] bool isFlying;
    [SerializeField] float flyDelay = 0.2f;
    float flyDelayTime = 0.0f;
    float slideKickDuration = 0.5f;
    float slideKickTime = 0f;
    [SerializeField] float slideMoveSpeed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        // getting component references
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // checking variables for dirty data
        if (rb == null) Debug.Log("No RigidBody reference");
        if (sr == null) Debug.Log("No SpriteRenderer reference");
        if (anim == null) Debug.Log("No Animator reference");

        if (moveSpeed <= 0)
        {
            moveSpeed = 5.0f;
            Debug.Log("moveSpeed was set to default value");
        }

        if (jumpForce <= 0)
        {
            jumpForce = 300.0f;
            Debug.Log("jumpForce was set to default value");
        }

        if (groundCheck == null)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(gameObject.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.name = "GroundCheck";
            groundCheck = obj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, isGroundLayer);

        if (isGrounded)
            rb.sharedMaterial = pmRough;
        else
            rb.sharedMaterial = pmSlippery;

        // Jump
        if (isGrounded && !isCrouching && !isFlying && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
        else if ((!isGrounded || isFlying) && Input.GetButton("Jump"))
        {
            flyDelayTime += Time.deltaTime;
            if (flyDelayTime >= flyDelay)
            {
                isFlying = true;
                rb.velocity = new Vector2(rb.velocity.x, Vector2.up.y * flyForce);
            }
            
        }

        else if (!isGrounded && !Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        // Crouching
        if (isGrounded && !isFlying && vInput < -0.1 && !isCrouching)
        {
            isCrouching = true;
            //rb.velocity = Vector2.zero;
        }
        else if (isGrounded && vInput >= 0 && vInput < 0.1 && !isSlideKicking)
        {
            isCrouching = false;
        }

        /// B button ///
        
        // flying exhale
        if (isFlying && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Exhale");
            isFlying = false;
        }

        // Slide Kick
        if (isCrouching && Input.GetButtonDown("Fire1") && !isSlideKicking)
        {
            isSlideKicking = true;
        }

        if (isSlideKicking)
        {
            slideKickTime += Time.deltaTime;
            if (slideKickTime >= slideKickDuration)
            {
                slideKickTime = 0;
                isSlideKicking = false;
                rb.velocity = Vector2.zero;
            }
            else if (sr.flipX)
            {
                rb.velocity = new Vector2(-slideMoveSpeed, rb.velocity.y);
                Debug.Log("slide left");
            }
            else
            {
                rb.velocity = new Vector2(slideMoveSpeed, rb.velocity.y);
                Debug.Log("slide right");
            }
        }

        // Ground Movement
        if (!isCrouching)
        {
            Vector2 moveDirection = new Vector2(hInput * moveSpeed, rb.velocity.y);
            rb.velocity = moveDirection;
        }
      

        // Flipping the sprite
        if (hInput > 0 && !isSlideKicking)
            sr.flipX = false;
        else if (hInput < 0 && !isSlideKicking)
            sr.flipX = true;

        // Animation Parameters
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("vDown", rb.velocity.y);
        anim.SetFloat("hInput", Mathf.Abs(hInput));
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isSlideKicking", isSlideKicking);
        anim.SetBool("isFlying", isFlying);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            Destroy(collision.gameObject);
        }
    }
}
