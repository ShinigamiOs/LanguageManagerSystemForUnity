// LanguageEditPopup.cs
// Editor popup window for editing translation values of a specific key across multiple languages
//
// Features:
// - Provides a modal interface for editing translations
// - Supports multiple languages (excluding the main language)
// - Includes save/cancel functionality
// - Displays key as read-only information
//
// Author: Shinigami Overshini
// Date: 2024-04-30
// Version: 1.0

using UnityEngine;
using UnityEditor; // Required for EditorWindow functionality
using System;
using System.Collections.Generic;
using LanguageSystem.Runtime; // Access to core language system types

namespace LanguageSystem.Editor
{
    /// <summary>
    /// Popup editor window for modifying translations of a specific key across multiple languages.
    /// Provides a modal interface that blocks other editor interaction until closed.
    /// </summary>
    public class LanguageEditPopup : EditorWindow
    {
        // The translation key being edited
        private string key;
        
        // List of available languages in the project
        private List<string> languages;
        
        // The project's main language (shown as read-only)
        private string mainLanguage;
        
        // Dictionary holding the edited values:
        // Key: language code
        // Value: translation text
        private Dictionary<string, string> values;
        
        // Callback invoked when saving changes
        private Action<Dictionary<string, string>> onSave;
        
        // Scroll position for the content area
        private Vector2 scroll;

        /// <summary>
        /// Opens the translation edit popup window
        /// </summary>
        /// <param name="project">Language project reference</param>
        /// <param name="key">The translation key being edited</param>
        /// <param name="existingValues">Current translations for this key (null for new keys)</param>
        /// <param name="onSave">Callback when saving changes (receives updated translations)</param>
        public static void Open(LanguageProject project, string key, 
                              Dictionary<string, string> existingValues, 
                              Action<Dictionary<string, string>> onSave)
        {
            var window = CreateInstance<LanguageEditPopup>();
            window.key = key;
            window.languages = project.languages;
            window.mainLanguage = project.mainLanguage;
            window.onSave = onSave;

            // Initialize values dictionary
            window.values = new Dictionary<string, string>();
            foreach (var lang in window.languages)
            {
                if (lang != window.mainLanguage)
                {
                    // Use existing value if available, otherwise empty string
                    if (existingValues != null && existingValues.TryGetValue(lang, out string val))
                    {
                        window.values[lang] = val;
                    }
                    else
                    {
                        window.values[lang] = "";
                    }
                }
            }

            // Configure window properties
            window.titleContent = new GUIContent("Edit Language Key");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 300);
            window.ShowUtility(); // Show as modal utility window
        }

        /// <summary>
        /// Draws the popup window GUI
        /// </summary>
        private void OnGUI()
        {
            // Display the key (read-only)
            EditorGUILayout.LabelField("Key:", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(key, EditorStyles.textField);
            EditorGUILayout.Space();

            // Begin scrollable area for translations
            scroll = EditorGUILayout.BeginScrollView(scroll);

            // Display editable fields for each language (excluding main language)
            foreach (var lang in languages)
            {
                if (lang != mainLanguage)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(lang.ToUpper(), GUILayout.Width(80));
                    values[lang] = EditorGUILayout.TextField(values[lang]);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            // Action buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                onSave?.Invoke(values);
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}