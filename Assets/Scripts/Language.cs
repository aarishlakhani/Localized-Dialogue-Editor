using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Language", menuName = "Localization/Language")]
public class Language : ScriptableObject, ISerializationCallbackReceiver
{
    public string languageName; // Name of the language (e.g., "English")
    public Dictionary<string, string> translations = new Dictionary<string, string>();

    [System.Serializable]
    public class TranslationEntry
    {
        public string key;   // Translation key
        public string value; // Translation value
    }

    [SerializeField]
    private List<TranslationEntry> serializedTranslations = new List<TranslationEntry>();

    // Called before the object is serialized
    public void OnBeforeSerialize()
    {
        serializedTranslations.Clear();
        foreach (var kvp in translations)
        {
            serializedTranslations.Add(new TranslationEntry { key = kvp.Key, value = kvp.Value });
        }
    }

    // Called after the object is deserialized
    public void OnAfterDeserialize()
    {
        translations.Clear();

        foreach (var entry in serializedTranslations)
        {
            if (!string.IsNullOrEmpty(entry.key))
            {
                translations[entry.key] = entry.value;
                Debug.Log($"Deserialized key: {entry.key}, value: {entry.value}");
            }
            else
            {
                Debug.LogWarning("Encountered an empty key in Serialized Translations. Skipping entry.");
            }
        }

        if (translations.Count == 0)
        {
            Debug.LogWarning($"No translations found for the language: {languageName}. Ensure the Serialized Translations list is correctly populated.");
        }
    }

    // Add a translation to the dictionary and serialize it
    public void AddTranslation(string key, string value)
    {
        if (!string.IsNullOrEmpty(key))
        {
            translations[key] = value;
            OnBeforeSerialize(); // Ensure it gets serialized
            Debug.Log($"Added translation: Key = {key}, Value = {value}");
        }
        else
        {
            Debug.LogError("Cannot add a translation with an empty key.");
        }
    }

    // Remove a translation by key
    public void RemoveTranslation(string key)
    {
        if (translations.ContainsKey(key))
        {
            translations.Remove(key);
            OnBeforeSerialize(); // Update serialized data
            Debug.Log($"Removed translation: Key = {key}");
        }
        else
        {
            Debug.LogWarning($"Attempted to remove a non-existing key: {key}");
        }
    }
}
