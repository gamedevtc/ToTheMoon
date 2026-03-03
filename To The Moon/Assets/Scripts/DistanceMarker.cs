using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DistanceMarker : MonoBehaviour
{
    [SerializeField] private Text distanceText;
    [SerializeField] GameObject pMan;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, pMan.transform.position);
        if (distanceToPlayer < 600)
        {
            distanceText.text = (distanceToPlayer / 1.5f) + "KM";
        }
        if (transform.gameObject.CompareTag("Enemy"))
            transform.LookAt(Camera.main.transform);
    }
}
