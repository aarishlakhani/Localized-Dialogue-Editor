using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class LocalizationEditorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string newKey = "";
    private List<Language> languages = new List<Language>();
    private Dictionary<string, string> newTranslations = new Dictionary<string, string>();

    [MenuItem("Window/Localization Editor")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationEditorWindow>("Localization Editor");
    }

    void OnEnable()
    {
        RefreshLanguageList();
    }

    void RefreshLanguageList()
    {
        languages.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Language");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Language language = AssetDatabase.LoadAssetAtPath<Language>(path);
            if (language != null)
            {
                languages.Add(language);
            }
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        try
        {
            // Create New Language Button
            if (GUILayout.Button("Create New Language"))
            {
                CreateNewLanguage();
            }

            EditorGUILayout.Space();

            // If no languages exist, show info box
            if (languages.Count == 0)
            {
                EditorGUILayout.HelpBox("No languages found. Create a new language to begin.", MessageType.Info);
                return;
            }

            // Add New Translation Section
            EditorGUILayout.LabelField("Add New Translation", EditorStyles.boldLabel);
            newKey = EditorGUILayout.TextField("Key:", newKey);

            foreach (var language in languages)
            {
                if (language == null || string.IsNullOrEmpty(language.languageName))
                {
                    Debug.LogError("A language asset is null or has an empty language name.");
                    continue;
                }

                if (!newTranslations.ContainsKey(language.languageName))
                {
                    newTranslations[language.languageName] = "";
                }

                newTranslations[language.languageName] = EditorGUILayout.TextField(
                    language.languageName + ":",
                    newTranslations[language.languageName]
                );
            }

            if (GUILayout.Button("Add Translation"))
            {
                AddNewTranslation();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Existing Translations", EditorStyles.boldLabel);

            // Display Existing Translations
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            try
            {
                DisplayExistingTranslations();
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }

    private void CreateNewLanguage()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create New Language",
            "New Language.asset",
            "asset",
            "Please enter a name for the new language asset"
        );

        if (!string.IsNullOrEmpty(path))
        {
            Language newLanguage = ScriptableObject.CreateInstance<Language>();

            // Prompt the user to provide a name for the new language
            string languageName = Path.GetFileNameWithoutExtension(path); // Extract name from the file
            newLanguage.languageName = languageName;

            AssetDatabase.CreateAsset(newLanguage, path);
            AssetDatabase.SaveAssets();
            RefreshLanguageList();

            Debug.Log($"Created new language: {newLanguage.languageName}");
        }
    }

    private void AddNewTranslation()
    {
        // Validation: Ensure key is not empty or whitespace
        if (string.IsNullOrWhiteSpace(newKey))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a valid key.", "OK");
            return;
        }

        // Validation: Ensure key is unique across all languages
        foreach (var language in languages)
        {
            if (language.translations.ContainsKey(newKey))
            {
                EditorUtility.DisplayDialog("Error", $"The key '{newKey}' already exists in {language.languageName}.", "OK");
                return;
            }
        }

        foreach (var language in languages)
        {
            if (newTranslations.TryGetValue(language.languageName, out string translation))
            {
                Undo.RecordObject(language, "Add Translation");
                language.AddTranslation(newKey, translation);
                EditorUtility.SetDirty(language);
            }
        }

        // Clear input fields
        newKey = "";
        newTranslations.Clear();
        AssetDatabase.SaveAssets();
    }

    private void DisplayExistingTranslations()
    {
        // Get all unique keys across all languages
        HashSet<string> allKeys = new HashSet<string>();
        foreach (var language in languages)
        {
            foreach (var key in language.translations.Keys)
            {
                allKeys.Add(key);
            }
        }

        foreach (string key in allKeys.OrderBy(k => k))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            try
            {
                EditorGUILayout.LabelField($"Key: {key}", EditorStyles.boldLabel);

                foreach (var language in languages)
                {
                    if (language == null || string.IsNullOrEmpty(language.languageName))
                    {
                        Debug.LogError("A language asset is null or has an empty language name.");
                        continue;
                    }

                    string translation = "";
                    language.translations.TryGetValue(key, out translation);

                    EditorGUI.BeginChangeCheck();
                    string newTranslation = EditorGUILayout.TextField(language.languageName, translation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(language, "Update Translation");
                        language.AddTranslation(key, newTranslation);
                        EditorUtility.SetDirty(language);
                    }
                }

                if (GUILayout.Button("Delete Key"))
                {
                    DeleteTranslationKey(key);
                }
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
        }
    }

    private void DeleteTranslationKey(string key)
    {
        if (EditorUtility.DisplayDialog("Confirm Delete",
            $"Are you sure you want to delete the key '{key}' from all languages?",
            "Yes", "No"))
        {
            foreach (var language in languages)
            {
                Undo.RecordObject(language, "Delete Translation");
                language.RemoveTranslation(key);
                EditorUtility.SetDirty(language);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
