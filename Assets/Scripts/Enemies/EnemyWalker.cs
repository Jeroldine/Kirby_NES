using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyWalker : Enemy
{

    Rigidbody2D rb;

    [SerializeField] float xSpeed;

    bool isInhaled;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        isInhaled = false;

        if (xSpeed <= 0)
            xSpeed = 2;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] currentAnimClips = anim.GetCurrentAnimatorClipInfo(0);
        if (currentAnimClips[0].clip.name == "Walk")
            rb.velocity = sr.flipX ? (new Vector2(xSpeed, rb.velocity.y)) : (new Vector2(-xSpeed, rb.velocity.y));
        else if (currentAnimClips[0].clip.name == "PuffDeath" || currentAnimClips[0].clip.name == "StarDeath")
            rb.velocity = Vector2.zero;
    }

    public override void TakeDamage(int damage, int proj)
    {
        base.TakeDamage(damage, proj);
        Debug.Log("EnemyWalker took " + damage.ToString() + " damage.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier") && !isInhaled)
            sr.flipX = !sr.flipX;
    }
}
