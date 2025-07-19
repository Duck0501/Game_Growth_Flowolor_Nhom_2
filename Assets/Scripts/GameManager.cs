using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject canvasHome;
    public GameObject canvasHelp;
    public GameObject canvasLevel;
    public GameObject canvasWin;
    public GameObject canvasLose;
    public GameObject canvasShop;
    public GameObject canvasBonus;
    public GameObject currentLevel;
    public GameObject[] levelPrefabs;

    public AudioSource audioSource;
    public AudioClip bgMusic;
    public AudioClip winSound;
    public AudioClip loseSound;

    public Transform levelParent;

    public Button buttonPlay;
    public Button buttonHelp;
    public Button buttonShop;
    public Button buttonExitShop;
    public Button buttonExitHelp;
    public Button buttonExitLevel;

    public Button[] levelButtons;
    public Text moveCountText;
    public Text scoreText;

    private int currentLevelIndex = -1;
    private List<WinBlock> winBlocks = new List<WinBlock>();
    private int moveCount = 0;
    private bool hasShownBonusThisSession = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        hasShownBonusThisSession = false;
        ShowCanvas(canvasHome);

        if (audioSource != null && bgMusic != null)
        {
            audioSource.clip = bgMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        buttonPlay.onClick.AddListener(() =>
        {
            ShowCanvas(canvasLevel);
        });

        buttonHelp.onClick.AddListener(() =>
        {
            ShowCanvas(canvasHelp);
        });

        buttonExitHelp.onClick.AddListener(() =>
        {
            ShowCanvas(canvasHome);
        });

        buttonExitLevel.onClick.AddListener(() =>
        {
            ShowCanvas(canvasHome);
        });

        buttonShop.onClick.AddListener(() =>
        {
            ShowCanvas(canvasShop);
        });

        buttonExitShop.onClick.AddListener(() =>
        {
            ShowCanvas(canvasHome);
        });

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int level = i + 1;
            levelButtons[i].onClick.AddListener(() => {
                LoadLevel(level);
            });
        }

        UpdateMoveCountText();
    }

    void ShowCanvas(GameObject targetCanvas)
    {
        canvasHome.SetActive(targetCanvas == canvasHome);
        canvasHelp.SetActive(targetCanvas == canvasHelp);
        canvasWin.SetActive(targetCanvas == canvasWin);
        canvasLose.SetActive(targetCanvas == canvasLose);
        canvasLevel.SetActive(targetCanvas == canvasLevel);
        canvasShop.SetActive(targetCanvas == canvasShop);
        if (canvasBonus != null)
        {
            canvasBonus.SetActive(targetCanvas == canvasHome && !hasShownBonusThisSession);
        }
        if (targetCanvas == canvasWin || targetCanvas == canvasLose)
        {
            DisableOtherButtons(targetCanvas);
        }
        else
        {
            EnableAllButtons();
        }
    }

    public void OnBonusCanvasHidden()
    {
        hasShownBonusThisSession = true;
    }

    public void LoadLevel(int level)
    {
        HideAllCanvases();
        winBlocks.Clear();
        moveCount = 0;
        UpdateMoveCountText();
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }

        if (level > 0 && level <= levelPrefabs.Length)
        {
            currentLevelIndex = level - 1;
            currentLevel = Instantiate(levelPrefabs[currentLevelIndex], levelParent);
            LevelQuestData questData = currentLevel.GetComponent<LevelQuestData>();
            if (questData != null)
            {
                QuestSystem.Instance?.StartQuest(
                    questData.timeLimit,
                    questData.rewardExp,
                    questData.rewardGold
                );
            }
            else
            {
                QuestSystem.Instance?.StartQuest();
            }
            Button[] buttons = currentLevel.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                string lowerName = btn.name.ToLower();

                if (lowerName.Contains("home"))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => ShowCanvas(canvasHome));
                }

                if (lowerName.Contains("replay"))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => LoadLevel(currentLevelIndex + 1));
                }
            }
        }
    }

    public void ShowWinCanvas()
    {
        QuestSystem.Instance?.OnLevelWin();
        canvasHome.SetActive(false);
        canvasHelp.SetActive(false);
        canvasLose.SetActive(false);
        canvasShop.SetActive(false);
        canvasBonus.SetActive(false);
        canvasWin.SetActive(true);
        DisableOtherButtons(canvasWin);

        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + moveCount;
        }

        SaveHighScore();

        Button[] winButtons = canvasWin.GetComponentsInChildren<Button>();
        foreach (Button btn in winButtons)
        {
            string lowerName = btn.name.ToLower();

            if (lowerName.Contains("home"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    HideAllCanvases();
                    ShowCanvas(canvasHome);
                });
            }
            else if (lowerName.Contains("next"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    HideAllCanvases();
                    if (currentLevelIndex + 1 >= levelPrefabs.Length)
                    {
                        ShowCanvas(canvasHome);
                    }
                    else
                    {
                        LoadLevel(currentLevelIndex + 2);
                    }
                });
            }
            else if (lowerName.Contains("reset"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    HideAllCanvases();
                    if (currentLevelIndex + 1 >= levelPrefabs.Length)
                    {
                        LoadLevel(1);
                    }
                    else
                    {
                        LoadLevel(currentLevelIndex + 1);
                    }
                });
            }
        }
    }

    public void ShowLoseCanvas()
    {
        canvasHome.SetActive(false);
        canvasHelp.SetActive(false);
        canvasWin.SetActive(false);
        canvasShop.SetActive(false);
        canvasBonus.SetActive(false);
        canvasLose.SetActive(true);
        DisableOtherButtons(canvasLose);

        if (audioSource != null && loseSound != null)
        {
            audioSource.PlayOneShot(loseSound);
        }

        Button[] loseButtons = canvasLose.GetComponentsInChildren<Button>();
        foreach (Button btn in loseButtons)
        {
            string lowerName = btn.name.ToLower();

            if (lowerName.Contains("home"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    HideAllCanvases();
                    ShowCanvas(canvasHome);
                });
            }
            else if (lowerName.Contains("reset"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    HideAllCanvases();
                    LoadLevel(currentLevelIndex + 1);
                });
            }
        }
    }

    void DisableOtherButtons(GameObject activeCanvas)
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            bool inActiveCanvas = btn.transform.IsChildOf(activeCanvas.transform);
            btn.interactable = inActiveCanvas;
        }
    }

    void EnableAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            btn.interactable = true;
        }
    }

    void HideAllCanvases()
    {
        canvasHome.SetActive(false);
        canvasHelp.SetActive(false);
        canvasLevel.SetActive(false);
        canvasWin.SetActive(false);
        canvasLose.SetActive(false);
        canvasShop.SetActive(false);
        canvasBonus.SetActive(false);

        EnableAllButtons();
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public void RegisterWinBlock(WinBlock block)
    {
        if (!winBlocks.Contains(block))
        {
            winBlocks.Add(block);
        }
    }

    public void CheckAllWinBlocksMatched()
    {
        foreach (WinBlock block in winBlocks)
        {
            if (!block.IsMatched())
                return;
        }

        ShowWinCanvas();
    }

    public void IncrementMoveCount()
    {
        if (QuestSystem.Instance != null && !QuestSystem.Instance.IsQuestActive()) return;

        moveCount++;
        UpdateMoveCountText();
        if (moveCount > 20)
        {
            ShowLoseCanvas();
        }
    }

    private void UpdateMoveCountText()
    {
        if (moveCountText != null)
        {
            moveCountText.text = "Move: " + moveCount;
        }
    }

    public bool IsQuestActive()
    {
        return QuestSystem.Instance != null && QuestSystem.Instance.IsQuestActive();
    }

    void SaveHighScore()
    {
        if (currentLevelIndex >= 0)
        {
            int highScore = PlayerPrefs.GetInt($"HighScore_Level_{currentLevelIndex}", int.MaxValue);
            if (moveCount < highScore)
            {
                PlayerPrefs.SetInt($"HighScore_Level_{currentLevelIndex}", moveCount);
                PlayerPrefs.Save();
            }
        }
    }

    public int GetHighScore(int levelIndex)
    {
        return PlayerPrefs.GetInt($"HighScore_Level_{levelIndex}", int.MaxValue);
    }
}