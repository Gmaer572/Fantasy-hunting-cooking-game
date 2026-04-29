using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;

    [Header("Flow")]
    [SerializeField] private bool loadSceneAfterDialogue = false;
    [SerializeField] private string nextSceneName = "Room1";

    private int currentLineIndex;
    private bool finished;

    private void Start()
    {
        AutoAssignTextReferencesIfNeeded();
        currentLineIndex = 0;
        finished = false;
        ShowCurrentLine();
        UpdateHint();
    }

    private void AutoAssignTextReferencesIfNeeded()
    {
        if (dialogueText == null)
        {
            GameObject dialogueObj = GameObject.Find("DialogueText");
            if (dialogueObj != null)
            {
                dialogueText = dialogueObj.GetComponent<TextMeshProUGUI>();
            }
        }

        if (hintText == null)
        {
            GameObject hintObj = GameObject.Find("HintText");
            if (hintObj != null)
            {
                hintText = hintObj.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void Update()
    {
        if (!WasAdvancePressedThisFrame())
        {
            return;
        }

        if (finished)
        {
            if (loadSceneAfterDialogue && !string.IsNullOrWhiteSpace(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            return;
        }

        currentLineIndex++;
        if (currentLineIndex >= lines.Length)
        {
            finished = true;
            ShowEndMessage();
            UpdateHint();
            return;
        }

        ShowCurrentLine();
    }

    private bool WasAdvancePressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        if (Keyboard.current == null)
        {
            return false;
        }

        return Keyboard.current.spaceKey.wasPressedThisFrame
            || Keyboard.current.enterKey.wasPressedThisFrame
            || Keyboard.current.numpadEnterKey.wasPressedThisFrame;
    }

    private void ShowCurrentLine()
    {
        if (dialogueText == null)
        {
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            dialogueText.text = "No dialogue lines set.";
            finished = true;
            return;
        }

        dialogueText.text = lines[currentLineIndex];
    }

    private void ShowEndMessage()
    {
        if (dialogueText == null)
        {
            return;
        }

        dialogueText.text = "Dialogue finished.";
    }

    private void UpdateHint()
    {
        if (hintText == null)
        {
            return;
        }

        if (!finished)
        {
            hintText.text = "Click / Space / Enter to continue";
            return;
        }

        if (loadSceneAfterDialogue)
        {
            hintText.text = "Click / Space / Enter to continue";
        }
        else
        {
            hintText.text = "End";
        }
    }
}
