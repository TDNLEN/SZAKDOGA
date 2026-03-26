using UnityEngine;

public class TrainWagonFollower : MonoBehaviour
{
    [Header("Refs")]
    public Transform train;
    public Transform followPoint;

    [Header("Fallback Offset")]
    public Vector3 localOffset = new Vector3(-3f, 0f, 0f);

    private void LateUpdate()
    {
        if (train == null) return;

        if (followPoint != null)
        {
            transform.position = followPoint.position;
            transform.rotation = followPoint.rotation;
        }
        else
        {
            transform.position = train.TransformPoint(localOffset);
            transform.rotation = train.rotation;
        }
    }
}