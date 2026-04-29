using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        EnsureDialogueUi();
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
            dialogueText = FindTextByNameToken("dialogue");
        }

        if (hintText == null)
        {
            hintText = FindTextByNameToken("hint");
        }
    }

    private TextMeshProUGUI FindTextByNameToken(string token)
    {
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        if (allTexts == null || allTexts.Length == 0)
        {
            return null;
        }

        for (int i = 0; i < allTexts.Length; i++)
        {
            if (allTexts[i] == null)
            {
                continue;
            }

            string objName = allTexts[i].gameObject.name;
            if (!string.IsNullOrEmpty(objName) && objName.ToLower().Contains(token))
            {
                return allTexts[i];
            }
        }

        return null;
    }

    private void EnsureDialogueUi()
    {
        if (GameObject.Find("DialogueBox") != null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        GameObject boxObj = new GameObject("DialogueBox", typeof(RectTransform), typeof(Image));
        boxObj.transform.SetParent(canvas.transform, false);
        RectTransform boxRect = boxObj.GetComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0f, 0f);
        boxRect.anchorMax = new Vector2(1f, 0f);
        boxRect.pivot = new Vector2(0.5f, 0f);
        boxRect.offsetMin = new Vector2(40f, 30f);
        boxRect.offsetMax = new Vector2(-40f, 250f);

        Image boxImage = boxObj.GetComponent<Image>();
        boxImage.color = new Color(0.95f, 0.92f, 0.86f, 0.95f);

        GameObject dialogueObj = new GameObject("DialogueText", typeof(RectTransform), typeof(TextMeshProUGUI));
        dialogueObj.transform.SetParent(boxObj.transform, false);
        RectTransform dialogueRect = dialogueObj.GetComponent<RectTransform>();
        dialogueRect.anchorMin = new Vector2(0f, 0f);
        dialogueRect.anchorMax = new Vector2(1f, 1f);
        dialogueRect.offsetMin = new Vector2(30f, 60f);
        dialogueRect.offsetMax = new Vector2(-30f, -25f);

        TextMeshProUGUI dialogueTmp = dialogueObj.GetComponent<TextMeshProUGUI>();
        dialogueTmp.fontSize = 46f;
        dialogueTmp.alignment = TextAlignmentOptions.TopLeft;
        dialogueTmp.enableWordWrapping = true;
        dialogueTmp.text = string.Empty;

        GameObject hintObj = new GameObject("HintText", typeof(RectTransform), typeof(TextMeshProUGUI));
        hintObj.transform.SetParent(boxObj.transform, false);
        RectTransform hintRect = hintObj.GetComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(1f, 0f);
        hintRect.anchorMax = new Vector2(1f, 0f);
        hintRect.pivot = new Vector2(1f, 0f);
        hintRect.anchoredPosition = new Vector2(-20f, 12f);
        hintRect.sizeDelta = new Vector2(520f, 52f);

        TextMeshProUGUI hintTmp = hintObj.GetComponent<TextMeshProUGUI>();
        hintTmp.fontSize = 30f;
        hintTmp.alignment = TextAlignmentOptions.BottomRight;
        hintTmp.text = "Space / Enter to continue";
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
