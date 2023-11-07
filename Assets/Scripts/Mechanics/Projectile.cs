using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime;

    [HideInInspector]
    public float speed;
    public bool isImpulse;

    // Start is called before the first frame update
    void Start()
    {
        if (lifeTime <= 0) lifeTime = 1.0f;

        if (isImpulse)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(speed, 0), ForceMode2D.Impulse);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
        }
        
        Destroy(gameObject, lifeTime);
    }


}
