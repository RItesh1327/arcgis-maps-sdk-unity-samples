using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GetKeyCode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Color32 mNormalColor = Color.white;
    public Color32 mHoverColor = Color.gray;
    public Color32 mDownColor = Color.red;

    private OpenVirtualKeyboard keyboardController; // Virtual keyboard controller
    private Image buttonImage;                      // The button's image component
    private string buttonString;                    // The key's string (used as input)
    private Text showString;                        // The text component displaying the key

    // Use TMP_InputField instead of InputField for the text entry target.
    private TMP_InputField inputTarget;

    private bool toLowLetterCase;                   // Tracks the letter case status
    private readonly CultureInfo cult = new CultureInfo("en-US", false);

    private void OnEnable()
    {
        if (keyboardController == null)
            keyboardController = GameObject.Find("VRCanvas").GetComponent<OpenVirtualKeyboard>();

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (string.IsNullOrEmpty(buttonString))
            buttonString = transform.name;

        if (showString == null)
            showString = transform.Find("Text").GetComponent<Text>();
    }

    private void Update()
    {
        // Only update if the letter case state has changed.
        if (toLowLetterCase == LetterCaseDetection.Lowercase)
            return;

        toLowLetterCase = LetterCaseDetection.Lowercase;

        // If the button string is alphanumeric (and not one of the special keys), adjust its case.
        if (Regex.IsMatch(buttonString, "^[a-zA-Z0-9]*$") &&
            !(string.Equals(buttonString, "delete") || string.Equals(buttonString, "clear") ||
              string.Equals(buttonString, "backward") || string.Equals(buttonString, "forward") ||
              string.Equals(buttonString, "Letter case") || string.Equals(buttonString, "To0") ||
              string.Equals(buttonString, "ToLast")))
        {
            buttonString = toLowLetterCase ? buttonString.ToLower(cult) : buttonString.ToUpper(cult);
            showString.text = buttonString;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (keyboardController != null)
            keyboardController.onExitKeyboardArea = false;
        if (buttonImage != null)
            buttonImage.color = mHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (keyboardController != null)
            keyboardController.onExitKeyboardArea = false;
        if (buttonImage != null)
            buttonImage.color = mNormalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = mDownColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Optionally, you can add behavior here on pointer up.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (keyboardController != null)
            keyboardController.onExitKeyboardArea = false;
        if (buttonImage != null)
            buttonImage.color = mHoverColor;

        // Retrieve the target input field's name and the current index from your static class.
        string target = GetInputFieldTarget.SelectInputFieldName;
        int index = GetInputFieldTarget.Index;

        // Attempt to locate the TMP_InputField if it's not already assigned.
        if (inputTarget == null)
        {
            GameObject inputObj = GameObject.Find(target);
            if (inputObj != null)
            {
                inputTarget = inputObj.GetComponent<TMP_InputField>();
                if (inputTarget == null)
                {
                    //              Debug.LogError("The GameObject " + target + " does not have a TMP_InputField component.");
                    return;
                }
            }
            else
            {
                //         Debug.LogError("Could not find GameObject with name: " + target);
                return;
            }
        }

        // If the current target's name doesn't match, reassign the input target.
        if (inputTarget.gameObject.name != target)
        {
#if UNITY_EDITOR
            //        Debug.Log("Changing target");
#endif
            GameObject inputObj = GameObject.Find(target);
            if (inputObj != null)
            {
                inputTarget = inputObj.GetComponent<TMP_InputField>();
                if (inputTarget == null)
                {
                    //          Debug.LogError("The GameObject " + target + " does not have a TMP_InputField component.");
                    return;
                }
            }
            else
            {
                //         Debug.LogError("Could not find GameObject with name: " + target);
                return;
            }
        }

#if UNITY_EDITOR
        //     Debug.Log("You clicked: " + buttonString);
        //    Debug.Log("Your input target is: " + inputTarget.gameObject.name);
        //     Debug.Log("Your input index is: " + index);
#endif

        string targetText = inputTarget.text;

        // For normal character input:
        if (!(string.Equals(buttonString, "delete") || string.Equals(buttonString, "clear") ||
              string.Equals(buttonString, "backward") || string.Equals(buttonString, "forward") ||
              string.Equals(buttonString, "Letter case") || string.Equals(buttonString, "To0") ||
              string.Equals(buttonString, "ToLast")))
        {
            // Clamp index to a valid range
            index = Mathf.Clamp(index, 0, targetText.Length);
            inputTarget.text = targetText.Insert(index, buttonString);
            // Update index to be after the newly inserted text
            GetInputFieldTarget.Index = index + buttonString.Length;
#if UNITY_EDITOR
            //      Debug.Log("Updated inputTarget.text = " + inputTarget.text);
#endif
        }
        else
        {
            // Handle special commands.
            switch (buttonString)
            {
                case "delete":
                    if (!string.IsNullOrEmpty(targetText) && GetInputFieldTarget.Index > 0)
                    {
                        // Clamp index so it is within [1, targetText.Length]
                        GetInputFieldTarget.Index = Mathf.Clamp(GetInputFieldTarget.Index, 1, targetText.Length);
                        int removeIndex = GetInputFieldTarget.Index - 1;
                        inputTarget.text = targetText.Remove(removeIndex, 1);
                        GetInputFieldTarget.Index = removeIndex;
                    }
#if UNITY_EDITOR
                    //           Debug.Log("After delete: inputTarget.text.Length = " + inputTarget.text.Length);
                    //           Debug.Log("After delete: GetInputFieldTarget.Index = " + GetInputFieldTarget.Index);
#endif
                    break;
                case "clear":
                    if (targetText.Length > 0)
                    {
                        inputTarget.text = string.Empty;
                        GetInputFieldTarget.Index = 0;
                    }
#if UNITY_EDITOR
                    //        Debug.Log("After clear: inputTarget.text.Length = " + inputTarget.text.Length);
                    //       Debug.Log("After clear: GetInputFieldTarget.Index = " + GetInputFieldTarget.Index);
#endif
                    break;
                case "backward":
                    if (GetInputFieldTarget.Index > 0)
                        GetInputFieldTarget.Index--;
#if UNITY_EDITOR
                    //     Debug.Log("After backward: GetInputFieldTarget.Index = " + GetInputFieldTarget.Index);
#endif
                    break;
                case "forward":
                    if (GetInputFieldTarget.Index < targetText.Length)
                        GetInputFieldTarget.Index++;
#if UNITY_EDITOR
                    //      Debug.Log("After forward: GetInputFieldTarget.Index = " + GetInputFieldTarget.Index);
#endif
                    break;
                case "Letter case":
                    LetterCaseDetection.Lowercase = !LetterCaseDetection.Lowercase;
#if UNITY_EDITOR
                    //    Debug.Log("LetterCaseDetection.Lowercase = " + LetterCaseDetection.Lowercase);
#endif
                    break;
                case "ToLast":
                    GetInputFieldTarget.Index = targetText.Length;
#if UNITY_EDITOR
                    //        Debug.Log("After ToLast: GetInputFieldTarget.Index = " + GetInputFieldTarget.Index);
#endif
                    break;
                case "To0":
                    GetInputFieldTarget.Index = 0;
#if UNITY_EDITOR
                    //      Debug.Log("After To0: GetInputFieldTarget.Index = " + GetInputFieldTarget.Index);
#endif
                    break;
            }
        }
    }
}
