using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantArrowCompass : MonoBehaviour
{
    //[SerializeField] private Transform target;

    private Transform target = null;

    private void Start()
    {
        target = Target.Instance.transform;
    }
    private void Update()
    {
        target = Target.Instance.transform;

        if (target = null)
            return;

        transform.LookAt(target.position);
    }
}