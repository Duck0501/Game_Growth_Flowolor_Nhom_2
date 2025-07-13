using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GameManagerMove : MonoBehaviour
{
    public static GameManagerMove Instance;

    [Header("Các slot sẽ hiển thị khi click block")]
    public List<GameObject> hiddenObjects = new List<GameObject>();
    [Header("Parent của các slot")]
    public List<Transform> hiddenParents = new List<Transform>();

    private ClickableBlock currentBlock;
    private Transform currentBlockParent;

    void Awake()
    {
        Instance = this;
    }

    public void ShowHiddenObjects()
    {
        foreach (var obj in hiddenObjects)
            if (obj != null) obj.SetActive(true);
    }

    public void HideHiddenObjects()
    {
        foreach (var obj in hiddenObjects)
            if (obj != null) obj.SetActive(false);
    }

    public void SetCurrentBlock(ClickableBlock block)
    {
        currentBlock = block;
        currentBlockParent = block?.GetParent();
    }

    public ClickableBlock GetCurrentBlock() => currentBlock;

    public void SwapWithSlot(Transform slotParent)
    {
        if (currentBlockParent == null || slotParent == null) return;

        Vector3 posA = currentBlockParent.position;
        Quaternion rotA = currentBlockParent.rotation;

        Vector3 posB = slotParent.position;
        Quaternion rotB = slotParent.rotation;

        float duration = currentBlock.GetMoveDuration();

        Collider colA = currentBlockParent.GetComponentInChildren<Collider>();
        Collider colB = slotParent.GetComponentInChildren<Collider>();

        if (colA != null) colA.enabled = false;
        if (colB != null) colB.enabled = false;

        Tweener tweenA = currentBlockParent.DOMove(posB, duration);
        Tweener tweenB = slotParent.DOMove(posA, duration);

        Tween rotA_Tween = currentBlockParent.DORotateQuaternion(rotB, duration);
        Tween rotB_Tween = slotParent.DORotateQuaternion(rotA, duration);

        Sequence seq = DOTween.Sequence();
        seq.Append(tweenA);
        seq.Join(tweenB);
        seq.Join(rotA_Tween);
        seq.Join(rotB_Tween);

        seq.OnComplete(() =>
        {
            tweenA.Kill();
            tweenB.Kill();
            rotA_Tween.Kill();
            rotB_Tween.Kill();

            currentBlockParent.position = posB;
            currentBlockParent.rotation = rotB;

            slotParent.position = posA;
            slotParent.rotation = rotA;

            if (colA != null) colA.enabled = true;
            if (colB != null) colB.enabled = true;

            currentBlock.ResetToOriginalState();
            HideHiddenObjects();
            currentBlock = null;
            currentBlockParent = null;

            GameManager.Instance.IncrementMoveCount();
        });
    }
}
