using UnityEngine;

public class RoadController : MonoBehaviour
{
    public Transform resetPos;
    public Transform EndPos;
    public float speed = 2f;

    private Vector3 target;

    void Start()
    {
        // Begin by moving toward the end
        target = EndPos.position;
    }

    void Update()
    {
        // Move towards the EndPos at constant speed
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If we've reached or passed the EndPos, reset to resetPos
        if (Vector3.Distance(transform.position, EndPos.position) < 0.01f)
        {
            transform.position = resetPos.position;
        }
    }
}
