using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public enum ProjectileType
    {
        puff = 0,
        star = 1,
        cannonBall = 2,
    }

    [SerializeField] ProjectileType currentProjectile;
    [SerializeField] int damage;
    [SerializeField] float lifeTime;

    [HideInInspector]
    public float xSpeed;
    [HideInInspector]
    public float ySpeed;
    public bool isImpulse;

    // Start is called before the first frame update
    void Start()
    {
        if (lifeTime <= 0) lifeTime = 1.0f;
        if (damage <= 0) damage = 1;

        if (isImpulse)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(xSpeed, ySpeed), ForceMode2D.Impulse);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(xSpeed, ySpeed);
        }
        
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Level"))
        {
            Destroy(gameObject);
        }

        else if (collision.CompareTag("Enemy") && (gameObject.CompareTag("PuffProjectile") || gameObject.CompareTag("StarProjectile")) )
        {
            switch (currentProjectile)
            {
                case ProjectileType.puff:
                    collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, 0);
                    break;
                case ProjectileType.star:
                    collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, 1);
                    break;
            }
            Debug.Log("Enemy was hit");
            Destroy(gameObject);
        }

        else if (collision.CompareTag("Player") && gameObject.CompareTag("EnemyProjectile"))
        {
            if (!collision.GetComponent<PlayerController>().GetInvincibilityState())
            {
                Destroy(gameObject);
                GameManager.Instance.currentHP--;
            }
        }
    }


}
