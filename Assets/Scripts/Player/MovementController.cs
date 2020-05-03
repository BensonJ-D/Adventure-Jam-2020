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
    [SerializeField] private float sampleSpeed;
    [SerializeField] private float jumpSpeed;

    Animation animation = Animation.IDLE;
    bool doubleJump = false;
    float direction = 1;
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
        AddDebugValue("Animation State", animation.ToString());
        AddDebugValue("Grounded", isTouchingGround().ToString());
        
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
        float oldHorizontalVelocity = rb.velocity.x;
        float newHorizontalVelocity = 0;
        float clampedHorizontalVelocity = 0;
        float dx = (moveSpeed * Time.deltaTime * sampleSpeed);
        direction = hInput == 0 ? direction : Mathf.Abs(hInput) / hInput;

        if(hInput != 0) {
            newHorizontalVelocity = (oldHorizontalVelocity) + (dx  * direction);
            clampedHorizontalVelocity = Mathf.Max(Mathf.Min(newHorizontalVelocity, moveSpeed), -moveSpeed);
        } else {
            newHorizontalVelocity = (oldHorizontalVelocity) + (dx * -direction);
            clampedHorizontalVelocity = direction > 0 ? Mathf.Max(newHorizontalVelocity, 0) : Mathf.Min(newHorizontalVelocity, 0);
        }

        float clampedVerticalVelocity = Mathf.Max(-maxFallSpeed, rb.velocity.y);
        rb.velocity = new Vector2(clampedHorizontalVelocity, clampedVerticalVelocity);

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
            AddDebugValue("Absolute Y Speed", Mathf.Abs(rb.velocity.y).ToString());
            if(Mathf.Abs(rb.velocity.y) < 2.0f){
                animation = Animation.SLOW_FALL;
            } else if(rb.velocity.y > 0) {
                animation = Animation.JUMPING;
            } else if(rb.velocity.y < 0) {
                animation = Animation.FAST_FALL;
            }
        }
    }

    void SetFacing() {
        if(direction > 0){
            renderer.flipX = false;
        }
        else if(direction < 0) {
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
        int i = 0;
        foreach (KeyValuePair<string, string> item in debugLog)
        {
            GUI.Label(new Rect(10, 10 + (20 * i), 200, 200), item.Key + ": " + item.Value);
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
