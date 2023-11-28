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
    //AnimatorClipInfo[] currentClipInfo;

    // Pysics Materials
    [SerializeField] PhysicsMaterial2D pmRough;
    [SerializeField] PhysicsMaterial2D pmSlippery;

    // Movement Variables
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float jumpForce = 300.0f;
    [SerializeField] float flyForce = 150.0f;
    [SerializeField] float extraGrav = 10.0f;

    // Health and lives
    int maxHP = 5;
    private int _currentHP = 4;
    public int currentHP
    {
        get { return _currentHP; }
        set
        {
            _currentHP = value;
            if (_currentHP > maxHP)
                _currentHP = maxHP;

            Debug.Log("HP has been set to: " + _currentHP.ToString());
        }
    }

    int maxLives = 4;
    private int _currentLives = 3;
    public int currentLives
    {
        get { return _currentLives; }
        set
        {
            _currentLives = value;
            if (_currentLives > maxLives)
                _currentLives = maxLives;

            Debug.Log("Lives has been set to: " + _currentLives.ToString());
        }
    }

    // Invincibility
    [SerializeField] bool isInvincible;
    Coroutine invincibilityChange;

    // Ground check stuff
    public bool isGrounded;
    public Transform groundCheck;
    public LayerMask isGroundLayer;
    [SerializeField] float groundCheckRadius = 0.02f;

    // Animations
    [SerializeField] bool isCrouching;
    [SerializeField] bool isSlideKicking;
    [SerializeField] bool isFlying;
    [SerializeField] bool isFlyingTrans;
    [SerializeField] bool isFlyExhaling;
    [SerializeField] bool isInhaling;
    [SerializeField] bool isInhalingTrans;
    [SerializeField] bool isFull;
    [SerializeField] float flyDelay = 0.2f;
    bool startInhaleTimer;
    bool startFlyExhaleTimer;

    float flyExhaleDuration;
    float inhaleDuration;
    float stopInhaleDuration;
    float flyDelayTime = 0.0f;
    float inhaleDelay;
    float inhaleDelayTime = 0.0f;
    float flyExhaleDelayTime = 0.0f;
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

        GetAnimClipDurations();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, isGroundLayer);

        if (!isFlying)
        {
            rb.AddForce(Vector2.down * extraGrav * Time.deltaTime);
        }

        if (isGrounded)
            rb.sharedMaterial = pmRough;
        else
            rb.sharedMaterial = pmSlippery;

        // Jump
        if (isGrounded && !isCrouching && !isInhaling && !isFlying && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * jumpForce );
        }
        else if ((!isGrounded || isFlying) && !isInhaling && Input.GetButton("Jump"))
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
        if (isGrounded && !isFlying && vInput < -0.1 && !isCrouching && !isInhaling)
        {
            isCrouching = true;
            //rb.velocity = Vector2.zero;
        }
        else if (isGrounded && vInput >= 0 && vInput < 0.1 && !isSlideKicking)
        {
            isCrouching = false;
        }

        /// B button ///
        
        // inhale
        if (!isFlying && !isInhaling && !isCrouching && !isFull && Input.GetButtonDown("Fire1"))
        {
            isInhaling = true;
            isInhalingTrans = true;
            rb.velocity = Vector2.zero;
        }
        else if (isInhaling && Input.GetButtonUp("Fire1"))
        {
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Inhale")
            {
                inhaleDelay = inhaleDuration + stopInhaleDuration;
                Debug.Log("Long inhale delay" + inhaleDelay);
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "LongInhale")
            {
                inhaleDelay = stopInhaleDuration;
                Debug.Log("Short inhale delay" + inhaleDelay);
            }
            startInhaleTimer = true;
            isInhalingTrans = false;
            Debug.Log("StoppingInhale");
        }
        if (startInhaleTimer)
        {
            inhaleDelayTime += Time.deltaTime;
            if (inhaleDelayTime >= inhaleDelay)
            {
                isInhaling = false;
                startInhaleTimer = false;
                inhaleDelay = 0;
                inhaleDelayTime = 0;
            }
        }

        // flying exhale
        if (isFlying && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Exhale");
            isFlyExhaling = true;
            startFlyExhaleTimer = true;
            isFlying = false;
            flyDelayTime = 0;
        }
        else if (startFlyExhaleTimer)
        {
            rb.velocity = Vector2.zero;
            flyExhaleDelayTime += Time.deltaTime;
            if (flyExhaleDelayTime >= flyExhaleDuration)
            {
                isFlyExhaling = false;
                flyExhaleDelayTime = 0;
                startFlyExhaleTimer = false;
            }
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
        if (!isCrouching && !isInhaling)
        {
            Vector2 moveDirection = new Vector2(hInput * moveSpeed, rb.velocity.y);
            rb.velocity = moveDirection;
        }
      

        // Flipping the sprite
        if (hInput > 0 && !isSlideKicking && !isInhaling)
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
        anim.SetBool("isInhaling", isInhalingTrans);
        anim.SetBool("isFlyExhaling", isFlyExhaling);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void GetAnimClipDurations()
    {
        foreach(AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            switch(clip.name)
            {
                case "Inhale":
                    inhaleDuration = clip.length;
                    break;
                case "StopInhale":
                    stopInhaleDuration = clip.length;
                    break;
                case "FlyExhale":
                    flyExhaleDuration = clip.length;
                    break;
            }
        }
    }

    public void StartInvincibilityChange(float duration)
    {
        if (invincibilityChange == null)
            invincibilityChange = StartCoroutine(InvincibilityChange(duration));
        else
        {
            StopCoroutine(invincibilityChange);
            invincibilityChange = null;
            isInvincible = false;
            invincibilityChange = StartCoroutine(InvincibilityChange(duration));
        }
    }

    IEnumerator InvincibilityChange(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        invincibilityChange = null;
    }
}
