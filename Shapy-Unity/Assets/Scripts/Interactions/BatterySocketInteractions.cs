using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocketInteractions : InteractableObject
{
    private Animator anim;
    public Animator frontDoorsAnim;
    private GameObject batteryVisualGameobject;
    BatteryStatus currentBattery;
    public bool nonRemovableBattery;
    //public BoxCollider doorCollider;


    private void Start()
    {
        base.Start();
        GetUnitAnimators();
    }
    public override void Interact()
    {
        if (currentBattery == null)
        {
            print("Placing battery into power socket!");
            currentBattery = InventoryManager.Instance.ReturnFirstAvailableBattery();
            if (currentBattery != null)
            {
                anim.SetTrigger("LoadBattery");
                currentBattery.gameObject.transform.SetParent(transform.GetChild(0));
                currentBattery.transform.localPosition = Vector3.zero;
                currentBattery.transform.localRotation = Quaternion.identity;
                currentBattery.gameObject.SetActive(true);
                //doorCollider.enabled = false;
                //frontDoorsAnim.gameObject.GetComponent<BoxCollider>().enabled = false;
                frontDoorsAnim.GetComponent<DoorInteractions>().Interact();
                frontDoorsAnim.GetComponent<SphereCollider>().enabled = true;
                anim.SetBool("DoorOpen", true);
                currentBattery.transform.GetChild(0).gameObject.SetActive(true);
                currentBattery.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                print("Current battery not referenced?");
            }
        }
        else
        {
            if (nonRemovableBattery)
            {
                col.enabled = false;
                this.enabled = false;
                return;
            }
            print("Tooking :-) battery from power socket!");
            InventoryManager.Instance.AddNewBatteryToInventory(currentBattery);
            if (currentBattery != null)
            {
                anim.SetTrigger("UnloadBattery");
                currentBattery.gameObject.transform.SetParent(null);
                currentBattery.gameObject.SetActive(false);
                //doorCollider.enabled = true;
                //frontDoorsAnim.gameObject.GetComponent<BoxCollider>().enabled = true;
                frontDoorsAnim.GetComponent<DoorInteractions>().Interact();
                anim.SetBool("DoorOpen", false);
                currentBattery.transform.GetChild(0).gameObject.SetActive(false);
                currentBattery.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                print("Current battery not referenced?");
            }
            currentBattery = null;
        }

    }
    private void GetUnitAnimators()
    {
        anim = GetComponent<Animator>();
    }

}
