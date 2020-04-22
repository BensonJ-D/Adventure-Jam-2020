using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private Rect groundCollider;
    [SerializeField] private float maxVelocity;

    bool jump = false;
    bool doubleJump = false;

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody2D>();
    }

    // private void FixedUpdate() {
    //     rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    // }
    // Update is called once per frame
    void Update()
    {
        this.HorizontalMovement();
        bool grounded = isTouchingGround();
        
        if(grounded) 
        {
            doubleJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.Jump(grounded);
        }
        Debug.Log("Grounded: " + grounded);
        Debug.Log("Double Jump: " + doubleJump);
    }

    void HorizontalMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(3.0f * horizontal, rb.velocity.y);
    }

    void Jump(bool grounded)
    {
        if(grounded) {
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        } else if(doubleJump) {
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
            doubleJump = false;
        }
    }  
    
    bool isTouchingGround() {
        Vector2 myPos = transform.position;
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("ground");
        filter.useLayerMask = true;

        int collisions = Physics2D.OverlapArea(myPos + groundCollider.position, myPos + groundCollider.position + groundCollider.size, filter, results);
        return collisions > 0 ? true : false;
    }

    void OnDrawGizmos()
    {
        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        DrawRect(groundCollider);
    }
     
    void OnDrawGizmosSelected()
    {
        // Orange
        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
        DrawRect(groundCollider);
    }
     
    void DrawRect(Rect rect)
    {
        Vector2 myPos = transform.position;
        Gizmos.DrawWireCube(myPos + rect.center, rect.size);
    }
}
