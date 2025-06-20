using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class RopeSwingManager : MonoBehaviour
{
    private List<CapsuleCollider> ropeSegments = new List<CapsuleCollider>();
    CapsuleCollider playerCollider;
    RopeComponent rc;

    void Start()
    {

        rc = GetComponent<RopeComponent>();
        playerCollider = FindAnyObjectByType<PlayerController>().GetComponent<CapsuleCollider>();
        foreach (Transform child in this.transform)
        {
            ropeSegments.AddRange(child.GetComponentsInChildren<CapsuleCollider>());
            child.GetComponent<MeshRenderer>().enabled = false;
        }
        rc.InitRopePoints( transform, ropeSegments[ropeSegments.Count - 1].transform);
        rc.InitLineRenderer();
        rc.Connected = true;
    }

    public void OnRopeExit()
    {
        foreach(CapsuleCollider collider in ropeSegments)
        {
            Physics.IgnoreCollision(collider, playerCollider, true);
        }
    }
    public void ResetCollision()
    {
        foreach (CapsuleCollider collider in ropeSegments)
        {
            Physics.IgnoreCollision(collider, playerCollider, false);
        }
    }
}
