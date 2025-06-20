using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeComponent : MonoBehaviour
{
    [Header("Rope Attributes")]
    [SerializeField] Material ropeMaterial;
    public float ropeLength = 0.5f;
    [SerializeField] int totalSegments = 5;
    [SerializeField] float segmentsPerUnit = 2f;
    int segments = 0;
    [SerializeField] float ropeWidth = 0.1f;

    [SerializeField] int verletIterations = 1;
    [SerializeField] int solverIterations = 1;

    LineRenderer line;
    RopePoint[] points;
    public bool Connected;
    public void InitRopePoints(Transform targetTrans, Transform startTrans)
    {
        if (totalSegments > 0)
            segments = totalSegments;
        else
            segments = Mathf.CeilToInt(ropeLength * segmentsPerUnit);

        //Vector3 cableDirection = (targetPoint - transform.position).normalized;
        Vector3 cableDirection = (targetTrans.position - startTrans.position).normalized;

        float initialSegmentLength = ropeLength / segments;
        points = new RopePoint[segments + 1];

        for (int pointIdx = 0; pointIdx <= segments; pointIdx++)
        {
            Vector3 initialPosition = transform.position + (cableDirection * (initialSegmentLength * pointIdx));
            points[pointIdx] = new RopePoint(initialPosition);
        }

        // Bind start and end points
        RopePoint start = points[0];
        RopePoint end = points[segments];
        start.Bind(startTrans);
        end.Bind(targetTrans);
    }
    public void InitLineRenderer()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = ropeWidth;
        line.endWidth = ropeWidth;
        line.positionCount = segments + 1;
        line.material = ropeMaterial;
        line.GetComponent<Renderer>().enabled = true;
    }
    void Update()
    {
        if (!Connected) return;
        RenderRope();
    }
    void FixedUpdate()
    {
        if (!Connected) return;
        for (int verletIdx = 0; verletIdx < verletIterations; verletIdx++)
        {
            ApplyRopePhysics();
            ApplyConstraints();
        }
        //ApplyDragForce(dragForce);
    }
    public void DisableRope()
    {
        Connected = false;
        if (line != null) line.enabled = false;
        Destroy(line);
        line = null;

        if (points == null) return;
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = null;
        }
    }
    void RenderRope()
    {
        for (int pointIdx = 0; pointIdx < segments + 1; pointIdx++)
        {
            line.SetPosition(pointIdx, points[pointIdx].Position);
        }
    }
    void ApplyRopePhysics()
    {
        Vector3 gravityDisplacement = Time.fixedDeltaTime * Time.fixedDeltaTime * Physics.gravity;
        foreach (RopePoint particle in points)
        {
            particle.UpdateVerlet(gravityDisplacement);
        }
    }
    void ApplyConstraints()
    {
        for (int iterationIdx = 0; iterationIdx < solverIterations; iterationIdx++)
        {
            ApplyDistanceConstraint();
        }
    }
    void ApplyDistanceConstraint()
    {
        float segmentLength = ropeLength / segments;
        for (int SegIdx = 0; SegIdx < segments; SegIdx++)
        {
            RopePoint particleA = points[SegIdx];
            RopePoint particleB = points[SegIdx + 1];

            // Find current vector between particles
            Vector3 delta = particleB.Position - particleA.Position;
            float currentDistance = delta.magnitude;
            float errorFactor = (currentDistance - segmentLength) / currentDistance;

            if (particleA.IsFree() && particleB.IsFree())
            {
                particleA.Position += errorFactor * 0.5f * delta;
                particleB.Position -= errorFactor * 0.5f * delta;
            }
            else if (particleA.IsFree())
            {
                particleA.Position += errorFactor * delta;
            }
            else if (particleB.IsFree())
            {
                particleB.Position -= errorFactor * delta;
            }
        }
    }
}
