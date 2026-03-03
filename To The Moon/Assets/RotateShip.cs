using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateShip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] GameObject[] Rotating;
    [SerializeField] float speed;

    [SerializeField] bool Left;
    bool ispressed = false;

    Quaternion q;
    void Start()
    {
        q = Rotating[0].transform.rotation;
    }
    

    void Update()
    {
        if (!ispressed)
            return;
        if (Left)
        {
            left();
        }
        else
        {
            right();
        }
       
      
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        ispressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
    }


    public void ResetRotations()
    {
        StartCoroutine(reset());
    }

    IEnumerator reset()
    {
        while (Rotating[0].transform.rotation.y != q.y)
        {
            if (Rotating[0].transform.rotation.y > q.y)
            {
                foreach (GameObject item in Rotating)
                {
                   
                    item.transform.Rotate(0, -speed, 0);
                }
                if (Rotating[0].transform.rotation.y < q.y)
                {
                    break;
                }
                yield return new WaitForSeconds(.00000f);
            }
            else
            {
                foreach (GameObject item in Rotating)
                {
                    item.transform.Rotate(0, speed, 0);
                }
                if (Rotating[0].transform.rotation.y > q.y)
                {
                    break;
                }
                yield return new WaitForSeconds(.00000f);

            }

           

        }
    }


    public void left()
    {
        foreach (GameObject item in Rotating)
        {
            item.transform.Rotate(0, speed, 0);
        }
    }

    void right()
    {
        foreach (GameObject item in Rotating)
        {
            item.transform.Rotate(0, -speed, 0);
        }
    }

}
