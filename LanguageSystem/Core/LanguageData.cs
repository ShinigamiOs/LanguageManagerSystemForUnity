// LanguageData.cs
// Data structures for the UnityLanguageManager system
//
// Author: Shinigami Overshini
// Date: 2025-04-30
//
// Description:
// Core data models used by the language management system:
// - LanguageProject defines the project configuration (main language and supported languages)
// - LanguageFile stores key-value text entries
// - LanguageEntry represents individual text pairs

using System;
using System.Collections.Generic;

namespace LanguageSystem.Runtime{

    /// <summary>
    /// Project configuration containing the project name, main language, and supported languages.
    /// </summary>
    [Serializable]
    public class LanguageProject{
        public string projectName;
        public string mainLanguage;
        public List<string> languages;
    }

    /// <summary>
    /// Represents a collection of key-value text entries.
    /// Provides methods to convert to and from dictionaries.
    /// </summary>
    [Serializable]
    public class LanguageFile{
        public List<LanguageEntry> entries;
        public LanguageFile(){ }
        public LanguageFile(Dictionary<string, string> data){

             entries = new List<LanguageEntry>(); // Inicializa la lista siempre

            if (data == null){
                return;
            }
            foreach (var pair in data){
                entries.Add(new LanguageEntry { key = pair.Key, value = pair.Value });
            }
        }
        public Dictionary<string, string> ToDictionary(){
            Dictionary<string, string> dict = new();
            foreach (var entry in entries){
                dict[entry.key] = entry.value;
            }
            return dict;
        }

        public static LanguageFile FromDictionary(Dictionary<string, string> data){
            LanguageFile file = new();
            file.entries = new List<LanguageEntry>();
            foreach (var kvp in data){
                file.entries.Add(new LanguageEntry { key = kvp.Key, value = kvp.Value });
            }
            return file;
        }
    }

    /// <summary>
    /// Represents a single key-value text entry.
    /// </summary>
    [Serializable]
    public class LanguageEntry{
        public string key;
        public string value;
    }
}