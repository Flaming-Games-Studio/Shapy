using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ReactionTimeTest : MonoBehaviour
{
    public Animator buttonAnim;
    public Transform dial;
    public Transform targetObject; // New transform for visual component

    private bool isSpinning = true;
    private bool isRestarted = false;
    private Coroutine spinCoroutine;

    private Quaternion currentTarget;
    private Quaternion previousTarget;
    private Quaternion stoppedRotation;
    public float rotationSpeed = 90f;
    private float startingRotationSpeed;

    [Header("Sounds")]
    private AudioSource audioS;
    public AudioClip loseClip;
    public AudioClip winClip;

    //analytics
    [Header("Analytics")]
    private DateTime lastButtonPressTime;
    public AnalyticsMethods analyticsMethods;
    public GlobalTimer timer;

    private Collider dialCol, targetCol;
    private bool canPlay;
    public GameObject instructionsCanvas;

    private void Start()
    {
        dialCol = dial.GetComponent<Collider>();
        targetCol = targetObject.GetComponent<Collider>();  
        
        //stefan rotation magic
        currentTarget = Quaternion.Euler(0, 90, 30);
        previousTarget = Quaternion.Euler(0, -90, -30);
        //

        audioS = GetComponent<AudioSource>();
        startingRotationSpeed = rotationSpeed;
        stoppedRotation = dial.localRotation; // Updated
        GetRandomizedTargetRotation(targetObject.localRotation, 20f); // Set initial random rotation for targetObject
        enabled = false;
        instructionsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        enabled = true;
        canPlay = true;
        instructionsCanvas.SetActive(true);
        //analytics
        timer = new GlobalTimer();
        spinCoroutine = StartCoroutine(SpinDial());
    }

    public void ResetGame()
    {
        isSpinning = false;
        instructionsCanvas.SetActive(false);
        canPlay = false;
        enabled = false;
    }

    private void Update()
    {
        if (!canPlay) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSpinning)
            {
                //analytics
                DateTime currentTime;
                timer.GetCurrentTimestamp(out currentTime);

                if (lastButtonPressTime.Year < 2000)
                {
                    lastButtonPressTime = currentTime;
                }
                analyticsMethods.TrackButtonPress(lastButtonPressTime, currentTime);
                lastButtonPressTime = currentTime;
                //analytics end

                StopRotation();
            }
            else
            {
                buttonAnim.SetTrigger("Depress");
                isSpinning = true;
                isRestarted = true;
                spinCoroutine = StartCoroutine(SpinDial());
            }
        }
    }

    private IEnumerator SpinDial()
    {
        float angleToRotate = Quaternion.Angle(stoppedRotation, currentTarget);

        while (true)
        {
            if (isRestarted)
            {
                // Calculate corrected duration based on remaining angle to target
                angleToRotate = Quaternion.Angle(dial.localRotation, currentTarget); // Updated
                isRestarted = false;
            }

            float step = rotationSpeed * Time.deltaTime;
            dial.localRotation = Quaternion.RotateTowards(dial.localRotation, currentTarget, step); // Updated

            // Check if the target is reached
            if (Quaternion.Angle(dial.localRotation, currentTarget) < 0.1f) // Updated
            {
                dial.localRotation = currentTarget; // Updated
                stoppedRotation = currentTarget; // Updated
                Quaternion temp = previousTarget;
                previousTarget = currentTarget;
                currentTarget = temp;
                angleToRotate = Quaternion.Angle(dial.localRotation, currentTarget); // Updated
            }

            yield return null;

            if (!isSpinning)
            {
                stoppedRotation = dial.localRotation; // Updated
                CheckIfInGreenZone();
                yield break;
            }
        }
    }

    public void StopRotation()
    {
        buttonAnim.SetTrigger("Press");
        isSpinning = false;
    }

    private void CheckIfInGreenZone()
    {
        
        if (AreCollidersTouching(dialCol, targetCol))
        {
            Debug.Log("Landed in Green Zone");
            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.gameObject);
            audioS.clip = winClip;
            audioS.Play();

            if (rotationSpeed < startingRotationSpeed * 5)
            {
                rotationSpeed += 10f;
            }
            // Randomize the targetObject rotation for the next round
            canPlay = false;
            Invoke(nameof(SpinNeedleAfterHit), 1f);
            //analytics
            analyticsMethods.ReactionTimePuzzleFinished(true);
        }
        else
        {
            Debug.Log("Missed Green Zone");
            audioS.clip = loseClip;
            audioS.Play();
            if (rotationSpeed > startingRotationSpeed)
            {
                rotationSpeed -= 10f;
            }
            //analytics
            analyticsMethods.ReactionTimePuzzleFailed(true);
        }
    }

    public void SpinNeedleAfterHit()
    {
        GetRandomizedTargetRotation(targetObject.localRotation, 20f);
    }

    private void GetRandomizedTargetRotation(Quaternion oldRotation, float minDifference = 10f)
    {
        // Generate random rotation within specified ranges
        System.Random rand = new System.Random();
        Quaternion newRotation;
        float randomY = rand.Next(-80, 81); // Random Y rotation between -90 and 90 degrees
        //float randomZ = Mathf.Lerp(-30f, 30f, (randomY + 90f) / 180f); // Interpolates Z based on Y to follow same pattern
        //float randomX = 0f;

        //randomY = rand.Next(-80, 81); // Random Y rotation between -80 and 80 degrees
        //randomX = (randomY < 0) ? Mathf.Lerp(0f, 30f, (randomY + 90f) / 180f) : Mathf.Lerp(30f, 0f, (randomY + 90f) / 180f);
        //if (randomY < 0)
        //{
        //    randomX = Mathf.Lerp(0f, 30f, (randomY + 90f) / 180f); // Interpolates X based on Y to follow same pattern
        //}
        //else
        //{
        //    randomX = Mathf.Lerp(30f, 0f, (randomY + 90f) / 180f); // Interpolates X based on Y to follow same pattern
        //}
        newRotation = Quaternion.Euler(0, randomY, 0);
        if(Quaternion.Angle(oldRotation, newRotation) < minDifference)
        {
            print("Getting new rotation too close");
            GetRandomizedTargetRotation(oldRotation, minDifference);
            return;
        }
        else
        {
            targetObject.localRotation = newRotation;
            canPlay = true;
        }
        
    }

    bool AreCollidersTouching(Collider col1, Collider col2)
    {
        return col1.bounds.Intersects(col2.bounds);
    }
    private Quaternion GetRandomizedTargetRotation()
    {
        // Generate random rotation within specified ranges
        System.Random rand = new System.Random();
        float randomY = rand.Next(-80, 81); // Random Y rotation between -90 and 90 degrees
        float randomZ = Mathf.Lerp(-30f, 30f, (randomY + 90f) / 180f); // Interpolates Z based on Y to follow same pattern
        float randomX;
        if (randomY < 0)
        {
            randomX = Mathf.Lerp(0f, 30f, (randomY + 90f) / 180f); // Interpolates X based on Y to follow same pattern
        }
        else
        {
            randomX = Mathf.Lerp(30f, 0f, (randomY + 90f) / 180f); // Interpolates X based on Y to follow same pattern
        }

        return Quaternion.Euler(0, randomY, 0);
    }
}

