using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] float projectileForce;

    [SerializeField] Transform spawnPointR;
    [SerializeField] Transform spawnPointL;
    SpriteRenderer sr;

    [SerializeField] GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (projectileForce <= 0) projectileForce = 7.0f;

        if (!spawnPointR || !spawnPointL || !projectilePrefab)
            Debug.Log("Set the default values on " + gameObject.name);
    }

    // Update is called once per frame
    public void Fire()
    {
        if (!sr.flipX)
        {
            GameObject curProjectile = Instantiate(projectilePrefab, spawnPointR.position, spawnPointR.rotation);
            curProjectile.GetComponent<Projectile>().speed = projectileForce;
        }
        else
        {
            GameObject curProjectile = Instantiate(projectilePrefab, spawnPointL.position, spawnPointL.rotation);
            curProjectile.GetComponent<SpriteRenderer>().flipX = true;
            curProjectile.GetComponent<Projectile>().speed = -projectileForce;
        }
    }
}
