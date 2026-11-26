
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject sudokuPanel;

    [Header("Game Reference")]
    [SerializeField] private LevelSelectionManager levelSelectionManager;

    private void Start()
    {
        ShowHomePanel();
    }

    // Home Screen
    public void ShowHomePanel()
    {
        homePanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
        sudokuPanel.SetActive(false);
    }

    // Play Button: Open Level Selection
    public void ShowLevelSelection()
    {
        homePanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        sudokuPanel.SetActive(false);

        levelSelectionManager.RefreshLevelButtons();
    }

    // Return from Level Selection to Home
    public void ReturnToHome()
    {
        ShowHomePanel();
    }
}
