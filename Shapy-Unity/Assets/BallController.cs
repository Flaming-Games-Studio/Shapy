using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public enum Direction
    {
        None,
        Up_Down,
        Right_Left
    }

    public float speed = 5f; // Adjust the speed as needed
    public int attackPower = 1;

    public float width;
    public float height;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Random.insideUnitCircle.normalized * speed; // Set initial random velocity
    }

    //TODO rjesi duplo udaranje u koliderima

    //void Update()
    //{
    //    CheckScreenEdges();
    //    //// Handle collisions with walls
    //    //if (transform.position.x < -width || transform.position.x > width)
    //    //{
    //    //    // Reverse the horizontal velocity to bounce off the walls
    //    //    rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
    //    //}

    //    //if (transform.position.y < -height || transform.position.y > height)
    //    //{
    //    //    // Reverse the vertical velocity to bounce off the walls
    //    //    rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
    //    //}
    //}

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions with other colliders (blocks, enemies, etc.)
        if (collision.gameObject.CompareTag("Block"))
        {
            BlockController block = collision.gameObject.GetComponent<BlockController>();

            // Calculate the reflection direction based on the collision normal
            Vector2 reflection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);

            Bounce(reflection, Direction.None);
            //// Update the ball's velocity with the reflection direction
            //rb.velocity = reflection * speed;

            // Check if the block's HP is less than the ball's attack power
            if (block != null && block.GetBlockHP() < attackPower)
            {
                // Destroy the block if it has less HP than the ball's attack power
                Destroy(collision.gameObject);
            }
            else if (block != null)
            {
                // Reduce the block's HP if it has more HP than the ball's attack power
                block.ReduceHP(attackPower);
            }
        }
        else
        {
            // Calculate the reflection direction based on the collision normal
            Vector2 reflection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);

            Bounce(reflection, Direction.None);
            //// Update the ball's velocity with the reflection direction
            //rb.velocity = reflection * speed;
        }
    }

    //void CheckScreenEdges()
    //{
    //    //Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

    //    //// Check for collisions with screen edges and reflect the ball's velocity
    //    //if (screenPosition.x < 0f || screenPosition.x > Screen.width)
    //    //{
    //    //    Bounce(Vector2.zero, Direction.Right_Left);
    //    //    //// Reverse the horizontal velocity to bounce off the left or right edge
    //    //    //rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
    //    //}

    //    //if (screenPosition.y < 0f || screenPosition.y > Screen.height)
    //    //{
    //    //    Bounce(Vector2.zero, Direction.Up_Down);
    //    //    //// Reverse the vertical velocity to bounce off the bottom or top edge
    //    //    //rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
    //    //}
    //}

    public void Bounce(Vector2 dir, Direction d)
    {
        if (dir == Vector2.zero)
        {
            if (d == Direction.None) return;

            if (d == Direction.Up_Down)
            {
                //up-down
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
            
            if(d == Direction.Up_Down)
            {
                //right-left
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }
        else
        {
            if (d == Direction.None) rb.velocity = dir * speed; ;

            if (d == Direction.Up_Down)
            {
                //up-down
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }

            if (d == Direction.Up_Down)
            {
                //right-left
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }
    }
}

