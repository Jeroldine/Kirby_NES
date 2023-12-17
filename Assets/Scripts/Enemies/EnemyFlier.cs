using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFlier : Enemy
{
    Rigidbody2D rb;
    AudioSourceManager asm;

    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;
    [SerializeField] float period;
    [SerializeField] protected AudioClip deathSound;

    float timer = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        asm = GetComponent<AudioSourceManager>();

        if (!asm) Debug.Log("AudioSourceManager script not attached.");
        if (xSpeed <= 0)
            xSpeed = 2;
        if (ySpeed <= 0)
            ySpeed = 2;
        if (period <= 0)
            period = 4;

        OnDeath += EnemyDied;
    }

    private void EnemyDied()
    {
        asm.PlayOneShot(deathSound, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInhaled)
        {
            FollowKirby();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= period)
        {
            timer = 0;
            sr.flipX = !sr.flipX;
        }

        AnimatorClipInfo[] currentAnimClips = anim.GetCurrentAnimatorClipInfo(0);
        if (currentAnimClips[0].clip.name == "Fly")
        {
            float newYSpeed = ySpeed * Mathf.Cos(((2 * Mathf.PI) / period) * timer);
            rb.velocity = sr.flipX ? (new Vector2(xSpeed, newYSpeed)) : (new Vector2(-xSpeed, newYSpeed));
        }
            
        else if (currentAnimClips[0].clip.name == "PuffDeath" || currentAnimClips[0].clip.name == "StarDeath")
            rb.velocity = Vector2.zero;
    }

    public override void FollowKirby()
    {
        //inhaledSpeedX += gainX * (followPos.position.x - transform.position.x);
        inhaledSpeedY += gainY * (followPos.position.y - transform.position.y);
        //Debug.Log(inhaledSpeedX);
        rb.AddForce(new Vector2(inhaledSpeedX, inhaledSpeedY));
    }

    public override void TakeDamage(int damage, int proj)
    {
        base.TakeDamage(damage, proj);
        Debug.Log("EnemyFlier took " + damage.ToString() + " damage.");
    }
}
