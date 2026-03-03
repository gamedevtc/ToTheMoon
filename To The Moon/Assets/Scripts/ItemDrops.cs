using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrops : MonoBehaviour
{
    [SerializeField] private GameObject pMan;
    [SerializeField] private Player4Base pScript;
    [SerializeField] private float distToPlayer;
    [SerializeField] private bool isInfiniteBoost;
    [SerializeField] private bool isRapidBoost;
    [SerializeField] private bool isUltimateBoost;
    [SerializeField] private bool isGodBoost;

    private int randx;
    private int randy;
    private int randz;
    private float timer;
    private bool timerBool;
    
    // Start is called before the first frame update
    void Start()
    {
        pMan = GameObject.FindGameObjectWithTag("Player");
        pScript = pMan.GetComponent<Player4Base>();
        randx = UnityEngine.Random.Range(-150, 150);
        randy = UnityEngine.Random.Range(-150, 150);
        randz = UnityEngine.Random.Range(-150, 150);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(randx * Time.deltaTime, randy * Time.deltaTime, randz * Time.deltaTime);
        float distanceToPlayer = Vector3.Distance(transform.position, pMan.transform.position);
        distToPlayer = distanceToPlayer;
        if (distanceToPlayer < 400)
        {
            transform.position = Vector3.MoveTowards(transform.position, pMan.transform.position, 100 * Time.deltaTime);
        }
        if (timerBool)
        {
            if (isInfiniteBoost)
            {
                pScript.swapInfiniteBoost();

            }
            if (isGodBoost)
            {
                pScript.swapGodBoost();

            }
            if (isRapidBoost)
            {

            }
            timer -= Time.deltaTime;
        }
        if (timer <= 0 && timerBool)
        {
            if (isInfiniteBoost)
            {
                pScript.swapInfiniteBoost();

            }
            if (isInfiniteBoost)
            {
                pScript.swapGodBoost();

            }
            if (isRapidBoost)
            {

            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Player"))
        {
            
            if (isInfiniteBoost)
            {
                timer = 10.0f;
                timerBool = true;

            }
            if (isGodBoost)
            {
                timer = 10.0f;
                timerBool = true;

            }
            if (isRapidBoost)
            {
                timer = 15.0f;
                timerBool = true;
            }
            if (isUltimateBoost)
            {
                timer = 0;
                timerBool = true;
                
            }
            gameObject.SetActive(false);
            
            
        }
    }
}
