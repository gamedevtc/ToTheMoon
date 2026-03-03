using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BodyLink : MonoBehaviour
{
    [SerializeField] List<Body> bodies;
    [SerializeField] List<Body> Holobodies;
    [SerializeField] Ship.BodyOption activeBody;

    [SerializeField] Ship shipMain;

    private void Awake()
    {
     //   shipMain = GetComponentInParent<Ship>();
    }

    public void setActiveBody(Ship.BodyOption newBody)
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            if (bodies[i].gameObject.activeInHierarchy)
            {
                bodies[i].gameObject.SetActive(false);
            }
        }
        
        activeBody = newBody;
        bodies[(int)activeBody - (int)Ship.ColorOption.COUNT].gameObject.SetActive(true);
    }
    public void setHoloBody(Ship.BodyOption newBody)
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            if (bodies[i].gameObject.activeInHierarchy)
            {
                bodies[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < Holobodies.Count; i++)
        {
            if (Holobodies[i].gameObject.activeInHierarchy)
            {
                Holobodies[i].gameObject.SetActive(false);
            }
        }
        activeBody = newBody;
        Holobodies[(int)activeBody - (int)Ship.ColorOption.COUNT].gameObject.SetActive(true);
    }
    public void HoloOff()
    {
        for (int i = 0; i < Holobodies.Count; i++)
        {
            if (Holobodies[i].gameObject.activeInHierarchy)
            {
                Holobodies[i].gameObject.SetActive(false);
            }
        }
    }
    public Ship getShipMain()
    {
        return shipMain;
    }

    public BodyStats getStats()
    {
        return bodies[(int)activeBody - (int)Ship.ColorOption.COUNT].stats;
    }

    public Mesh getMesh()
    {
        return bodies[(int)activeBody - (int)Ship.ColorOption.COUNT].shipMesh;
    }

    public void setColor(Material mat)
    {
        bodies[(int)activeBody - (int)Ship.ColorOption.COUNT].setColor(mat);
    }
    
}
