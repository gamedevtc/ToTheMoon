using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBlastTest : MonoBehaviour
{
    [SerializeField] GameObject DeathEffect;
    [SerializeField] List<GameObject> coinPrefabs;
    [SerializeField] float coinInitialVelocity = 15;
    [SerializeField] float coinBlastDirectionStrength = 5;

    public void coinBlast()
    {
        Instantiate(DeathEffect, this.transform.position, this.transform.rotation);

        int coinCount = Random.Range(1, 6);
        for (int i = 0; i < coinCount; i++)
        {
            int coinIndex = Random.Range(0, coinPrefabs.Count);
            Vector3 Direction = new Vector3(Random.Range(-coinBlastDirectionStrength, coinBlastDirectionStrength), Random.Range(-coinBlastDirectionStrength, coinBlastDirectionStrength), Random.Range(-coinBlastDirectionStrength, coinBlastDirectionStrength));
            Direction *= coinInitialVelocity;
            GameObject coin = Instantiate(coinPrefabs[coinIndex], transform.position, transform.rotation);
            coin.GetComponent<Rigidbody>().velocity = Direction;
        }
    }
}
