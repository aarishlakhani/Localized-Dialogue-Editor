using UnityEngine;
using System.Collections.Generic;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField]
    private Language[] availableLanguages; // Array of available languages

    private Language currentLanguage; // The currently selected language
    public event Action OnLanguageChanged; // Event to notify when the language changes

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the manager persists across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (availableLanguages != null && availableLanguages.Length > 0)
        {
            SetLanguage(availableLanguages[0]); // Set the default language
        }
        else
        {
            Debug.LogError("No available languages assigned in the LocalizationManager.");
        }
    }

    /// <summary>
    /// Sets the current language and triggers the OnLanguageChanged event.
    /// </summary>
    public void SetLanguage(Language language)
    {
        if (language == null)
        {
            Debug.LogError("Attempted to set a null language in LocalizationManager.");
            return;
        }

        // Force deserialization to ensure the language is fully loaded
        language.OnAfterDeserialize();

        currentLanguage = language;

        Debug.Log($"Language set to: {currentLanguage.languageName}");

        // Debug translations
        foreach (var translation in currentLanguage.translations)
        {
            Debug.Log($"Key: {translation.Key}, Value: {translation.Value}");
        }

        OnLanguageChanged?.Invoke(); // Notify listeners that the language has changed
    }

    /// <summary>
    /// Retrieves the localized text for a given key.
    /// </summary>
    public string GetTranslation(string key)
    {
        if (currentLanguage == null)
        {
            Debug.LogError("No language is currently set in LocalizationManager.");
            return $"Missing translation: {key}";
        }

        if (!currentLanguage.translations.ContainsKey(key))
        {
            Debug.LogWarning($"Translation for key '{key}' is missing in the current language '{currentLanguage.languageName}'.");
            return $"Missing translation: {key}";
        }

        return currentLanguage.translations[key];
    }

    /// <summary>
    /// Returns the array of available languages.
    /// </summary>
    public Language[] GetAvailableLanguages()
    {
        return availableLanguages;
    }

    /// <summary>
    /// Returns the currently selected language.
    /// </summary>
    public Language GetCurrentLanguage()
    {
        return currentLanguage;
    }

    /// <summary>
    /// Retrieves localized text for a key with a fallback option.
    /// </summary>
    public string GetTranslationOrDefault(string key, string fallback)
    {
        if (currentLanguage != null && currentLanguage.translations.ContainsKey(key))
        {
            return currentLanguage.translations[key];
        }

        Debug.LogWarning($"Key '{key}' not found. Using fallback: '{fallback}'");
        return fallback;
    }
}
