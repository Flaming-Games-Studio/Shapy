using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//rename expected
public class BatteryAnimationState : MonoBehaviour
{
    private BatteryChargerInteractions BCI;
    private int index;
    public Animator anim;
    public bool occupied;
    public BatteryStatus currentBattery;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetBCI(BatteryChargerInteractions bci)
    {
        BCI = bci;
    }

    public BatteryChargerInteractions GetBCI()
    {
        return BCI;
    }

    public int GetIndex()
    {
        return index;
    }
    public void SetIndex(int ind)
    { 
        index = ind;
    }
}
