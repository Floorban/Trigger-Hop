using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public GameObject indicatorPrefab;

    private Camera cam;
    private SpriteRenderer spriteRend;
    private float spriteWidth;
    private float spriteHeight;

    private Transform player;

    // <target, indicator>
    private Dictionary<GameObject, GameObject> targetIndicators = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>().gameObject.transform;
        cam = Camera.main;
        spriteRend = indicatorPrefab.GetComponent<SpriteRenderer>();

        Bounds bounds = spriteRend.bounds;
        spriteWidth = bounds.size.x / 2f;
        spriteHeight = bounds.size.y / 2f;
    }
    private void Update()
    {
        foreach (KeyValuePair<GameObject, GameObject> entry in targetIndicators)
        {
            GameObject target = entry.Key;
            GameObject indicator = entry.Value;
            UpdateTarget(target, indicator);
        }
    }
    private void UpdateTarget(GameObject target, GameObject indicator)
    {
        Vector3 screenPos = cam.WorldToViewportPoint(target.transform.position);
        bool isOffscreen = screenPos.x <= 0 || screenPos.x >= 1 || screenPos.y <= 0 || screenPos.y >= 1;
        if (isOffscreen)
        {
            indicator.SetActive(true);
            Vector3 spriteSize = cam.WorldToViewportPoint(new Vector3(spriteWidth, spriteHeight, 0)) - cam.WorldToViewportPoint(Vector3.zero);
            screenPos.x = Mathf.Clamp(screenPos.x, spriteSize.x, 1-spriteSize.x);
            screenPos.y = Mathf.Clamp(screenPos.y, spriteSize.y, 1 - spriteSize.y);

            // set position
            Vector3 worldPos = cam.ViewportToWorldPoint(screenPos);
            worldPos.z = 0;
            indicator.transform.position = worldPos;

            // set rotation
            Vector3 dir = target.transform.position - player.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 110f));
        }
        else
        {
            indicator.SetActive(false);
        }
    }
    public void InitIndicator(GameObject target)
    {
        targets.Add(target);
        GameObject indicator = Instantiate(indicatorPrefab);
        indicator.SetActive(false);
        targetIndicators.Add(target, indicator);
    }
    public void RemoveTarget(GameObject target)
    {
        targets.Remove(target);

        if (targetIndicators.TryGetValue(target, out GameObject indicator))
        {
            Destroy(indicator);
            targetIndicators.Remove(target);
        }
    }
}
