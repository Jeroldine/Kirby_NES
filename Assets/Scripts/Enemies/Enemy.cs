using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Animator anim;
    
    protected int health;
    public int maxHealth;



    public virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (maxHealth <= 0)
            maxHealth = 10;
        health = maxHealth;
    }

    public virtual void TakeDamage(int damage, int proj)
    {
        health -= damage;

        if (health <= 0)
            switch (proj)
            {
                case 0:
                    anim.SetTrigger("PuffDeath");
                    Destroy(transform.parent.gameObject, 0.3f);
                    break;
                case 1:
                    anim.SetTrigger("StarDeath");
                    Destroy(transform.parent.gameObject, 0.333f);
                    break;

            }
    }

}
