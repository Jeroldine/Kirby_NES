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
    EnemyTurret currentET;
    Shoot shoot;
    AudioSourceManager asm;

    //AnimatorClipInfo[] currentClipInfo;

    // audio clips
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;
    [SerializeField] AudioClip crouchSound;
    [SerializeField] AudioClip slideKickSound;
    [SerializeField] AudioClip inhaleSound;
    [SerializeField] AudioClip swallowSound;
    [SerializeField] AudioClip shootPuffSound;
    [SerializeField] AudioClip shootStarSound;
    [SerializeField] AudioClip damageSound;
    double nextEventTime;
    //[SerializeField] AudioClip deathSound;

    // Pysics Materials
    [SerializeField] PhysicsMaterial2D pmRough;
    [SerializeField] PhysicsMaterial2D pmSlippery;

    // Movement Variables
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float jumpForce = 300.0f;
    [SerializeField] float flyForce = 150.0f;
    [SerializeField] float extraGrav = 10.0f;

    // Ground inhaling mechanic
    [SerializeField] GameObject inhaleBoxPrefab;
    [SerializeField] Transform inhaleBoxR;
    [SerializeField] Transform inhaleBoxL;
    GameObject ib;

    // Invincibility
    [SerializeField] bool isInvincible;
    Color invincibilityColour = new Color(1, 0.9882353f, 0, 1);
    Coroutine invincibilityChange;

    // Damage
    Coroutine damageStateChange;

    // level
    bool isAtEndOfLevel;

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
    [SerializeField] bool isDamaged;
    bool startInhaleTimer;
    bool startFlyExhaleTimer;

    float flyExhaleDuration;
    float inhaleDuration;
    float stopInhaleDuration;
    float damageDuration;
    float damageFullDuration;
    float flyDelayTime = 0.0f;
    float inhaleDelay;
    float inhaleDelayTime = 0.0f;
    float flyExhaleDelayTime = 0.0f;
    float slideKickDuration = 0.5f;
    float slideKickTime = 0f;
    float colourChangeTimer = 0;
    float colourChangeDuration = 0.1f;
    [SerializeField] float slideMoveSpeed = 100.0f;
    [SerializeField] float knockbackForce;


    // Start is called before the first frame update
    void Start()
    {
        // getting component references
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        shoot = GetComponent<Shoot>();
        asm = GetComponent<AudioSourceManager>();
        ib = null;

        // checking variables for dirty data
        if (rb == null) Debug.Log("No RigidBody reference");
        if (sr == null) Debug.Log("No SpriteRenderer reference");
        if (anim == null) Debug.Log("No Animator reference");
        if (shoot == null) Debug.Log("No Shoot script attached");
        if (asm == null) Debug.Log("No AudioSourceManager script attached");

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

        if (knockbackForce <= 0)
        {
            knockbackForce = 5;
            Debug.Log("knockbackForce was set to default value");
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
        //DontDestroyOnLoad(this);
        shoot.OnPrimaryProjSpawn += OnPrimaryProjSpawned;
        shoot.OnSecondaryProjSpawn += OnSecondaryProjSpawned;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPaused) return;
        //if (isDamaged) return;

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, isGroundLayer);

        // invincibility colour change
        if (isInvincible)
        {
            colourChangeTimer += Time.deltaTime;
            if (colourChangeTimer >= colourChangeDuration)
            {
                sr.color = (sr.color != Color.white) ? Color.white : invincibilityColour;
                colourChangeTimer = 0;
            }
        }

        if (!isFlying)
        {
            rb.AddForce(Vector2.down * extraGrav * Time.deltaTime);
        }

        if (isGrounded)
            rb.sharedMaterial = pmRough;
        else
            rb.sharedMaterial = pmSlippery;

        // Jump
        if (isGrounded && !isCrouching && !isInhaling && !isFlying && !isDamaged && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * jumpForce );
            asm.PlayOneShot(jumpSound, false);
        }
        else if ((!isGrounded || isFlying) && !isInhaling && !isFull && !isDamaged && Input.GetButton("Jump"))
        {
            flyDelayTime += Time.deltaTime;
            if (flyDelayTime >= flyDelay)
            {
                isFlying = true;
                rb.velocity = new Vector2(rb.velocity.x, Vector2.up.y * flyForce);
            }
            
        }
        else if (!isGrounded && isFlying && !Input.GetButtonDown("Jump") && rb.velocity.y < -flyForce)
        {
            rb.velocity = new Vector2(rb.velocity.x, Vector2.down.y * flyForce);
        }

        // Crouching
        if (isGrounded && !isFlying && vInput < -0.1 && !isCrouching && !isInhaling && !isFull && !isDamaged)
        {
            isCrouching = true;
            asm.PlayOneShot(crouchSound, false);
            //rb.velocity = Vector2.zero;
        }
        else if (isGrounded && vInput >= 0 && vInput < 0.1 && !isSlideKicking && isCrouching)
        {
            isCrouching = false;
        }

        // leave level
        if (isAtEndOfLevel && !isSlideKicking && !isCrouching && !isDamaged && vInput > 0.1)
        {
            GameManager.Instance.currentSceneIndex++;
            GameManager.Instance.LoadLevel(GameManager.Instance.currentSceneIndex);
            //Debug.Log("You will leave the level when implemented.");
        }

        /// B button ///
        
        // inhale
        if (!isFlying && !isInhaling && !isCrouching && !isFull && !isDamaged && Input.GetButtonDown("Fire1"))
        {
            asm.PlayOneShot(inhaleSound, false);
            nextEventTime = AudioSettings.dspTime + inhaleSound.length;
            isInhaling = true;
            isInhalingTrans = true;
            rb.velocity = Vector2.zero;
            if (sr.flipX)
            {
                ib = Instantiate(inhaleBoxPrefab, inhaleBoxL.position, inhaleBoxL.rotation);
                ib.GetComponent<ibFollow>().SetFollowPos(inhaleBoxL, transform);
            }
            else
            {
                ib = Instantiate(inhaleBoxPrefab, inhaleBoxR.position, inhaleBoxR.rotation);
                ib.GetComponent<ibFollow>().SetFollowPos(inhaleBoxR, transform);
            }   
        }
        else if (!isFull && isInhaling && !isDamaged && Input.GetButton("Fire1"))
        {
            if (AudioSettings.dspTime + (inhaleSound.length/2) >= nextEventTime)// && !hasSoundQueued)
            {
                //hasSoundQueued = true;
                asm.PlayWithDelay(inhaleSound, false);
                nextEventTime += inhaleSound.length / 2;
            }
        }
        else if (!isFull && isInhaling && Input.GetButtonUp("Fire1"))
        {
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Inhale")
            {
                inhaleDelay = inhaleDuration + stopInhaleDuration;
                Debug.Log("Long inhale delay" + inhaleDelay);
            }
            /*else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "LongInhale")
            {
                inhaleDelay = stopInhaleDuration;
                Debug.Log("Short inhale delay" + inhaleDelay);
            }*/
            startInhaleTimer = true;
            isInhalingTrans = false;
            Debug.Log("StoppingInhale");
            Destroy(ib, inhaleDelay);
            ib = null;
        }
        else if (isFull && !isDamaged && Input.GetButtonDown("Fire1")) // fire star projectile
        {
            shoot.Fire(1);
            isFull = false;
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
                asm.StopClip(inhaleSound);
            }
        }
        
        // flying exhale
        if (isFlying && !isFlyExhaling && !isDamaged && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Exhale");
            isFlyExhaling = true;
            startFlyExhaleTimer = true;
            isFlying = false;
            flyDelayTime = 0;
            shoot.Fire();
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
            asm.PlayOneShot(slideKickSound, false);
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
        if (!isCrouching && !isInhaling && !isDamaged)
        {
            Vector2 moveDirection = new Vector2(hInput * moveSpeed, rb.velocity.y);
            rb.velocity = moveDirection;
        }

        // Flipping the sprite
        if (hInput > 0 && !isSlideKicking && !isInhaling && !isDamaged)
            sr.flipX = false;
        else if (hInput < 0 && !isSlideKicking && !isInhaling && !isDamaged)
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
        anim.SetBool("isFull", isFull);
        anim.SetBool("isDamaged", isDamaged);
    }

    public void PlayPickupSound(AudioClip clip)
    {
        asm.PlayOneShot(clip, false);
    }

    private void OnPrimaryProjSpawned()
    {
        asm.PlayOneShot(shootPuffSound, false);
    }

    private void OnSecondaryProjSpawned()
    {
        asm.PlayOneShot(shootStarSound, false);
    }

    public void PlaySwallowSound()
    {
        asm.PlayOneShot(swallowSound, false);
    }

    public bool GetInvincibilityState()
    {
        return isInvincible;
    }

    public void KnockBack(Transform ePos)
    {
        asm.PlayOneShot(damageSound, false);
        rb.velocity = Vector2.zero;
        Vector2 kbDirection = (transform.position.x > ePos.position.x) ? new Vector2(knockbackForce, 0) : new Vector2(-knockbackForce, 0);
        rb.AddForce(kbDirection, ForceMode2D.Impulse);
        isDamaged = true;
        StartInvincibilityChange(2f);
        if (isFull)
            StartDamageStateChange(damageFullDuration);
        else
            StartDamageStateChange(damageDuration);
        Debug.Log("damageDuration: " + damageFullDuration.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy e = collision.gameObject.GetComponent<Enemy>();
            e.TakeDamage(1, 2);
            e.GetComponent<BoxCollider2D>().isTrigger = true;
            if (!isInvincible)
            {
                GameManager.Instance.currentHP--;
                KnockBack(e.transform);
            }
        }
        if (collision.gameObject.CompareTag("Level") && !isFlying && isGrounded)
        {
            asm.PlayOneShot(landSound, false);
            flyDelayTime = 0;
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndOfLevel"))
            isAtEndOfLevel = true;

        if (collision.CompareTag("Radar"))
        {
            currentET = collision.GetComponentInParent<EnemyTurret>();
            currentET.SetFireState(true);
        }

        if (collision.CompareTag("Enemy") && isInhaling)
        {
            isFull = true;
            startInhaleTimer = true;
            isInhalingTrans = false;
            asm.StopClip(inhaleSound);
            asm.PlayOneShot(swallowSound, false);
            collision.GetComponent<Enemy>().TakeDamage(1, 3);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Radar"))
        {
            currentET.SetFireState(false);
            currentET.ResetTimer();
            currentET = null;
        }
        else if (collision.CompareTag("EndOfLevel"))
            isAtEndOfLevel = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Radar"))
        {
            float angle = Mathf.Atan2(transform.position.y - collision.transform.position.y, transform.position.x - collision.transform.position.x);
            currentET.setAnimStates(angle);
            //Debug.Log("The angle is: " + angle.ToString());
        }
        if (collision.CompareTag("Enemy") && isInhaling)
        {
            isFull = true;
            startInhaleTimer = true;
            isInhalingTrans = false;
            asm.StopClip(inhaleSound);
            asm.PlayOneShot(swallowSound, false);
            collision.GetComponent<Enemy>().TakeDamage(1, 3);
        }
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
                case "Damage":
                    damageDuration = clip.length;
                    break;
                case "DamageFull":
                    damageFullDuration = clip.length;
                    break;
            }
        }
    }

    public void StartDamageStateChange(float duration)
    {
        if (damageStateChange == null)
            damageStateChange = StartCoroutine(DamageStateChange(duration));
        else
        {
            StopCoroutine(damageStateChange);
            damageStateChange = null;
            isDamaged = false;
            damageStateChange = StartCoroutine(DamageStateChange(duration));
        }
    }

    IEnumerator DamageStateChange(float duration)
    {
        isDamaged = true;
        yield return new WaitForSeconds(duration);
        isDamaged = false;
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
        colourChangeTimer = 0;
        sr.color = Color.white;
        invincibilityChange = null;
    }

    public void ResetPlayerStates()
    {
        isCrouching = false;
        isSlideKicking = false;
        isFlying = false;
        isFlyingTrans = false;
        isFlyExhaling = false;
        isInhaling = false;
        isInhalingTrans = false;
        isFull = false;
        isInvincible = false;
    }

    public bool GetInhalingState()
    {
        return isInhaling;
    }

    public void ResetDamagedState()
    {
        isDamaged = false;
    }
}