// LanguageProjectSetupWindow.cs
// Editor window for creating and configuring new language projects
//
// Features:
// - Creates new language projects with configurable languages
// - Allows setting a main/default language
// - Supports loading existing projects
// - Creates necessary folder structure and JSON files
// - Integrates with the main language editor window
//
// Author: Shinigami Overshini
// Date: 2024-04-30
// Version: 1.0

using UnityEngine;
using UnityEditor; // Required for EditorWindow and Editor GUI functionality
using System.Collections.Generic;
using System.IO; // For file system operations
using LanguageSystem.Runtime; // Access to core language system types

namespace LanguageSystem.Editor
{
    /// <summary>
    /// Setup window for creating new language projects or loading existing ones.
    /// Provides interface for configuring project name, languages, and main language.
    /// </summary>
    public class LanguageProjectSetupWindow : EditorWindow
    {
        // Project configuration fields
        private string projectName = "NewLanguageProject";
        private List<string> languages = new List<string> { "en" }; // Default language
        private int mainLanguageIndex = 0; // Index of selected main language
        
        // Temporary field for adding new languages
        private string newLangCode = "";

        /// <summary>
        /// Menu item to show the setup window
        /// </summary>
        [MenuItem("Window/Language Manager/Setup Project")]
        public static void ShowWindow()
        {
            GetWindow<LanguageProjectSetupWindow>("Language Project Setup");
        }

        /// <summary>
        /// Main GUI rendering method for the setup window
        /// </summary>
        private void OnGUI()
        {
            // Window title and project name field
            GUILayout.Label("Create or Load Language Project", EditorStyles.boldLabel);
            projectName = EditorGUILayout.TextField("Project Name", projectName);

            // Languages list management
            GUILayout.Label("Languages");
            for (int i = 0; i < languages.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                languages[i] = EditorGUILayout.TextField(languages[i]);
                
                // Delete button (disabled for main language)
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (i != mainLanguageIndex)
                    {
                        languages.RemoveAt(i);
                        if (mainLanguageIndex > i) mainLanguageIndex--;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            "Invalid Action", 
                            "Cannot remove the main language.", 
                            "OK");
                    }
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            // Add new language field
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            newLangCode = EditorGUILayout.TextField("New Language", newLangCode);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                if (!string.IsNullOrWhiteSpace(newLangCode) && !languages.Contains(newLangCode))
                {
                    languages.Add(newLangCode);
                }
                newLangCode = "";
            }
            EditorGUILayout.EndHorizontal();

            // Main language selection
            EditorGUILayout.Space(10);
            mainLanguageIndex = EditorGUILayout.Popup(
                "Main Language", 
                mainLanguageIndex, 
                languages.ToArray());

            // Action buttons
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Create Project"))
            {
                CreateProjectFile();
            }

            if (GUILayout.Button("Load Existing Project"))
            {
                string path = EditorUtility.OpenFilePanel(
                    "Load Language Project", 
                    "Assets/UnityLanguageManager/Resources", 
                    "json");
                
                if (!string.IsNullOrEmpty(path))
                {
                    LoadProjectFile(path);
                }
            }
        }

        /// <summary>
        /// Creates a new language project with the current configuration
        /// </summary>
        private void CreateProjectFile()
        {
            // Create project folder structure
            string folderPath = $"Assets/UnityLanguageManager/Resources/{projectName}";
            string langFolder = Path.Combine(folderPath, "Languages");

            if (!Directory.Exists(langFolder))
            {
                Directory.CreateDirectory(langFolder);
            }

            // Create empty JSON files for each language
            foreach (var lang in languages)
            {
                string langFile = Path.Combine(langFolder, lang + ".json");
                if (!File.Exists(langFile))
                {
                    File.WriteAllText(langFile, JsonUtility.ToJson(new LanguageFile(), true));
                }
            }

            // Create project definition file
            LanguageProject data = new LanguageProject
            {
                projectName = projectName,
                mainLanguage = languages[mainLanguageIndex],
                languages = new List<string>(languages)
            };

            string projectFilePath = Path.Combine(folderPath, "LanguageProject.json");
            File.WriteAllText(projectFilePath, JsonUtility.ToJson(data, true));

            // Refresh Unity's asset database
            AssetDatabase.Refresh();

            // Open the main editor window with the new project
            LanguageEditorMainWindow.Open(data, folderPath);
        }

        /// <summary>
        /// Loads an existing language project
        /// </summary>
        /// <param name="path">Path to the project JSON file</param>
        private void LoadProjectFile(string path)
        {
            string json = File.ReadAllText(path);
            LanguageProject data = JsonUtility.FromJson<LanguageProject>(json);
            string folderPath = Path.GetDirectoryName(path).Replace("\\", "/");

            // Open the main editor window with the loaded project
            LanguageEditorMainWindow.Open(data, folderPath);
        }
    }
}