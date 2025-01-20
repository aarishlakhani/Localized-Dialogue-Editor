using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string translationKey;
    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();

        if (string.IsNullOrEmpty(translationKey))
        {
            Debug.LogWarning($"LocalizedText on GameObject '{gameObject.name}' has an empty translation key.");
        }

        UpdateText();
    }

    void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
        else
        {
            Debug.LogError($"LocalizationManager.Instance is null. Ensure it is added to the scene and initialized before enabling '{gameObject.name}'.");
        }
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void SetTranslationKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning($"Attempted to set an empty translation key on GameObject '{gameObject.name}'.");
            return;
        }

        translationKey = key;
        UpdateText();
    }

    private void UpdateText()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError($"LocalizationManager.Instance is null. Ensure it is added to the scene and initialized.");
            return;
        }

        if (textComponent == null)
        {
            Debug.LogError($"TMP_Text component is missing on GameObject '{gameObject.name}'. Ensure this script is attached to a UI element with a TMP_Text component.");
            return;
        }

        if (string.IsNullOrEmpty(translationKey))
        {
            Debug.LogWarning($"No translation key set for GameObject '{gameObject.name}'. Text will not be updated.");
            return;
        }

        string translation = LocalizationManager.Instance.GetTranslation(translationKey);
        if (translation.StartsWith("Missing translation"))
        {
            Debug.LogWarning($"Translation for key '{translationKey}' is missing in the current language.");
        }

        textComponent.text = translation;
    }
}
