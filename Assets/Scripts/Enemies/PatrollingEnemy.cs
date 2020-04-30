using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    public float speed;
    private float direction;
    private Vector2 raycastPosition;
    public RaycastHit2D raycastHitInfo;
    Rigidbody2D rigidBody2D;
    float turnTimer; 

    // Start is called before the first frame update
    void Start()
    {
        turnTimer = 0.1f; 
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        turnTimer -= Time.deltaTime; 
        raycastHitInfo = Physics2D.Raycast(raycastPosition, Vector2.down, 1f);
        direction = Mathf.Sign(speed);
        raycastPosition = new Vector2(transform.position.x + 0.55f * direction, transform.position.y + -0.2f);

        if (raycastHitInfo.collider == null && turnTimer <= 0f)
        {
            turnTimer = 0.1f; 
            speed *= -1;
            transform.localScale = new Vector2(Mathf.Sign(speed), 1);
        }
        else
        {
            Debug.Log("raycast hit:" + raycastHitInfo.collider);
        }
        rigidBody2D.velocity = new Vector2(speed, rigidBody2D.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(raycastPosition, Vector2.down);
    }
}
