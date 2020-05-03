using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubHelper : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private Vector2 raycastPosition;
    public RaycastHit2D lineOfSightRaycast;
    public RaycastHit2D followRaycast;
    private float direction;
    public GameObject questMarker;
    private bool bounceTriggered; 
    float speed;
    public GameObject target;
    public float followDistance;
    public float sightDistance;

    void Start()
    {
        questMarker.GetComponent<SpriteRenderer>().forceRenderingOff = true; 
        speed = 1; 
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        direction = Mathf.Sign(speed);
        raycastPosition = new Vector2(transform.position.x + 0.55f * direction,
            transform.position.y + -0.2f);
        lineOfSightRaycast = Physics2D.Raycast(raycastPosition, Vector2.right, sightDistance);
        followRaycast = Physics2D.Raycast(raycastPosition, Vector2.right, followDistance);

        if (lineOfSightRaycast.collider != null)
        {
            if(lineOfSightRaycast.collider.name == "Player")
            {
                questMarker.GetComponent<SpriteRenderer>().forceRenderingOff = false;
                if (!bounceTriggered)
                {
                    questMarker.GetComponent<Animator>().SetTrigger("Bounce");
                    bounceTriggered = true;
                }
                if (target)
                {
                    if (followRaycast)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 1f * Time.deltaTime);
                        gameObject.transform.SetPositionAndRotation(transform.position, new Quaternion(0f, 180f, 0f, 0f));
                    }else
                    {
                        gameObject.transform.SetPositionAndRotation(transform.position, new Quaternion(0f, 0, 0f, 0f));
                    }
                }
            }
        }
        else
        {
            questMarker.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            bounceTriggered = false;
            gameObject.transform.SetPositionAndRotation(transform.position, new Quaternion(0f, 0f, 0f, 0f));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(raycastPosition, Vector2.right);
    }
}
