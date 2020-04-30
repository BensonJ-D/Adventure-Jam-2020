using UnityEngine;

public class CameraSlowFollow : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(
            new Vector3(gameObject.transform.position.x,
            0, -10f),
            new Vector3(target.transform.position.x,
            0, -10f), 3 * Time.deltaTime);
    }
}