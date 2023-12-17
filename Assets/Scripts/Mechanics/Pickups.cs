using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum PickupType
    {
        lives = 0,
        hpFull = 1,
        hpPortion = 2,
        invincible = 3,
    }

    [SerializeField] PickupType currentPickup;
    [SerializeField] float invincibilityDuration = 5.0f;
    [SerializeField] AudioClip pickUpSound;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.PlayPickupSound(pickUpSound);
            switch (currentPickup)
            {
                case PickupType.lives:
                    GameManager.Instance.currentLives++;
                    break;
                case PickupType.hpFull:
                    GameManager.Instance.currentHP += 6;
                    break;
                case PickupType.hpPortion:
                    GameManager.Instance.currentHP += 2;
                    break;
                case PickupType.invincible:
                    pc.StartInvincibilityChange(invincibilityDuration);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
