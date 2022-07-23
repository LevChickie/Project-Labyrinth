using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;

public class HologramPath : MonoBehaviour
{
    [Header("Marker Configuration")]
    [SerializeField] private GameObject[] markers;
    [SerializeField] private Vector3 inactivePosition;
    [SerializeField] private float markerDistance = 5.0f;
    [SerializeField] private int skipAFewMarkers = 2;
    [Header("Agents Configuration")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;

    private NavMeshPath _currentPath;
    private NavMeshAgent _navAgent;

    // Start is called before the first frame update
    private void Start()
    {
        _currentPath = new NavMeshPath();
    }

    // Update is called once per frame
    private void Update()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Target").transform;
        
        if (target != null)
        {
            NavMeshHit playerNavMeshPos, targetNavMeshPos;
            NavMesh.SamplePosition(player.position, out playerNavMeshPos, 5.0f, NavMesh.AllAreas);
            NavMesh.SamplePosition(target.position, out targetNavMeshPos, 50.0f, NavMesh.AllAreas);

            NavMesh.CalculatePath(targetNavMeshPos.position,playerNavMeshPos.position, NavMesh.AllAreas, _currentPath);
            for (int i = 0; i < _currentPath.corners.Length - 1; i++)
                Debug.DrawLine(_currentPath.corners[i], _currentPath.corners[i + 1], Color.red);
            
            UpdateHologramMarkers(_currentPath.corners);   
        }
    }

    private void UpdateHologramMarkers(Vector3[] path)
    {
        if (path.Length < 2)
            return;

        var potentialMarkerPositions = new List<Vector3>();
        var rest = 0.0f;
        var from = Vector3.zero;
        var to = Vector3.zero;

        for (var i = 0; i < path.Length - 1; ++i)
        {
            from = path[i];
            to = path[i + 1];
            var pathSegmentLength = (to - from).magnitude;
            var remainingDistance = rest + pathSegmentLength;

            while (remainingDistance > markerDistance)
            {
                remainingDistance -= markerDistance;
                var pos = from + (to - from).normalized * (pathSegmentLength - remainingDistance);
                potentialMarkerPositions.Add(pos);
            }
            
            rest = remainingDistance;
        }

        for (var i = 0; i < markers.Length; ++i)
        {
            if (potentialMarkerPositions.Count > i + skipAFewMarkers)
                markers[i].transform.position =
                    potentialMarkerPositions[potentialMarkerPositions.Count - 1 - i];
            else
                markers[i].transform.position = inactivePosition;
        }
    }
}