
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockIcon;

    private int levelNumber;
    private Action<int> onLevelSelected;

    public void Setup(
        int number,
        bool isUnlocked,
        Action<int> selectionCallback)
    {
        // Enable the LevelButton GameObject and its components.
        gameObject.SetActive(true);
        enabled = true;
    
        Image image = GetComponent<Image>();
        if (image != null)
            image.enabled = true;
    
        if (button != null)
        {
            button.enabled = true;
            button.interactable = isUnlocked;
        }
    
        if (levelNumberText != null)
        {
            levelNumberText.gameObject.SetActive(true);
            levelNumberText.enabled = true;
        }
    
        levelNumber = number;
        onLevelSelected = selectionCallback;
    
        levelNumberText.text = number.ToString();
    
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);
    
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        onLevelSelected?.Invoke(levelNumber);
    }
}
