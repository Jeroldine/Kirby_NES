using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] Pickups MaxTomatoPrefab;
    [SerializeField] Pickups bottlePrefab;
    [SerializeField] Pickups lifeUpPrefab;
    [SerializeField] Pickups CandyPrefab;
    int randomPickup;


    // Start is called before the first frame update
    void Start()
    {
        randomPickup = Random.Range(0, 5);
        Debug.Log(randomPickup);
        switch (randomPickup)
        {
            case 0:
                Instantiate(MaxTomatoPrefab, transform.position, transform.rotation);
                break;
            case 1:
                Instantiate(bottlePrefab, transform.position, transform.rotation);
                break;
            case 2:
                Instantiate(lifeUpPrefab, transform.position, transform.rotation);
                break;
            case 3:
                Instantiate(CandyPrefab, transform.position, transform.rotation);
                break;
            case 4:
                break;
        }
    }


}
