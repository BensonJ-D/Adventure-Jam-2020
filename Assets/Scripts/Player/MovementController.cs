using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MovementController : MonoBehaviour
{
    enum Actions {IDLE, WALKING, JUMPING, DOUBLE_JUMPING};
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer renderer;
    
    [SerializeField] private Rect groundCollider;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;

    Actions action = Actions.IDLE;

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        this.Move();
        this.Animate();
        this.SetFacing();
    }

    void Animate() {
        animator.SetBool("Grounded",        isTouchingGround());
        animator.SetBool("Idle",            action == Actions.IDLE);
        animator.SetBool("Walking",         action == Actions.WALKING);
        animator.SetBool("Jumping",         action == Actions.JUMPING);
        animator.SetBool("Double Jumping",  action == Actions.DOUBLE_JUMPING);
    }

    void Move()
    {
        bool grounded = isTouchingGround();
        float horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveSpeed * horizontal, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(grounded) {
                action = Actions.JUMPING;
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            } else if(action != Actions.DOUBLE_JUMPING) {
                action = Actions.DOUBLE_JUMPING;
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }
        } else if(grounded) {
            if(horizontal != 0){
                action = Actions.WALKING;
            } else {
                action = Actions.IDLE;
            }
        } else if(action != Actions.DOUBLE_JUMPING) {
            action = Actions.JUMPING;
        }
    }

    void SetFacing() {
        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal > 0){
            renderer.flipX = false;
        }
        else if(horizontal < 0) {
            renderer.flipX = true;
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

    void OnGUI() 
    {
        GUI.Label(new Rect(10, 10, 200, 200), "State: " + this.action);
        GUI.Label(new Rect(10, 30, 200, 200), "Grounded: " + isTouchingGround()); 
    }
     
    void DrawRect(Rect rect)
    {
        Vector2 myPos = transform.position;
        Gizmos.DrawWireCube(myPos + rect.center, rect.size);
    }

    Vector3 ScreenToWorld(float x, float y) {
        Camera camera = Camera.current;
        Vector3 s = camera.WorldToScreenPoint(new Vector3(x, y, 0));
        return camera.ScreenToWorldPoint(s);
    }
}
