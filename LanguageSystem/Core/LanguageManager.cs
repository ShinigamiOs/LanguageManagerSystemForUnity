// LanguageManager.cs
// Manages multi-language support for Unity projects using JSON-based language files.
// 
// Features:
// - Load language project from JSON assets
// - Supports dynamic language switching at runtime
// - Provides case-insensitive key lookup
// - Offers string formatting options (upper, lower, capitalized)
// - Implements singleton pattern for global access
// - Includes event system for language change update
//
// Author: Shinigami Overshini
// Date: 2024-04-30
// Version: 1.0
// 
// Usage:
// 1. Create a language project file using the editor window
// 2. Add language files for each supported language
// 3. Assign the language project asset to the manager
// 4. Use LangString(key) to retrieve localized strings
// 5. Call SetLanguage(languageCode) to change languages at runtime

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace LanguageSystem.Runtime{

    /// <summary>
    /// Core manager for handling multilingual support in Unity projects.
    /// Implements singleton pattern for global access and provides methods
    /// for loading language files, switching languages, and retrieving localized strings.
    /// </summary>
    public class LanguageManager : MonoBehaviour{
        
        /// <summary>
        /// Singleton instance of the LanguageManager
        /// </summary>
        public static LanguageManager Instance { get; private set; }

        [Header("Proyecto de Lenguaje (.json)")]
        [SerializeField] private TextAsset languageProjectAsset;

        private string projectName;
        private string mainLanguage;
        private List<string> languages;
        private string currentLanguage;

        // Dictionary storing current language strings (case-insensitive comparison)
        private Dictionary<string, string> currentData = new();

        /// <summary>
        /// Path to the language files directory (readonly)
        /// </summary>
        private string ProjectFolder => Path.Combine(Application.dataPath, "UnityLanguageManager","Resources", projectName, "Languages");

        /// <summary>
        /// Event triggered when language is changed
        /// </summary>
        public static event Action OnLanguageChanged;

        /// <summary>
        /// Initializes the singleton instance and loads language data
        /// </summary>
        private void Awake(){
            if (Instance != null && Instance != this){
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (languageProjectAsset == null){
                Debug.LogError("[LanguageManager] No se ha asignado un proyecto de lenguaje.");
                return;
            }

            LoadProject(languageProjectAsset.text);

            currentLanguage = PlayerPrefs.GetString("LanguageManager.CurrentLanguage", mainLanguage);
            LoadLanguage(currentLanguage);
        }

        /// <summary>
        /// Loads language project configuration from JSON
        /// </summary>
        /// <param name="json">JSON string containing project configuration</param>
        private void LoadProject(string json){
            var project = JsonUtility.FromJson<LanguageProject>(json);
            projectName = project.projectName;
            mainLanguage = project.mainLanguage;
            languages = project.languages;
        }

        /// <summary>
        /// Changes the current application language
        /// </summary>
        /// <param name="language">Language code to switch to</param>
        public void SetLanguage(string language){
            if (language == currentLanguage) return;
            LoadLanguage(language);
            PlayerPrefs.SetString("LanguageManager.CurrentLanguage", language);
            PlayerPrefs.Save();
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Gets the currently active language code
        /// </summary>
        /// <returns>Current language code</returns>
        public string GetCurrentLanguage(){
            return currentLanguage;
        }

        /// <summary>
        /// Loads language data from specified language file
        /// </summary>
        /// <param name="language">Language code to load</param>
        private void LoadLanguage(string language){
            string path = Path.Combine(ProjectFolder, language + ".json");
            if (!File.Exists(path)){
                Debug.LogError($"[LanguageManager] No se encontró el archivo de idioma: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            var langData = JsonUtility.FromJson<LanguageFile>(json);

            
            currentData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in langData.entries){
                if (!string.IsNullOrEmpty(entry.key)){
                    currentData[entry.key.ToLowerInvariant()] = entry.value;
                }
            }

            currentLanguage = language;
        }

        /// <summary>
        /// Retrieves localized string for the given key
        /// </summary>
        /// <param name="key">Localization key</param>
        /// <returns>Localized string or [key] if not found</returns>
        public string LangString(string key){
            if (string.IsNullOrEmpty(key)) return "[null]";
            string lowerKey = key.ToLowerInvariant();
            if (currentData.TryGetValue(lowerKey, out var value)){
                return value;
            }
            return $"[{key}]";
        }
        
        /// <summary>
        /// Retrieves localized string in uppercase
        /// </summary>
        public string LangUpper(string key) => LangString(key).ToUpper();

        /// <summary>
        /// Retrieves localized string in lowercase
        /// </summary>
        public string LangLower(string key) => LangString(key).ToLower();

        /// <summary>
        /// Retrieves localized string with first letter capitalized
        /// </summary>
        public string LangCapitalized(string key){
            var val = LangString(key);
            return string.IsNullOrEmpty(val) ? val : char.ToUpper(val[0]) + val.Substring(1).ToLower();
        }

        /// <summary>
        /// Checks if a localization key exists
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if key exists in current language</returns>
        public bool HasKey(string key){
            if (string.IsNullOrEmpty(key)) return false;
            return currentData.ContainsKey(key.ToLowerInvariant());
        }
    }
}