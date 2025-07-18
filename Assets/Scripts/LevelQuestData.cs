using UnityEngine;

public class LevelQuestData : MonoBehaviour
{
    [Tooltip("Thời gian giới hạn cho nhiệm vụ ở level này (giây)")]
    public float timeLimit = 30f;

    [Tooltip("Phần thưởng EXP nếu hoàn thành")]
    public int rewardExp = 100;

    [Tooltip("Phần thưởng Gold nếu hoàn thành")]
    public int rewardGold = 50;
}
