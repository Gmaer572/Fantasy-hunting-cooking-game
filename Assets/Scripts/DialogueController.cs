using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextMeshProUGUI speakerText;

    [Header("Dialogue Image")]
    [SerializeField] private Image dialogueImage;
    [SerializeField] private List<DialogueImageChange> dialogueImageChanges;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;

    [Header("Flow")]
    [SerializeField] private bool loadSceneAfterDialogue = false;
    [SerializeField] private string nextSceneName = "Room1";
    [SerializeField] private bool autoCreateUiAtRuntime = true;
    [SerializeField] private string defaultSpeakerName = "Narrator";

    private int currentLineIndex;
    private bool finished;

    [System.Serializable]
    private struct DialogueImageChange
    {
        public int lineIndex;
        public Sprite sprite;
    }

    private void Start()
    {
        if (autoCreateUiAtRuntime)
        {
            EnsureDialogueUi();
        }
        AutoAssignTextReferencesIfNeeded();
        AutoAssignImageReferencesIfNeeded();
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

        if (speakerText == null)
        {
            speakerText = FindTextByNameToken("speaker");
        }
    }

    private void AutoAssignImageReferencesIfNeeded()
    {
        if (dialogueImage == null)
        {
            dialogueImage = FindImageByNameToken("dialogue image") ?? FindImageByNameToken("dialogueimage");
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

    private Image FindImageByNameToken(string token)
    {
        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        if (allImages == null || allImages.Length == 0)
        {
            return null;
        }

        for (int i = 0; i < allImages.Length; i++)
        {
            if (allImages[i] == null)
            {
                continue;
            }

            string objName = allImages[i].gameObject.name;
            if (!string.IsNullOrEmpty(objName) && objName.ToLower().Contains(token))
            {
                return allImages[i];
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

        GameObject speakerObj = new GameObject("SpeakerText", typeof(RectTransform), typeof(TextMeshProUGUI));
        speakerObj.transform.SetParent(boxObj.transform, false);
        RectTransform speakerRect = speakerObj.GetComponent<RectTransform>();
        speakerRect.anchorMin = new Vector2(0f, 1f);
        speakerRect.anchorMax = new Vector2(0f, 1f);
        speakerRect.pivot = new Vector2(0f, 1f);
        speakerRect.anchoredPosition = new Vector2(30f, -10f);
        speakerRect.sizeDelta = new Vector2(500f, 48f);

        TextMeshProUGUI speakerTmp = speakerObj.GetComponent<TextMeshProUGUI>();
        speakerTmp.fontSize = 30f;
        speakerTmp.alignment = TextAlignmentOptions.TopLeft;
        speakerTmp.text = defaultSpeakerName;

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

        if (GameObject.Find("DialogueImage") == null && dialogueImage == null) // Made by Sam Jackson with help from VisualStudio AI Code
        {
            GameObject imageObj = new GameObject("DialogueImage", typeof(RectTransform), typeof(Image));
            imageObj.transform.SetParent(canvas.transform, false);
            RectTransform imageRect = imageObj.GetComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(1f, 1f);
            imageRect.anchorMax = new Vector2(1f, 1f);
            imageRect.pivot = new Vector2(1f, 1f);
            imageRect.anchoredPosition = new Vector2(-40f, -40f);
            imageRect.sizeDelta = new Vector2(280f, 280f);

            Image imageUi = imageObj.GetComponent<Image>();
            imageUi.color = new Color(1f, 1f, 1f, 0f);
            imageUi.raycastTarget = false;
            dialogueImage = imageUi;
        }
    } 

    [ContextMenu("Build Dialogue UI In Scene")]
    private void BuildDialogueUiInScene()
    {
        EnsureDialogueUi();
        AutoAssignTextReferencesIfNeeded();
        AutoAssignImageReferencesIfNeeded();
    }

    private void Update()
    {
        if (!WasAdvancePressedThisFrame())
        {
            return;
        }

        if (finished)
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnDialogueFinished();
                return;
            }

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

        string rawLine = lines[currentLineIndex] ?? string.Empty;
        string speaker = defaultSpeakerName;
        string content = rawLine;

        int splitIndex = rawLine.IndexOf(':');
        if (splitIndex > 0)
        {
            string parsedSpeaker = rawLine.Substring(0, splitIndex).Trim();
            string parsedContent = rawLine.Substring(splitIndex + 1).Trim();

            if (!string.IsNullOrWhiteSpace(parsedSpeaker))
            {
                speaker = parsedSpeaker;
            }

            content = parsedContent;
        }

        dialogueText.text = content;
        if (speakerText != null)
        {
            speakerText.text = speaker;
        }

        ApplyDialogueImageForLine(currentLineIndex);
    }

    private void ApplyDialogueImageForLine(int lineIndex)
    {
        if (dialogueImage == null || dialogueImageChanges == null || dialogueImageChanges.Count == 0)
        {
            return;
        }

        for (int i = 0; i < dialogueImageChanges.Count; i++)
        {
            if (dialogueImageChanges[i].lineIndex != lineIndex)
            {
                continue;
            }

            Sprite newSprite = dialogueImageChanges[i].sprite;
            dialogueImage.sprite = newSprite;
            dialogueImage.color = newSprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
            dialogueImage.enabled = newSprite != null;
            return;
        }
    }

    public void SetDialogueImageForLine(int lineIndex, Sprite sprite)
    {
        if (lineIndex < 0)
        {
            return;
        }

        if (dialogueImageChanges == null)
        {
            dialogueImageChanges = new List<DialogueImageChange>();
        }

        int existingIndex = dialogueImageChanges.FindIndex(x => x.lineIndex == lineIndex);
        DialogueImageChange change = new DialogueImageChange { lineIndex = lineIndex, sprite = sprite };

        if (existingIndex >= 0)
        {
            dialogueImageChanges[existingIndex] = change;
        }
        else
        {
            dialogueImageChanges.Add(change);
        }

        if (currentLineIndex == lineIndex)
        {
            ApplyDialogueImageForLine(lineIndex);
        }
    }

    public void ChangeCanvasImageAfterLine(int lineIndex, Sprite newSprite)
    {
        SetDialogueImageForLine(lineIndex, newSprite);
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

        if (DayManager.Instance != null)
        {
            hintText.text = "Click / Space / Enter to start day";
            return;
        }

        if (loadSceneAfterDialogue)
        {
            hintText.text = "Click / Space / Enter to start day";
        }
        else
        {
            hintText.text = "End";
        }
    }
}
