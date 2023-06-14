using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Target;
    private Vector3 offset;
    private float distance;
    private Vector3 currentAngles;
    void Start()
    {
        offset = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        UpdateTransform();
    }

    public void UpdateTransform()
    {
        currentAngles = Target.eulerAngles;
        Target.eulerAngles = new Vector3(0f, Target.eulerAngles.y, 0f);
        transform.parent = Target;
        transform.localPosition = offset;
        transform.eulerAngles = Target.eulerAngles;
        transform.parent = null;

        Target.eulerAngles = currentAngles;

    }
}
