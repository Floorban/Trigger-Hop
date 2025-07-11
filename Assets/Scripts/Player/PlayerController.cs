using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour, IDamageable
{
    public Rigidbody2D rb { get; private set; }
    private HealthSystem health;
    [HideInInspector] public bool justRevive;

/*    [Header("Movement")]
    public bool canMove = false;
    [SerializeField] private float moveSpeed = 3f;
    private int moveDir = 1; // 1 for right, -1 for left
    [SerializeField] private LayerMask playerLayer, platformLayer;*/

    [Header("RecoilEffect")]
    private Vector3 oriScale;
    //[SerializeField] private float movePauseDuration = 0.2f;

    static public UnityAction<PlayerController> OnLevelStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        oriScale = transform.localScale;
    }
    private void Start()
    {
        OnLevelStarted?.Invoke(this);
        health = GetComponent<HealthManager>()?.healthSystem;
    }

    public void ApplyRecoil(Vector2 dir, float force)
    {
        rb.AddForce(-dir.normalized * force, ForceMode2D.Impulse);
        RecoilSquash();
        //StartCoroutine(ShootPause());
/*        if (isOnPlatform && dir.normalized.y < -0.5f)
        {
            StartCoroutine(FallThroughPlatform());
        }*/
    }

    public void RecoilSquash(float duration = 0.08f, float squashAmount = 0.7f)
    {
        transform.DOKill();

        Vector3 originalScale = oriScale;
        Vector3 squashed = new Vector3(originalScale.x * squashAmount, originalScale.y / squashAmount, originalScale.z);

        transform.DOScale(squashed, duration).SetEase(Ease.OutQuad)
            .OnComplete(() => transform.DOScale(originalScale, duration * 0.8f).SetEase(Ease.InQuad));
    }
/*    private IEnumerator ShootPause()
    {
        //canMove = false;
        // set the duration depending on the current weapon (type and recoil)
        // using unscaled time here
        yield return new WaitForSecondsRealtime(movePauseDuration);
        canMove = true;
    }*/
    public void Stop()
    {
        GetComponentInChildren<WeaponManager>().inputLocked = true;
        DOTween.Kill(rb);
        Vector2 currentVelocity = rb.linearVelocity;

        DOTween.To(() => currentVelocity, x =>
        {
            currentVelocity = x;
            rb.linearVelocity = currentVelocity;
        }, Vector2.zero, 0.5f)
        .SetEase(Ease.OutQuad)
        .SetTarget(rb)
        .OnComplete(() => rb.bodyType = RigidbodyType2D.Kinematic);
    }
    public void UnFreeze()
    {
        var weapon = GetComponentInChildren<WeaponManager>();
        weapon.inputLocked = false;
        weapon.currentGun.Setup(this);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = false;
        justRevive = true;
    }

    public void TakeDamage(int damageAmount, float stun)
    {
        health.Damage(damageAmount);
        if (health.GetHealth() <= 0)
        {
            SceneController.instance.deadPos = transform.position + Vector3.one;
            SceneController.instance.LevelFinished(false);
        }
    }

    /*    private void ApplyCustomGravity()
    {
        if (SceneController.instance.GetPlayerTime() == 0f)
        {
            rb.linearVelocity = Vector2.zero; // freeze if paused
            return;
        }

        float timeScale = SceneController.instance.GetPlayerTime();
        if (timeScale == 0f) return;

        rb.linearDamping = timeScale < 1f && timeScale >= 0f ? slowMotionDrag / timeScale : baseDrag;
        //rb.gravityScale = customGravity * timeScale;

        Vector2 velocity = rb.linearVelocity;
        velocity.y -= customGravity * Time.fixedDeltaTime * timeScale;
        rb.linearVelocity = velocity;
    }*/

    /*private IEnumerator FallThroughPlatform()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        yield return new WaitForSeconds(fallThroughDuration);
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
    }*/

    /*    private void Flip()
        {
            moveDir *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }*/

    /*    private void OnCollisionEnter2D(Collision2D collision)
    {
*//*        if (collision.collider.CompareTag("Wall"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // hit from the side
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    Flip();
                    break;
                }
            }
        }*//*
        if (((1 << collision.gameObject.layer) & platformLayer) != 0)
        {
            isOnPlatform = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & platformLayer) != 0)
        {
            isOnPlatform = false;
        }
    }*/

    /*    private void HorizonMove() {
            if (!canMove) return;
            rb.linearVelocity = new Vector2(moveDir * moveSpeed * Time.fixedDeltaTime, rb.linearVelocity.y);
        }*/
    // TO DO: use leantween or dotween to replace the effect later
    /*    public IEnumerator RecoilSquash(float duration = 0.1f, float squashAmount = 0.8f)
        {
            Vector3 originalScale = transform.localScale;
            Vector3 squashed = new Vector3(originalScale.x * squashAmount, originalScale.y / squashAmount, originalScale.z);
            transform.localScale = squashed;

            yield return new WaitForSecondsRealtime(duration);
            transform.localScale = originalScale;
        }*/

    /// <summary>
    /// the old aimimg logic using Unity Touch
    /// </summary>

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
}
