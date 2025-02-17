using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private TMP_InputField myselfInputField;    // This component - TMP_InputField
    private TMP_Text inputFieldText;            // The text component of the TMP_InputField
    private TMP_InputField.LineType inputFieldLineType; // The line type of the TMP_InputField
    private OpenVirtualKeyboard keyboardController; // Virtual Keyboard Controller in the scene

    private void Awake()
    {
        if (myselfInputField == null)
            myselfInputField = GetComponent<TMP_InputField>();

        if (inputFieldText == null)
            inputFieldText = myselfInputField.textComponent;

        inputFieldLineType = myselfInputField.lineType;

        if (keyboardController == null)
        {
            GameObject kbController = GameObject.Find("VRCanvas");
            if (kbController != null)
            {
                keyboardController = kbController.GetComponent<OpenVirtualKeyboard>();
            }
            else
            {
                Debug.LogWarning("Virtual Keyboard Controller not found in the scene.");
            }
        }
    }

    private void OnEnable()
    {
        // Re-acquire references in case they were reset.
        if (myselfInputField == null)
            myselfInputField = GetComponent<TMP_InputField>();

        if (inputFieldText == null)
            inputFieldText = myselfInputField.textComponent;

        inputFieldLineType = myselfInputField.lineType;

        if (keyboardController == null)
        {
            GameObject kbController = GameObject.Find("VRCanvas");
            if (kbController != null)
            {
                keyboardController = kbController.GetComponent<OpenVirtualKeyboard>();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log("TMP InputField OnPointerEnter");
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log("TMP InputField OnPointerExit");
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log("TMP InputField OnPointerDown");
#endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log("TMP InputField OnPointerUp");
#endif
    }

    /// <summary>
    /// Called when the pointer clicks on the input field.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the keyboard controller is assigned.
        if (keyboardController == null)
        {
            Debug.LogError("Keyboard controller is not assigned or not found in the scene.");
            return;
        }

        // Signal the virtual keyboard that the input field is active.
        keyboardController.onExitKeyboardArea = false;
        GetInputFieldTarget.SelectInputFieldName = transform.name;
#if UNITY_EDITOR
        Debug.Log("SelectInputFieldName = " + transform.name);
#endif

        // Use the eventData's camera, or fallback to Camera.main if it's null.
        Camera cam = eventData.pressEventCamera;
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("No camera available for pointer event.");
                return;
            }
        }

        // Ensure the text component is assigned.
        if (inputFieldText == null)
        {
            Debug.LogError("InputField text component is not assigned.");
            return;
        }

        // Use TMP's utility method to find the intersecting character index.
        int index = TMP_TextUtilities.FindIntersectingCharacter(inputFieldText, eventData.position, cam, true);
        if (index == -1)
        {
            // If no character was hit, set the index at the end of the text.
            index = inputFieldText.text.Length;
        }
        GetInputFieldTarget.Index = index;
#if UNITY_EDITOR
        Debug.Log("index = " + GetInputFieldTarget.Index);
#endif

        // Finally, open the virtual keyboard.
        keyboardController.OnOpenVirtualKeyboard();
    }
}
