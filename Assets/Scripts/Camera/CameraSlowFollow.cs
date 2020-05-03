using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraSlowFollow : MonoBehaviour
{
    public GameObject target;
    Dictionary<string, string> debugLog = new Dictionary<string, string>();

    private void Update()
    {
        Vector2 targetPos2D = target.transform.position;
        Vector2 cameraPos2D = gameObject.transform.position;
        float distanceToTarget = (targetPos2D - cameraPos2D).sqrMagnitude / 2;

        gameObject.transform.position = Vector3.MoveTowards(
            new Vector3(gameObject.transform.position.x,
            gameObject.transform.position.y, -10f),
            new Vector3(target.transform.position.x,
            target.transform.position.y, -10f), distanceToTarget * Time.deltaTime);
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
            GUI.Label(new Rect(10, 70 + (20 * i), 200, 200), item.Key + ": " + item.Value);
            i++;
        }
    }
}