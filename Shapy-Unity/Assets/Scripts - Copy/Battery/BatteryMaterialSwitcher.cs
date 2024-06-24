using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryMaterialSwitcher : MonoBehaviour
{
    public MeshRenderer batteryCoil;
    public BatteryState batteryState;
    [Header("RED; ORANGE; GREEN;")]
    public Material[] batteryMaterials;
    private Material selectedMaterial;

    public enum BatteryState
    {
       Empty,
       Inbetween,
       Full
    }

    public void Start()
    {
        //batteryCoil = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        SwitchBatteryMaterial(batteryState);
    }

    public void SwitchBatteryMaterial(BatteryState state)
    {
        batteryState = state;
        switch (batteryState)
        {
            case BatteryState.Empty:
                selectedMaterial = batteryMaterials[0];
                break;
            case BatteryState.Inbetween:
                selectedMaterial = batteryMaterials[1];
                break;
            case BatteryState.Full:
                selectedMaterial = batteryMaterials[2];
                break;
            default:
                break;
        }
        batteryCoil.material = selectedMaterial;
    }
}
