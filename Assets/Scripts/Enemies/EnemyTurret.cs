using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootTurret))]
public class EnemyTurret : Enemy
{
    bool facingSide;
    bool facingUp;
    bool facingAngle;
    bool canFire;

    ShootTurret st;
    [SerializeField] float fireRate;

    float fireTimer = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        st = GetComponent<ShootTurret>();
        canFire = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canFire)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                st.Fire(0, facingAngle, facingUp, sr.flipX);
                fireTimer = 0;
            }
        }

        anim.SetBool("facingSide", facingSide);
        anim.SetBool("facingAngle", facingAngle);
        anim.SetBool("facingUp", facingUp);
    }

    public void setAnimStates(float angle) // angle in radians
    {
        angle = (angle * 180) / Mathf.PI;
        
        if ((angle > -90 && angle <= 0) || (angle > 0 && angle <= 15))
            SetFacingStates(true, false, false, false);
        else if (angle > 15 && angle <= 75)
            SetFacingStates(false, true, false, false);
        else if (angle > 75 && angle <= 105)
            SetFacingStates(false, false, true, false);
        else if (angle > 105 && angle <= 165)
            SetFacingStates(false, true, false, true);
        else if ((angle > 165 && angle <= 180) || (angle >= -180 && angle <= -90))
            SetFacingStates(true, false, false, true);
    }

    private void SetFacingStates(bool side, bool angled, bool up, bool flip)
    {
        facingSide = side;
        facingAngle = angled;
        facingUp = up;
        sr.flipX = flip;
    }

    public void SetFireState(bool fire)
    {
        canFire = fire;
    }

    public void ResetTimer()
    {
        fireTimer = 0;
    }
}
