using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpShip : Ship
{
    protected override void Awake()
    {
        
    }

    protected override void Start()
    {
        
    }

    public void setShip(playerShipSettings set)
    {
        base.activateGameParts(set);
    }
}
