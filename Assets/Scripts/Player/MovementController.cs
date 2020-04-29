using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MovementController : MonoBehaviour
{
    enum Animation {IDLE, WALKING, JUMPING, SLOW_FALL, FAST_FALL, LANDING};
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer renderer;
    
    [SerializeField] private Rect groundCollider;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;

    Animation animation = Animation.IDLE;
    bool doubleJump = false;
    float dir = 0;
    Dictionary<string, string> debugLog = new Dictionary<string, string>();

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
        animator.SetBool("Idle",            animation == Animation.IDLE);
        animator.SetBool("Walking",         animation == Animation.WALKING);
        animator.SetBool("Jumping",         animation == Animation.JUMPING);
        animator.SetBool("Slow Fall",       animation == Animation.SLOW_FALL);
        animator.SetBool("Fast Fall",       animation == Animation.FAST_FALL);
    }

    void Move()
    {
        bool grounded = isTouchingGround();
        float hInput = Input.GetAxis("Horizontal");
        float oldVelocity = rb.velocity.x;
        float newVelocity = 0;
        float clampedVelocity = 0;
        if(oldVelocity != 0) dir = Mathf.Abs(oldVelocity) / oldVelocity;

        // AddDebugValue("H INPUT", hInput.ToString());
        // AddDebugValue("Old Velocity", oldVelocity.ToString());
        if(hInput != 0) {
            newVelocity = (oldVelocity) + (moveSpeed / 10f * hInput);
            clampedVelocity = Mathf.Max(Mathf.Min(newVelocity, moveSpeed), -moveSpeed);
        } else {
            newVelocity = (oldVelocity) + (moveSpeed / 10f * -dir);
            clampedVelocity = dir > 0 ? Mathf.Max(newVelocity, 0) : Mathf.Min(newVelocity, 0);
        }
        // AddDebugValue("Dir", dir.ToString());
        // AddDebugValue("New Velocity", newVelocity.ToString());
        // AddDebugValue("Clamped Velocity", clampedVelocity.ToString());

        rb.velocity = new Vector2(clampedVelocity, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(grounded) {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                grounded = false;
            } else if(!grounded && doubleJump) {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                doubleJump = false;
                grounded = false;
            }
        }
        
        if(grounded) {
            doubleJump = true;
            
            if(hInput != 0){
                animation = Animation.WALKING;
            } else {
                animation = Animation.IDLE;
            }
        }
        
        if(!grounded) {
            if(Mathf.Abs(rb.velocity.y) < 1.0f){
                animation = Animation.SLOW_FALL;
            } else if(rb.velocity.y > 0) {
                animation = Animation.JUMPING;
            } else if(rb.velocity.y < 0) {
                animation = Animation.FAST_FALL;
            }
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

    void AddDebugValue(string name, string value) 
    {
        debugLog[name] = value;
    }

    void OnGUI() 
    {
        GUI.color = Color.blue;
        GUI.Label(new Rect(10, 10, 200, 200), "State: " + this.animation);
        GUI.Label(new Rect(10, 30, 200, 200), "Grounded: " + isTouchingGround()); 
        GUI.Label(new Rect(10, 50, 200, 200), "H Speed: " + rb.velocity.x); 

        int i = 0;
        foreach (KeyValuePair<string, string> item in debugLog)
        {
            GUI.Label(new Rect(10, 70 + (20 * i), 200, 200), item.Key + ": " + item.Value);
            i++;
        }
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
