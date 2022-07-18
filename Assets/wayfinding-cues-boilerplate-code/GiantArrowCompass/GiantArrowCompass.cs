using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantArrowCompass : MonoBehaviour
{
    //[SerializeField] private Transform target;

    private Vector3 target_position = Vector3.zero;

    //private void Start()
    //{
    //    target = Target.Instance.transform;
    //}

    public void SetTarget(GameObject obj)
    {
        target_position = obj.transform.position;
    }
    private void Update()
    {
        //target = Target.Instance.transform;

        if (target_position == Vector3.zero)
            return;

        Vector3 look_position = new Vector3(target_position.x, transform.position.y, target_position.z);

        transform.LookAt(target_position);
    }
}