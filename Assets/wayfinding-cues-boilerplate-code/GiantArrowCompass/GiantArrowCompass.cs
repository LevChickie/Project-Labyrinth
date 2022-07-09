using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantArrowCompass : MonoBehaviour
{
    //[SerializeField] private Transform target;

    public Transform target;

    private void Start()
    {
        target = Target.Instance.transform;
    }
    private void Update()
    {
        transform.LookAt(target.position);
    }
}