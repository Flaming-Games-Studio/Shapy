using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BatteryMaterialSwitcher;

public class BatteryStatus : MonoBehaviour
{
    public float depleteInterval = 5f;
    public float batteryMaxChargeValue = 100;
    public float batteryCurrentChargeValue = 0;
    public bool charging;
    public bool idle;
    private BatteryMaterialSwitcher bms;
    private BatteryAnimationState BAS;
    private int index = 0;


    public void Start()
    {
        charging = false;
        idle = true;
        bms = GetComponent<BatteryMaterialSwitcher>();

        switch (bms.batteryState)
        {
            case BatteryState.Empty:
                batteryCurrentChargeValue = batteryMaxChargeValue * 0;
                break;
            case BatteryState.Inbetween:
                batteryCurrentChargeValue = batteryMaxChargeValue * Random.Range(0.1f, 0.5f);
                break;
            case BatteryState.Full:
                batteryCurrentChargeValue = batteryMaxChargeValue * Random.Range(0.51f, 1f);
                break;
            default:
                break;
        }

        StartCoroutine(BatteryStatusChanger());
    }

    public void SetChargingStationInfo(BatteryAnimationState bas, int id)
    {
        BAS = bas;
        index = id;
    }

    IEnumerator BatteryStatusChanger()
    {
        while (true) // Run indefinitely
        {
            if (!idle)
            {
                if (charging)
                {
                    if (batteryCurrentChargeValue < batteryMaxChargeValue)
                    {
                        batteryCurrentChargeValue += batteryMaxChargeValue * 0.02f;
                        print(batteryCurrentChargeValue);
                        if (batteryCurrentChargeValue > batteryMaxChargeValue)
                        {
                            BAS.anim.SetTrigger("FullBattery");
                            batteryCurrentChargeValue = batteryMaxChargeValue;
                            charging = false;
                            idle = true;
                        }
                    }
                }
                else
                {
                    if (batteryCurrentChargeValue >= 0f)
                    {
                        batteryCurrentChargeValue -= batteryMaxChargeValue * 0.01f;
                        print(batteryCurrentChargeValue);
                    }
                    else print("Battery alrdy empty!");

                }
                MaterialControl();
            }
            
            // Wait for a certain amount of time before the next iteration
            yield return new WaitForSeconds(depleteInterval); // Wait for 5 seconds before continuing
        }
    }

    private void MaterialControl()
    {
        if (batteryCurrentChargeValue > batteryMaxChargeValue * 0.8f)
        {
            bms.SwitchBatteryMaterial(BatteryState.Full);
        }
        else if (batteryCurrentChargeValue < batteryMaxChargeValue * 0.8f && batteryCurrentChargeValue > batteryMaxChargeValue * 0.2f)
        {
            bms.SwitchBatteryMaterial(BatteryState.Inbetween);
        }
        else
        {
            bms.SwitchBatteryMaterial(BatteryState.Empty);
        }
    }
}
