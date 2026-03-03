using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private string targetTag2;
    [SerializeField] private GameObject HitPart;
    [SerializeField] private int bulletDamage;
    [SerializeField] private int bulletSpeed;
    private GameObject shooter;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        bullet bull = GetComponent<bullet>();
        bull.setDamage(bulletDamage);
        //setShooter(this.gameObject);
        Vector3 moveDirection = new Vector3(0, 0, bulletSpeed);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
        Destroy(this, 2f);
    }

    private void Update()
    {
        Destroy(this.gameObject, 1f);
    }

    public void setDamage(int damage)
    {
        bulletDamage = damage;
    }

    public void setShooter(GameObject shoot)
    {
        shooter = shoot;
    }

    public GameObject getShooter()
    {
        return shooter;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != null)
        {
            Debug.Log(collision.transform.tag);
            if (collision.transform.CompareTag(targetTag) && targetTag == "Enemy")
            {
                //other.gameObject.GetComponent<Script>().TakeDamage(bulletDamage);
                //other.gameObject.transform.SetPositionAndRotation(new Vector3(1000, 1000, 1000), new Quaternion(0, 0, 0, 0));
                collision.transform.gameObject.GetComponent<AIController>().onDamage(bulletDamage, shooter);
                //Instantiate(HitPart, collision.contacts[0].point, collision.transform.rotation);
                Destroy(gameObject);
            }

            if (collision.transform.gameObject != shooter)
            {
                if (collision.transform.CompareTag(targetTag2) && targetTag2 == "Player")
                {
                    Debug.Log("Taking dmg");
                    collision.transform.gameObject.GetComponent<MultiplayerMove>().TakeDamage(bulletDamage);
                    Instantiate(HitPart, collision.contacts[0].point, collision.transform.rotation);
                    Destroy(gameObject);
                }
            }

            if (collision.transform.CompareTag("Aster"))
            {
                //make gameobject for particles
                
                Destroy(gameObject);
                Instantiate(HitPart, collision.contacts[0].point, collision.transform.rotation);
                //Debug.Log("Hit: Asteroid");
            }

        }
    }
}
