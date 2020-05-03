using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyWander : MonoBehaviour
{
    [SerializeField] private float wanderDistance;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private int minWanderFrequency;
    [SerializeField] private int maxWanderFrequency;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private int nextWanderFrame = 0;
    private int framesSinceLastWander = 0;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        framesSinceLastWander++;
        if(maxWanderFrequency != 0 && nextWanderFrame < framesSinceLastWander) 
        {
            float distance = Random.Range(0.0f, wanderDistance);
            float angle = Random.Range(0.0f, 2f * Mathf.PI);

            Vector2 deviation = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));

            targetPosition = initialPosition + deviation;

            nextWanderFrame = Random.Range(minWanderFrequency, maxWanderFrequency);
            framesSinceLastWander = 0;
        }

        gameObject.transform.position = Vector3.MoveTowards(
            new Vector3(gameObject.transform.position.x,
            gameObject.transform.position.y, 0f),
            new Vector3(targetPosition.x,
            targetPosition.y, 0f), wanderSpeed * Time.deltaTime);
    }
}
