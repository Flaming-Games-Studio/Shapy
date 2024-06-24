using UnityEngine;

public class GearPuzzleGear : MonoBehaviour
{
    public float spinSpeed = 100f;
    [HideInInspector]
    public bool isSpinning = false;
    [HideInInspector]
    public bool spinLeft;


    void Update()
    {
        if (isSpinning)
        {
            float spinDir = 0;
            if (spinLeft)
                spinDir = -1;
            else
                spinDir = 1;


            transform.Rotate(Vector3.forward, spinSpeed * spinDir * Time.deltaTime);
        }
    }

    public void StartSpinning(bool spinL)
    {
        isSpinning = true;
        spinLeft = spinL;
    }

    public void StopSpinning()
    {
        isSpinning = false;
    }
}
