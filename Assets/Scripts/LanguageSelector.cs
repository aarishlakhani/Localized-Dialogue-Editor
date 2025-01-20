using UnityEngine;
using TMPro; // For the TMP_Dropdown component

public class LanguageSelector : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown languageDropdown; // Assign this in the Inspector

    private void Start()
    {
        // Clear existing options in the dropdown
        languageDropdown.ClearOptions();

        // Populate the dropdown with available languages
        var languages = LocalizationManager.Instance.GetAvailableLanguages();
        foreach (var language in languages)
        {
            languageDropdown.options.Add(new TMP_Dropdown.OptionData(language.languageName));
        }

        // Add a listener for dropdown value changes
        languageDropdown.onValueChanged.AddListener(OnLanguageSelected);

        // Set the dropdown to the currently selected language
        var currentLanguage = LocalizationManager.Instance.GetCurrentLanguage();
        int currentIndex = System.Array.FindIndex(languages, l => l == currentLanguage);
        if (currentIndex >= 0)
        {
            languageDropdown.SetValueWithoutNotify(currentIndex);
        }
    }

    private void OnLanguageSelected(int index)
    {
        // Get the selected language and set it in the LocalizationManager
        var languages = LocalizationManager.Instance.GetAvailableLanguages();
        if (index >= 0 && index < languages.Length)
        {
            LocalizationManager.Instance.SetLanguage(languages[index]);
        }
    }
}