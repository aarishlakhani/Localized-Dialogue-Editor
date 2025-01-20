using System.Collections;
using UnityEngine;
using TMPro;
using XNode;
using UnityEngine.UI;

public class NodeParser : MonoBehaviour
{
    public DialogueGraph graph;     // The dialogue graph
    public TMP_Text dialogue;       // UI element for the dialogue text
    public Button choiceA;          // Button for Choice A
    public Button choiceB;          // Button for Choice B
    public Button choiceC;          // Button for Choice C (New)
    public Button choiceD;          // Button for Choice D (New)
    public TMP_Text choiceAText;    // Text for Choice A button
    public TMP_Text choiceBText;    // Text for Choice B button
    public TMP_Text choiceCText;    // Text for Choice C button (New)
    public TMP_Text choiceDText;    // Text for Choice D button (New)

    Coroutine _parser;              // To manage the ParseNode coroutine

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += UpdateLocalizedUI;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateLocalizedUI;
    }

    private void Start()
    {
        foreach (BaseNode node in graph.nodes)
        {
            if (node.GetString() == "Start")
            {
                graph.current = node;
                break;
            }
        }

        if (graph.current != null)
        {
            _parser = StartCoroutine(ParseNode());
        }
        else
        {
            Debug.LogError("No Start Node found in the Dialogue Graph!");
        }
    }

    IEnumerator ParseNode()
    {
        while (graph.current != null)
        {
            BaseNode node = graph.current;

            if (node == null)
            {
                Debug.LogError("Current node is null. Stopping the dialogue parsing.");
                yield break;
            }

            string data = node.GetString();
            Debug.Log("Processing node: " + node.name + ", Data: " + data);

            string[] dataParts = data.Split('/');

            if (dataParts[0] == "Start")
            {
                Debug.Log("Start Node reached. Moving to the next node...");
                NextNode("exit");
            }
            else if (dataParts[0] == "DialogueNode")
            {
                string dialogueKey = dataParts[1];
                string choiceAKey = dataParts[2];
                string choiceBKey = dataParts[3];
                string choiceCKey = dataParts[4];  // Fetch key for Choice C
                string choiceDKey = dataParts[5];  // Fetch key for Choice D

                // Fetch localized text
                string localizedDialogue = LocalizationManager.Instance.GetTranslation(dialogueKey);
                string localizedChoiceA = LocalizationManager.Instance.GetTranslation(choiceAKey);
                string localizedChoiceB = LocalizationManager.Instance.GetTranslation(choiceBKey);
                string localizedChoiceC = LocalizationManager.Instance.GetTranslation(choiceCKey);  // Localized text for Choice C
                string localizedChoiceD = LocalizationManager.Instance.GetTranslation(choiceDKey);  // Localized text for Choice D

                // Update UI
                if (dialogue != null)
                {
                    dialogue.text = localizedDialogue;
                }
                else
                {
                    Debug.LogError("Dialogue TMP_Text is not assigned!");
                }

                choiceAText.text = localizedChoiceA;
                choiceBText.text = localizedChoiceB;
                choiceCText.text = localizedChoiceC;  // Update text for Choice C
                choiceDText.text = localizedChoiceD;  // Update text for Choice D

                // Wait for user choice
                choiceA.onClick.RemoveAllListeners();
                choiceB.onClick.RemoveAllListeners();
                choiceC.onClick.RemoveAllListeners();  // Clear listeners for Choice C
                choiceD.onClick.RemoveAllListeners();  // Clear listeners for Choice D

                choiceA.onClick.AddListener(() => { NextNode("choiceA"); });
                choiceB.onClick.AddListener(() => { NextNode("choiceB"); });
                choiceC.onClick.AddListener(() => { NextNode("choiceC"); });  // Listener for Choice C
                choiceD.onClick.AddListener(() => { NextNode("choiceD"); });  // Listener for Choice D

                yield break; // Exit coroutine to wait for button interaction
            }
            else
            {
                Debug.LogError("Unknown node type: " + dataParts[0]);
                yield break;
            }
        }
    }

    public void UpdateLocalizedUI()
    {
        if (graph.current is DialogueNode currentNode)
        {
            dialogue.text = LocalizationManager.Instance.GetTranslation(currentNode.dialogueKey);
            choiceAText.text = LocalizationManager.Instance.GetTranslation(currentNode.choiceAKey);
            choiceBText.text = LocalizationManager.Instance.GetTranslation(currentNode.choiceBKey);
            choiceCText.text = LocalizationManager.Instance.GetTranslation(currentNode.choiceCKey);  // Update localized text for Choice C
            choiceDText.text = LocalizationManager.Instance.GetTranslation(currentNode.choiceDKey);  // Update localized text for Choice D
        }
    }

    public void NextNode(string fieldName)
    {
        if (_parser != null)
        {
            StopCoroutine(_parser);
        }

        if (graph.current == null)
        {
            Debug.LogError("Current node is null. Cannot find the next node.");
            return;
        }

        foreach (NodePort port in graph.current.Outputs)
        {
            if (port.fieldName == fieldName && port.IsConnected)
            {
                graph.current = port.Connection.node as BaseNode;
                Debug.Log("Transitioned to next node: " + graph.current.name);
                break;
            }
        }

        if (graph.current != null)
        {
            _parser = StartCoroutine(ParseNode());
        }
        else
        {
            Debug.LogError("No valid connection found for field: " + fieldName);
        }
    }
}
