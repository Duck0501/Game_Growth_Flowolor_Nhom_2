using UnityEngine;
using System;
using TMPro;

[System.Serializable]
public class LevelDestination
{
    public Vector3[] destinations;
    public GameObject[] destinationObjects;
}

public class WinConditionChecker : MonoBehaviour
{
    public static WinConditionChecker Instance;

    [Header("Vị trí đích và GameObject cho từng level")]
    public LevelDestination[] levelDestinations;

    [Header("Danh sách player (kéo vào đây)")]
    public Transform[] playerTransforms;

    [Header("Ngưỡng so sánh tọa độ")]
    public float epsilon = 0.1f;

    [Header("Giới hạn số lần di chuyển")]
    [SerializeField] private int maxMoves = 10; // Giới hạn tối đa, chỉnh trong Inspector

    [Header("Text hiển thị số lần di chuyển")]
    public TextMeshProUGUI moveCountText; // Sử dụng TMP_Text cho TextMeshPro

    private Vector3[] destinationPositions;
    private GameObject[] destinationObjects;
    private Vector3[] playerPositions;
    private GameManager gameManager;
    private int moveCount = 0; // Số lần di chuyển

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        foreach (var level in levelDestinations)
        {
            if (level.destinationObjects != null)
            {
                foreach (var obj in level.destinationObjects)
                {
                    if (obj != null) obj.SetActive(false);
                }
            }
        }
        UpdatePlayerPositions();
        UpdateMoveCountText();
    }

    public void UpdatePlayerPositions()
    {
        if (playerTransforms == null || playerTransforms.Length == 0) return;

        playerPositions = new Vector3[playerTransforms.Length];
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            if (playerTransforms[i] != null)
            {
                playerPositions[i] = playerTransforms[i].position;
            }
        }
    }

    public void CheckDestinationObjects()
    {
        if (destinationPositions == null || playerPositions == null ||
            destinationObjects == null || destinationPositions.Length != destinationObjects.Length ||
            destinationPositions.Length != playerPositions.Length)
        {
            return;
        }

        for (int i = 0; i < destinationPositions.Length; i++)
        {
            if (destinationObjects[i] != null)
            {
                bool isAtDestination = Mathf.Abs(playerPositions[i].x - destinationPositions[i].x) <= epsilon &&
                                       Mathf.Abs(playerPositions[i].z - destinationPositions[i].z) <= epsilon;
                destinationObjects[i].SetActive(isAtDestination);
            }
        }
    }

    public void CheckWinCondition()
    {
        if (destinationPositions == null || playerPositions == null ||
            destinationPositions.Length == 0 || playerPositions.Length == 0 ||
            destinationPositions.Length != playerPositions.Length)
        {
            return;
        }

        bool allMatched = true;
        for (int i = 0; i < destinationPositions.Length; i++)
        {
            if (Mathf.Abs(playerPositions[i].x - destinationPositions[i].x) > epsilon ||
                Mathf.Abs(playerPositions[i].z - destinationPositions[i].z) > epsilon)
            {
                allMatched = false;
                break;
            }
        }

        if (allMatched && gameManager != null)
        {
            gameManager.ShowWinCanvas();
        }
    }

    public void SetupLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelDestinations.Length)
        {
            return;
        }

        destinationPositions = levelDestinations[levelIndex].destinations;
        destinationObjects = levelDestinations[levelIndex].destinationObjects;

        if (destinationObjects != null)
        {
            foreach (var obj in destinationObjects)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        UpdatePlayerPositions();
        CheckDestinationObjects();
    }

    public void ResetMoveCount()
    {
        moveCount = 0;
        UpdateMoveCountText();
    }

    public void IncrementMoveCount()
    {
        moveCount++;
        if (moveCount > maxMoves && gameManager != null)
        {
            gameManager.ShowLoseCanvas();
        }
        UpdateMoveCountText();
    }

    private void UpdateMoveCountText()
    {
        if (moveCountText != null)
        {
            moveCountText.text = moveCount + "/" + maxMoves;
        }
    }
}