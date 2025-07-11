using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject canvasHome;
    public GameObject canvasHelp;
    public GameObject canvasLevel;
    public GameObject canvasWin;
    public GameObject canvasLose;
    public GameObject currentLevel;
    public GameObject[] levelPrefabs;

    public Transform levelParent;

    public Button buttonPlay;
    public Button buttonHelp;
    public Button buttonExitHelp;
    public Button buttonExitLevel;

    public Button[] levelButtons;

    private int currentLevelIndex = -1;

    void Start()
    {
        ShowCanvas(canvasHome);

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

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int level = i + 1;
            levelButtons[i].onClick.AddListener(() => {
                LoadLevel(level);
            });
        }
    }
    void ShowCanvas(GameObject targetCanvas)
    {
        canvasHome.SetActive(targetCanvas == canvasHome);
        canvasHelp.SetActive(targetCanvas == canvasHelp);
        canvasWin.SetActive(targetCanvas == canvasWin);
        canvasLevel.SetActive(targetCanvas == canvasLevel);
        if (targetCanvas == canvasWin || targetCanvas == canvasLose)
        {
            DisableOtherButtons(targetCanvas);
        }
        else
        {
            EnableAllButtons();
        }
    }

    public void LoadLevel(int level)
    {
        HideAllCanvases();

        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }

        if (level > 0 && level <= levelPrefabs.Length)
        {
            currentLevelIndex = level - 1;
            currentLevel = Instantiate(levelPrefabs[currentLevelIndex], levelParent);

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
        canvasHome.SetActive(false);
        canvasHelp.SetActive(false);
        canvasWin.SetActive(true);
        DisableOtherButtons(canvasWin);

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
        HideAllCanvases();
        canvasLose.SetActive(true);
        DisableOtherButtons(canvasLose);

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

        EnableAllButtons();
    }
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}