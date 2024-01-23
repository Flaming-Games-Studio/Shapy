using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    public Transform topLeftRaycaster;
    public Transform bottomRightRaycaster;

    void Start()
    {
        topLeftRaycaster.position =
        // Find the raycasters by name or use public fields to assign them in the Inspector
        topLeftRaycaster.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        bottomRightRaycaster.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
    }

    void Update()
    {
        CheckRaycasters();
        // Other logic for the ball, if needed
    }

    void CheckRaycasters()
    {
        float horizontalDistance = Vector2.Distance((Vector2)Camera.main.ScreenToWorldPoint(new Vector2(topLeftRaycaster.position.x, topLeftRaycaster.position.y)), (Vector2)Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, topLeftRaycaster.position.y)));
        float verticalDistance = Vector2.Distance((Vector2)Camera.main.ScreenToWorldPoint(new Vector2(bottomRightRaycaster.position.x, bottomRightRaycaster.position.y)), (Vector2)Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, topLeftRaycaster.position.y)));
        print("Horizontal lenght = " + horizontalDistance.ToString("F2") + " and Vertical lenght " + verticalDistance.ToString("F2"));
        // Raycast from TopLeftRaycaster
        RaycastHit2D hitTopLeftDown = Physics2D.Raycast(topLeftRaycaster.position, Vector2.down, verticalDistance);
        RaycastHit2D hitTopLeftRight = Physics2D.Raycast(topLeftRaycaster.position, Vector2.right, horizontalDistance);

        // Raycast from BottomRightRaycaster
        RaycastHit2D hitBottomRightUp = Physics2D.Raycast(bottomRightRaycaster.position, Vector2.up, verticalDistance);
        RaycastHit2D hitBottomRightLeft = Physics2D.Raycast(bottomRightRaycaster.position, Vector2.left, horizontalDistance);

        // Check if the raycasts hit the ball
        if (hitTopLeftDown.collider != null)
        {
            // Handle actions for the ball being detected by the top-left raycaster
            Debug.Log("Ball detected by top-left raycaster!");
            hitTopLeftDown.collider.GetComponent<BallController>().Bounce(Vector2.zero, BallController.Direction.Right_Left);
        }
        if (hitTopLeftRight.collider != null)
        {
            // Handle actions for the ball being detected by the top-left raycaster
            Debug.Log("Ball detected by top-left raycaster!");
            hitTopLeftRight.collider.GetComponent<BallController>().Bounce(Vector2.zero, BallController.Direction.Up_Down);
        }

        if (hitBottomRightUp.collider != null)
        {
            // Handle actions for the ball being detected by the bottom-right raycaster
            Debug.Log("Ball detected by bottom-right raycaster!");
            hitBottomRightUp.collider.GetComponent<BallController>().Bounce(Vector2.zero, BallController.Direction.Right_Left);
        }
        if (hitBottomRightLeft.collider != null)
        {
            // Handle actions for the ball being detected by the bottom-right raycaster
            Debug.Log("Ball detected by bottom-right raycaster!");
            hitBottomRightLeft.collider.GetComponent<BallController>().Bounce(Vector2.zero, BallController.Direction.Up_Down);
        }
    }
}
