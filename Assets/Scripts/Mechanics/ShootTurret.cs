using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTurret : Shoot
{
    public event Action OnProjSpawn;

    [SerializeField] Transform spawnPointR45;
    [SerializeField] Transform spawnPointL45;
    [SerializeField] Transform spawnPointUp;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        if (!spawnPointR45 || !spawnPointL45 || !spawnPointUp)
            Debug.Log("Set the default values on " + gameObject.name);
    }

    public override void Fire(int proj = 0, bool angled = false, bool up = false, bool flip = false)
    {
        if (!angled && !up)
            base.Fire();
        else if (angled && !flip)
        {
            GameObject currProjectile =  Instantiate(primaryProjectilePrefab, spawnPointR45.position, spawnPointR45.rotation);
            currProjectile.GetComponent<Projectile>().xSpeed = projectileForceX / Mathf.Sqrt(2);
            currProjectile.GetComponent<Projectile>().ySpeed = projectileForceY / Mathf.Sqrt(2);
            OnProjSpawn?.Invoke();
        }
        else if (angled && flip)
        {
            GameObject currProjectile = Instantiate(primaryProjectilePrefab, spawnPointL45.position, spawnPointL45.rotation);
            currProjectile.GetComponent<Projectile>().xSpeed = -projectileForceX / Mathf.Sqrt(2);
            currProjectile.GetComponent<Projectile>().ySpeed = projectileForceY / Mathf.Sqrt(2);
            OnProjSpawn?.Invoke();
        }
        else if (up)
        {
            GameObject currProjectile = Instantiate(primaryProjectilePrefab, spawnPointUp.position, spawnPointUp.rotation);
            currProjectile.GetComponent<Projectile>().xSpeed = 0;
            currProjectile.GetComponent<Projectile>().ySpeed = projectileForceY;
            OnProjSpawn?.Invoke();
        }
            
    }

}
