using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public event Action OnPrimaryProjSpawn;
    public event Action OnSecondaryProjSpawn;

    [SerializeField] protected float projectileForceX;
    [SerializeField] protected float projectileForceY;

    [SerializeField] Transform spawnPointR;
    [SerializeField] Transform spawnPointL;
    SpriteRenderer sr;

    [SerializeField] protected GameObject primaryProjectilePrefab;
    [SerializeField] GameObject secondaryProjectilePrefab;

    // Start is called before the first frame update
    public virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (projectileForceX <= 0) projectileForceX = 7.0f;

        if (!spawnPointR || !spawnPointL || !primaryProjectilePrefab || !secondaryProjectilePrefab)
            Debug.Log("Set the default values on " + gameObject.name);
    }

    // Update is called once per frame
    public virtual void Fire(int proj = 0, bool angled = false, bool up = false, bool flip = false)
    {
        GameObject curProjectile;
        switch (proj)
        {
            case 0:
                if (!sr.flipX)
                {
                    curProjectile = Instantiate(primaryProjectilePrefab, spawnPointR.position, spawnPointR.rotation);
                    curProjectile.GetComponent<Projectile>().xSpeed = projectileForceX;
                    curProjectile.GetComponent<Projectile>().ySpeed = 0;
                }
                else
                {
                    curProjectile = Instantiate(primaryProjectilePrefab, spawnPointL.position, spawnPointL.rotation);
                    curProjectile.GetComponent<SpriteRenderer>().flipX = true;
                    curProjectile.GetComponent<Projectile>().xSpeed = -projectileForceX;
                    curProjectile.GetComponent<Projectile>().ySpeed = 0;
                }
                OnPrimaryProjSpawn?.Invoke();
                break;
            case 1:
                if (!sr.flipX)
                {
                    curProjectile = Instantiate(secondaryProjectilePrefab, spawnPointR.position, spawnPointR.rotation);
                    curProjectile.GetComponent<Projectile>().xSpeed = projectileForceX;
                    curProjectile.GetComponent<Projectile>().ySpeed = 0;
                }
                else
                {
                    curProjectile = Instantiate(secondaryProjectilePrefab, spawnPointL.position, spawnPointL.rotation);
                    curProjectile.GetComponent<SpriteRenderer>().flipX = true;
                    curProjectile.GetComponent<Projectile>().xSpeed = -projectileForceX;
                    curProjectile.GetComponent<Projectile>().ySpeed = 0;
                }
                OnSecondaryProjSpawn?.Invoke();
                break;
        }
        
    }
}
