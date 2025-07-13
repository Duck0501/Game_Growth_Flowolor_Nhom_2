using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WinBlock : MonoBehaviour
{
    [Header("Các màu cần va chạm (ví dụ: Blue, Green)")]
    [SerializeField] private List<Material> targetMaterials;

    [Header("Hiển thị khi đủ các màu")]
    [SerializeField] private Material fullyMatchedMaterial;

    [Header("GameObject xuất hiện khi đủ màu")]
    [SerializeField] private GameObject appearObject;
    [SerializeField] private float moveUpDistance = 1f;
    [SerializeField] private float moveDuration = 1f;

    private HashSet<string> matchedMaterialNames = new HashSet<string>();
    private Material firstMatchedMaterial;
    private Material defaultMaterial;
    private Renderer blockRenderer;
    private List<Collider> currentCollidingBlocks = new List<Collider>();
    private bool isMatched = false;

    private Vector3 lastVisiblePosition;

    void Awake()
    {
        blockRenderer = GetComponent<Renderer>();
        if (!blockRenderer)
        {
            Debug.LogError("WinBlock thiếu Renderer!", this);
            return;
        }

        defaultMaterial = blockRenderer.material;

        if (!GetComponent<Collider>())
            gameObject.AddComponent<BoxCollider>().isTrigger = true;

        if (!GetComponent<Rigidbody>())
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        if (appearObject != null)
            appearObject.SetActive(false);

        GameManager.Instance?.RegisterWinBlock(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ClickableBlock")) return;

        if (!currentCollidingBlocks.Contains(other))
            currentCollidingBlocks.Add(other);

        Renderer otherRenderer = other.GetComponent<Renderer>();
        if (otherRenderer != null)
        {
            string matName = otherRenderer.material.name.Replace(" (Instance)", "");

            if (!matchedMaterialNames.Contains(matName))
            {
                matchedMaterialNames.Add(matName);

                if (firstMatchedMaterial == null)
                {
                    firstMatchedMaterial = otherRenderer.material;
                    blockRenderer.material = firstMatchedMaterial;
                }

                UpdateVisual();
            }

            CheckIfMatched();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("ClickableBlock")) return;

        if (currentCollidingBlocks.Contains(other))
            currentCollidingBlocks.Remove(other);

        RecheckMatchState();
    }

    void RecheckMatchState()
    {
        if (currentCollidingBlocks.Count == 0)
        {
            if (isMatched)
            {
                isMatched = false;
                blockRenderer.material = defaultMaterial;
                firstMatchedMaterial = null;
                matchedMaterialNames.Clear();
                HideAppearObject();
            }
        }
    }

    void UpdateVisual()
    {
        if (matchedMaterialNames.Count >= targetMaterials.Count && fullyMatchedMaterial != null)
        {
            blockRenderer.material = fullyMatchedMaterial;
        }
    }

    private void CheckIfMatched()
    {
        foreach (Material target in targetMaterials)
        {
            string targetName = target.name.Replace(" (Instance)", "");
            if (!matchedMaterialNames.Contains(targetName))
                return;
        }

        if (isMatched) return;

        isMatched = true;

        if (fullyMatchedMaterial != null)
            blockRenderer.material = fullyMatchedMaterial;

        if (appearObject != null)
        {
            PlayAppearEffectThenNotify();
        }
        else
        {
            GameManager.Instance?.CheckAllWinBlocksMatched();
        }
    }

    void PlayAppearEffectThenNotify()
    {
        if (appearObject == null) return;

        lastVisiblePosition = appearObject.transform.position;
        Vector3 startPos = lastVisiblePosition - new Vector3(0, moveUpDistance, 0);

        appearObject.transform.position = startPos;
        appearObject.SetActive(true);

        appearObject.transform.DOMoveY(lastVisiblePosition.y, moveDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                GameManager.Instance?.CheckAllWinBlocksMatched();
            });
    }

    void HideAppearObject()
    {
        if (appearObject == null || !appearObject.activeSelf) return;

        Vector3 endPos = lastVisiblePosition - new Vector3(0, moveUpDistance, 0);

        appearObject.transform.DOMoveY(endPos.y, moveDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                appearObject.SetActive(false);
                appearObject.transform.position = lastVisiblePosition;
            });
    }

    public bool IsMatched() => isMatched;
}
