using UnityEngine;

public class NewRopeSwing : MonoBehaviour
{
    public float swingAcceleration = 100f;
    public float launchForce = 2f;
    public float climbSpeed = 2f;
    public float ropePull = 1000f;
    public float climbSmoothing = 600f;
    private ConfigurableJoint playerJoint;
    private Rigidbody playerRb;
    private bool isTouchingRope = false;
    Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private RopeSwingManager ropeSwingManager;
    bool attached = false;

    private void Start()
    {
        ropeSwingManager = FindAnyObjectByType<RopeSwingManager>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    private void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        //Attach player when touching rope and holding down space
        if (isTouchingRope && Input.GetKey(KeyCode.Space) && !attached)
        {
            AttachPlayer();
        }
        //Detach player when attached and releasing space
        else if (Input.GetKeyUp(KeyCode.Space) && attached)
        {
            DetachPlayer();
        }
        //Break attachment to rope segment when detecting vertical input while not touching rope while being attached
        else if ((Mathf.Abs(verticalInput) > 0.01f) && !isTouchingRope && attached)
        {
            BreakJoint();
        }
            if (attached && Input.GetKey(KeyCode.Space) && playerJoint != null)
            {
                // Horizontal swinging
                  float swingInput = Input.GetAxis("Horizontal");
                  rb.AddForce(Vector3.right * swingInput * swingAcceleration + rb.linearVelocity, ForceMode.Force);

               // Vertical climbing
            if (Mathf.Abs(verticalInput) > 0.01f)
            {
                Vector3 climb = Vector3.up * verticalInput * climbSpeed * Time.deltaTime;
                playerRb.MovePosition(playerRb.position + climb);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingRope = true;
            playerRb = collision.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void AttachPlayer()
    {
        if (playerRb == null || playerJoint != null) return;

        attached = true;
        playerJoint = gameObject.AddComponent<ConfigurableJoint>();
        playerJoint.connectedBody = playerRb;

        // Allow movement only along Y-axis (up/down)
        playerJoint.xMotion = ConfigurableJointMotion.Locked;
        playerJoint.yMotion = ConfigurableJointMotion.Free;
        playerJoint.zMotion = ConfigurableJointMotion.Locked;

        // Lock rotation
        playerJoint.angularXMotion = ConfigurableJointMotion.Locked;
        playerJoint.angularYMotion = ConfigurableJointMotion.Locked;
        playerJoint.angularZMotion = ConfigurableJointMotion.Locked;


        // Drive to smooth out climbing and keep hold of the rope
        JointDrive drive = new JointDrive {positionSpring = ropePull, positionDamper = climbSmoothing,maximumForce = Mathf.Infinity };
        playerJoint.yDrive = drive;

        playerRb.useGravity = false;
    }

    private void BreakJoint()
    {
        if (playerJoint != null)
        {
            attached = false;
            Destroy(playerJoint);
            playerJoint = null;
        }
    }
    private void DetachPlayer()
    {
        if (playerJoint != null)
        {
            attached = false;
            Destroy(playerJoint);
            playerJoint = null;
            playerRb.useGravity = true;

            playerRb.AddForce(new Vector3(1, 1, 0) * rb.linearVelocity.x * launchForce, ForceMode.Impulse);
            ropeSwingManager.OnRopeExit();
            Invoke("ResetLayer", 0.4f);
        }
    }
    void ResetLayer()
    {
        ropeSwingManager.ResetCollision();
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingRope = false;
        }
    }
}