using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Variables")]
    [SerializeField] private int damage = 100;
    [SerializeField] private float delay = 5f;
    [SerializeField] private float countdown = 0;
    [SerializeField] private float blastRadius = 50.0f;
    [SerializeField] private bool hasExploded = false;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float bombSpeed = -5;
    // Start is called before the first frame update
    void Start()
    {
        Bomb bom = GetComponent<Bomb>();
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 moveDirection = new Vector3(0, 0, bombSpeed);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.AddForce(moveDirection);
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0.0f && hasExploded == false)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        //show effects
        GameObject temp = Instantiate(explosionEffect, transform.position, transform.rotation);

        //get nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObj in colliders)
        {
            if (nearbyObj.CompareTag("Enemy"))
            {
                nearbyObj.GetComponent<AIController>().onDamage(damage, this.gameObject);
            }

            if (nearbyObj.CompareTag("Player"))
            {
                nearbyObj.GetComponent<Player3>().TakeDamage(damage);
            }
        }

        Destroy(temp, 2.5f);
        Destroy(this.gameObject);
    }

    public void setDamage(int dam)
    {
        damage = dam;
    }

}
