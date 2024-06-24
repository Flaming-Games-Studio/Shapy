using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceDispatcherBattery : MonoBehaviour
{
    public Slider batteryChargeSlider;
    public Slider batteryUsageSlider;

    public TextMeshProUGUI batteryChargeTxt;
    public TextMeshProUGUI batteryUsageTxt;

    [Header("private")]
    public int batteryCharge;
    public int batteryUsage;
}
