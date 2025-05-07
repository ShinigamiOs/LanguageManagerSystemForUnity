// LanguageEditorMainWindow.cs
// Editor window for managing language projects in Unity Editor
//
// Provides functionality to:
// - View and edit all language keys and translations
// - Add new translation keys
// - Modify existing translations
// - Delete keys across all languages
// - Search and filter keys
// - Save changes to language files
//
// Author: Shinigami Overshini
// Date: 2024-04-30
// Version: 1.0

using UnityEngine;
using UnityEditor; // Required for EditorWindow and Editor GUI functionality
using System.Collections.Generic;
using System.IO; // For file operations
using System.Linq; // For LINQ operations on collections
using LanguageSystem.Runtime; // Access to core language system types

namespace LanguageSystem.Editor
{
    /// <summary>
    /// Main editor window for managing language projects in the Unity Editor.
    /// Provides a GUI interface for viewing, adding, editing, and deleting translation keys.
    /// </summary>
    public class LanguageEditorMainWindow : EditorWindow
    {
        // Current language project being edited
        private LanguageProject project;
        
        // Path to the project folder
        private string projectFolderPath;

        // Nested dictionary storing all language data:
        // Outer key: language code
        // Inner key: translation key
        // Value: translated text
        private Dictionary<string, Dictionary<string, string>> languageData = new();

        // Search filter for keys
        private string searchKey = "";
        
        // Currently selected key in the UI
        private string selectedKey = null;

        // Scroll position for the main view
        private Vector2 scrollPos;

        /// <summary>
        /// Opens the language editor window with the specified project data
        /// </summary>
        /// <param name="data">Language project to edit</param>
        /// <param name="folderPath">Path to the project folder</param>
        public static void Open(LanguageProject data, string folderPath)
        {
            var window = GetWindow<LanguageEditorMainWindow>($"Language: {data.projectName}");
            window.project = data;
            window.projectFolderPath = folderPath;
            window.LoadLanguageData();
            Debug.Log("Folder path in Open: "+folderPath);
        }

        /// <summary>
        /// Loads all language data from JSON files in the project folder
        /// </summary>
        private void LoadLanguageData()
        {
            languageData.Clear();
            string langFolder = Path.Combine(projectFolderPath, "Languages");
            
            // Load each language file
            foreach (var lang in project.languages)
            {
                string path = Path.Combine(langFolder, lang + ".json");
                if (File.Exists(path))
                {
                    var content = File.ReadAllText(path);
                    var parsed = JsonUtility.FromJson<LanguageFile>(content);
                    languageData[lang] = parsed.ToDictionary();
                }
                else
                {
                    // Initialize empty dictionary for missing language files
                    languageData[lang] = new Dictionary<string, string>();
                }
            }

            SyncAllKeys();
        }

        /// <summary>
        /// Saves all language data back to JSON files
        /// </summary>
        private void SaveLanguageData()
        {
            string langFolder = Path.Combine(projectFolderPath, "Languages");
            
             
            foreach (var lang in project.languages)
            {
                var file = new LanguageFile(languageData[lang]);

                if (file.entries == null || file.entries.Count == 0)
                {
                    Debug.LogWarning($"No hay entradas para el idioma: {lang}");
                    continue;
                }
                string json = JsonUtility.ToJson(file, true); 
                File.WriteAllText(Path.Combine(langFolder, lang + ".json"), json);
            }

            AssetDatabase.Refresh(); // Ensure Unity detects the changes
        }

        /// <summary>
        /// Main GUI rendering method for the editor window
        /// </summary>
        private void OnGUI()
        {
            // Project header
            GUILayout.Label($"Project: {project.projectName}", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // Search/Add key section
            EditorGUILayout.BeginHorizontal();
            searchKey = EditorGUILayout.TextField("Key", searchKey);

            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                // Check for duplicate keys (case-insensitive)
                bool keyExists = languageData[project.mainLanguage].Keys
                    .Any(k => k.Equals(searchKey, System.StringComparison.OrdinalIgnoreCase));

                if (!keyExists)
                {
                    AddNewKeyPopup(searchKey);
                }
                else
                {
                    EditorUtility.DisplayDialog("Duplicate", $"Key '{searchKey}' already exists (case-insensitive).", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            
            // Begin scrollable area
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Table header
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.Width(200));
            EditorGUILayout.LabelField(project.mainLanguage.ToUpper(), GUILayout.Width(200));
            
            // Additional language columns
            foreach (var lang in project.languages)
            {
                if (lang != project.mainLanguage)
                {
                    EditorGUILayout.LabelField(lang.ToUpper(), GUILayout.Width(200));
                }
            }
            EditorGUILayout.EndHorizontal();

            // Filter keys based on search term (case-insensitive)
            var keys = languageData[project.mainLanguage].Keys
                .Where(k => string.IsNullOrEmpty(searchKey) || k.ToLowerInvariant().Contains(searchKey.ToLowerInvariant()))
                .OrderBy(k => k)
                .ToList();

            // Display each key row
            foreach (var key in keys)
            {
                EditorGUILayout.BeginHorizontal();

                // Key selection button
                if (GUILayout.Button(key, GUILayout.Width(200)))
                {
                    selectedKey = key;
                }

                // Main language value (read-only)
                EditorGUILayout.LabelField(key, GUILayout.Width(200)); 

                // Other language values (editable)
                foreach (var lang in project.languages)
                {
                    if (lang != project.mainLanguage)
                    {
                        languageData[lang].TryGetValue(key, out string val);
                        languageData[lang][key] = EditorGUILayout.TextField(val ?? "", GUILayout.Width(200));
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);

            // Selected key actions
            if (!string.IsNullOrEmpty(selectedKey))
            {
                EditorGUILayout.LabelField($"Selected: {selectedKey}", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit"))
                {
                    EditKeyPopup(selectedKey);
                }
                if (GUILayout.Button("Delete"))
                {
                    if (EditorUtility.DisplayDialog("Delete", $"Delete key '{selectedKey}' from all languages?", "Yes", "No"))
                    {
                        DeleteKey(selectedKey);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // Save button
            if (GUILayout.Button("Save All"))
            {
                SaveLanguageData();
            }
        }

        /// <summary>
        /// Opens popup to add a new translation key
        /// </summary>
        /// <param name="newKey">Key to add</param>
        private void AddNewKeyPopup(string newKey)
        {
            LanguageEditPopup.Open(project, newKey, null, (result) => 
            {
                // Callback when popup is confirmed
                foreach (var lang in project.languages)
                {
                    if (lang == project.mainLanguage)
                    {
                        languageData[lang][newKey] = newKey;
                    }
                    else
                    {
                        languageData[lang][newKey] = result.TryGetValue(lang, out var val) ? val : "";
                    }
                }
                selectedKey = newKey;
            });
        }

        /// <summary>
        /// Opens popup to edit an existing key's translations
        /// </summary>
        /// <param name="key">Key to edit</param>
        private void EditKeyPopup(string key)
        {
            var current = project.languages.ToDictionary(
                l => l, 
                l => languageData[l].ContainsKey(key) ? languageData[l][key] : "");

            LanguageEditPopup.Open(project, key, current, (result) => 
            {
                // Update translations from popup result
                foreach (var lang in project.languages)
                {
                    if (lang != project.mainLanguage)
                    {
                        languageData[lang][key] = result.TryGetValue(lang, out var val) ? val : "";
                    }
                }
            });
        }

        /// <summary>
        /// Deletes a key from all languages
        /// </summary>
        /// <param name="key">Key to delete</param>
        private void DeleteKey(string key)
        {
            foreach (var lang in project.languages)
            {
                languageData[lang].Remove(key);
            }
            selectedKey = null;
        }

        /// <summary>
        /// Synchronizes keys across all languages to ensure consistency
        /// </summary>
        private void SyncAllKeys()
        {
            // Get all keys from main language
            var allKeys = new HashSet<string>(languageData[project.mainLanguage].Keys);

            // Ensure all keys exist in all languages
            foreach (var lang in project.languages)
            {
                foreach (var key in allKeys)
                {
                    if (!languageData[lang].ContainsKey(key))
                    {
                        languageData[lang][key] = "";
                    }
                }
            }
        }
    }
}