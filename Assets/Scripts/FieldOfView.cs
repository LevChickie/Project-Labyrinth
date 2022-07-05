using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //field of view data;

    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject player;
    public LayerMask targetMask;
    public LayerMask groundMask;
    public bool playerInSight;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SearchingForPlayer());
    }


    private IEnumerator SearchingForPlayer()
    {
        //Only enter if the player is nearby!!
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);
        while (true)
        {
           yield return wait;
           CheckFieldOfView();
        }
    }

    private void CheckFieldOfView()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, targetMask);
        if(rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, groundMask))
                {
                    playerInSight = true;
                    Debug.Log("Player is in sight!");
                }
                else { playerInSight = false; }
            }
            else
            {
                playerInSight = false;
            }
        }
        else if(playerInSight) { playerInSight = false; }
    }
}
