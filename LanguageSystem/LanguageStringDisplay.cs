// LanguageStringDisplay.cs
// Component for displaying localized text in UI using TextMeshPro
//
// Key Features:
// - Automatically displays localized text based on a string key
// - Updates text when language changes
// - Supports runtime key changes
// - Self-initializes TextMeshProUGUI component if not assigned
// - Provides manual refresh capability
//
// Usage:
// 1. Attach to any GameObject with TextMeshProUGUI component
// 2. Assign a language key in the inspector
// 3. Text will automatically update when language changes
//
// Author: Shinigami Overshini
// Date: 2024-04-30
// Version: 1.0

using TMPro; // TextMeshPro namespace
using UnityEngine;
using LanguageSystem.Runtime;

[RequireComponent(typeof(TextMeshProUGUI))] // Ensure component exists
public class LanguageStringDisplay : MonoBehaviour
{
    [SerializeField, Tooltip("Key for the localized string to display")]
    private string langStringKey;

    [SerializeField, Tooltip("Target TextMeshProUGUI component (auto-detected if null)")]
    private TextMeshProUGUI targetText;

    /// <summary>
    /// Initializes the TextMeshProUGUI reference if not assigned
    /// </summary>
    private void Awake()
    {
        if (targetText == null) 
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Initial text update when component starts
    /// </summary>
    private void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// Subscribes to language change events and updates text when enabled
    /// </summary>
    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    /// <summary>
    /// Unsubscribes from language change events when disabled
    /// </summary>
    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

    /// <summary>
    /// Updates the displayed text with current language value
    /// </summary>
    private void UpdateText()
    {
        // Validate required components
        if (targetText == null)
        {
            Debug.LogWarning($"[LanguageStringDisplay] No TextMeshProUGUI assigned on {gameObject.name}", this);
            return;
        }

        if (LanguageManager.Instance == null)
        {
            Debug.LogWarning("[LanguageStringDisplay] LanguageManager not available in scene", this);
            return;
        }

        // Get and display localized text
        string value = LanguageManager.Instance.LangString(langStringKey);
        targetText.text = value;
    }

    /// <summary>
    /// Changes the language key and updates the display
    /// </summary>
    /// <param name="newKey">New localization key to use</param>
    public void SetLanguageKey(string newKey)
    {
        langStringKey = newKey;
        UpdateText();
    }

    /// <summary>
    /// Gets the current language key
    /// </summary>
    /// <returns>Current localization key</returns>
    public string GetLanguageKey()
    {
        return langStringKey;
    }

    /// <summary>
    /// Forces a refresh of the displayed text
    /// </summary>
    public void Refresh()
    {
        UpdateText();
    }
}