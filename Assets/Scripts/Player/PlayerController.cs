using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Rigidbody2D rb { get; private set; }

    [Header("Movement")]
    public bool canMove = false;
    [SerializeField] private float moveSpeed = 3f;
    private int moveDir = 1; // 1 for right, -1 for left

    [Header("RecoilEffect")]
    private Vector3 oriScale;
    [SerializeField] private float movePauseDuration = 0.2f;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        oriScale = transform.localScale;
    }
    private void FixedUpdate() {
        HorizonMove();
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Wall")) {
            foreach (ContactPoint2D contact in collision.contacts) {
                // hit from the side
                if (Mathf.Abs(contact.normal.x) > 0.1f) {
                    Flip();
                    break;
                }
            }
        }
    }
    private void HorizonMove() {
        if (!canMove) return;
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }
    private void Flip() {
        moveDir *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
/*    private void TouchInput() {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButtonDown(0)) {
            StartAiming();
        }
        else if (Input.GetMouseButton(0)) {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimDir = (mouseWorldPos - (Vector2)transform.position).normalized;
            UpdateAimLine(mouseWorldPos);
        }
        else if (Input.GetMouseButtonUp(0)) {
            Shoot(aimDir);
            StopAiming();
        }
#elif UNITY_IOS || UNITY_ANDROID
    if (Input.touchCount > 0) {
        Touch touch = Input.GetTouch(0);
        Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
        aimDir = (touchWorldPos - (Vector2)transform.position).normalized;

        if (touch.phase == TouchPhase.Began) {
            StartAiming();
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
            UpdateAimLine(touchWorldPos);
        }
        else if (touch.phase == TouchPhase.Ended) {
            Shoot(aimDir);
            StopAiming();
        }
    }
#endif
    }*/
    public void ApplyRecoil(Vector2 dir, float force) {
        rb.AddForce(-dir.normalized * force, ForceMode2D.Impulse);
        StartCoroutine(RecoilSquash());
        StartCoroutine(ShootPause());
    }
    public IEnumerator ShootPause() {
        canMove = false;
        // set the duration depending on the current weapon (type and recoil)
        // using unscaled time here
        yield return new WaitForSecondsRealtime(movePauseDuration);
        canMove = true;
    }
    // TO DO: use leantween or dotween to replace the effect later
    public IEnumerator RecoilSquash(float duration = 0.1f, float squashAmount = 0.8f) {
        Vector3 originalScale = transform.localScale;
        Vector3 squashed = new Vector3(originalScale.x * squashAmount, originalScale.y / squashAmount, originalScale.z);
        transform.localScale = squashed;

        yield return new WaitForSecondsRealtime(duration);
        transform.localScale = originalScale;
    }
}
