using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraSpawning : MonoBehaviour
{
    [SerializeField] float distanceFromCamera = 400;
    [SerializeField] LayerMask SpawnAvoidLayer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 newPos = Vector3.zero;
            if (getOnScreenPoint(out newPos))
            {
                transform.position = newPos;
            }
        }
    }

    bool getOnScreenPoint(out Vector3 newPos)
    {
        Vector3 screenPosition = 
            Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), 
            Random.Range(0, Screen.height), Camera.main.nearClipPlane + distanceFromCamera));
        if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), distanceFromCamera, SpawnAvoidLayer))
        {
            newPos = Vector3.zero;
            return false;
        }
        newPos = screenPosition;
        return true;
    }
}
