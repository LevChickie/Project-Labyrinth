using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //field of view data;
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject player;
    public LayerMask targetMask;
    public LayerMask groundMask;
    public bool playerInSight;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInSight = false;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine(SearchingForPlayer());
    }

    void LateUpdate()
    {
        DrawFieldOfView();
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

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(angle * meshResolution);
        float stepAngleSize = angle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angleCurrent = transform.eulerAngles.y - angle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angleCurrent);
            viewPoints.Add(newViewCast.point);
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    private void CheckFieldOfView()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, groundMask))
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
        else if (playerInSight) { playerInSight = false; }
    }

    public Vector3 dirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float rayAngle;
        public ViewCastInfo(bool hit, Vector3 point, float distance, float rayAngle)
        {
            this.hit = hit;
            this.point = point;
            this.distance = distance;
            this.rayAngle = rayAngle;
        }
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = dirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, radius, groundMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * radius, radius, globalAngle);
        }
    }
}
