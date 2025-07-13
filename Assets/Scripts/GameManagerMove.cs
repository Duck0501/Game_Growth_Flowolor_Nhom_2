using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GameManagerMove : MonoBehaviour
{
    public static GameManagerMove Instance;

    [Header("Các slot sẽ hiển thị khi click block")]
    public List<GameObject> hiddenObjects = new List<GameObject>();   // Slot con
    [Header("Parent của các slot")]
    public List<Transform> hiddenParents = new List<Transform>();     // Slot parent

    private ClickableBlock currentBlock;
    private Transform currentBlockParent;  // Parent của block đang chọn
    private GameManager gameManager; // Reference tới GameManager

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Tìm GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Không tìm thấy GameManager trong scene!");
        }
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
        if (currentBlock == null || currentBlockParent == null || slotParent == null) return;

        // Lưu lại transform
        Vector3 posA = currentBlockParent.position;
        Quaternion rotA = currentBlockParent.rotation;

        Vector3 posB = slotParent.position;
        Quaternion rotB = slotParent.rotation;

        float duration = currentBlock.GetMoveDuration();

        // Tắt collider
        Collider colA = currentBlockParent.GetComponentInChildren<Collider>();
        Collider colB = slotParent.GetComponentInChildren<Collider>();

        if (colA != null) colA.enabled = false;
        if (colB != null) colB.enabled = false;

        // Tạo tween
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
            // Hủy tween
            tweenA.Kill();
            tweenB.Kill();
            rotA_Tween.Kill();
            rotB_Tween.Kill();

            // Hoán đổi vị trí
            currentBlockParent.position = posB;
            currentBlockParent.rotation = rotB;

            slotParent.position = posA;
            slotParent.rotation = rotA;

            // Bật lại collider
            if (colA != null) colA.enabled = true;
            if (colB != null) colB.enabled = true;

            currentBlock.ResetToOriginalState();
            HideHiddenObjects();
            currentBlock = null;
            currentBlockParent = null;

            // Kiểm tra đích và thắng
            if (WinConditionChecker.Instance != null && gameManager != null)
            {
                int levelIndex = gameManager.GetCurrentLevelIndex();
                WinConditionChecker.Instance.SetupLevel(levelIndex);
                WinConditionChecker.Instance.UpdatePlayerPositions();
                WinConditionChecker.Instance.CheckDestinationObjects(); 
                WinConditionChecker.Instance.CheckWinCondition();
            }
        });
    }
}