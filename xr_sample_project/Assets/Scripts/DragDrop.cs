using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class DragDrop : MonoBehaviour
{
    [Header("Prefab Options")]
    [Tooltip("List of object prefabs that can be instantiated at the hit point.")]
    [SerializeField] private GameObject[] objectPrefabs; // Array of object prefabs

    [Header("UI Buttons")]
    [Tooltip("List of UI buttons corresponding to each prefab.")]
    [SerializeField] private Button[] buttons; // Array of buttons (order should match objectPrefabs)

    [Header("Button Colors")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color unselectedColor = Color.white;

    [Header("Raycast Settings")]
    [SerializeField] private XRRayInteractor raycastHand;
    [SerializeField] private InputActionProperty raycastInput;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform parentTransform; // Parent for instantiated objects

    // Currently selected prefab index (default is 0)
    private int selectedPrefabIndex = 0;

    private void Start()
    {
        // Assign mainCamera if not set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            Debug.Log("Main camera assigned from Camera.main.");
        }
        else
        {
            Debug.Log("Main camera assigned explicitly: " + mainCamera.name);
        }

        // Programmatically assign each button's onClick event
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i; // capture index locally for the lambda
                if (buttons[i] != null)
                {
                    buttons[i].onClick.RemoveAllListeners(); // remove any previous listeners
                    buttons[i].onClick.AddListener(() => SelectPrefab(index));
                    Debug.Log("Button " + i + " assigned to select prefab index: " + index);
                }
                else
                {
                    Debug.LogWarning("Button at index " + i + " is null.");
                }
            }
        }
        else
        {
            Debug.LogError("Buttons array is null.");
        }

        UpdateButtonColors();
        Debug.Log("RayCastObjectSelector started. Initial selected prefab index: " + selectedPrefabIndex);
    }

    private void OnEnable()
    {
        if (raycastInput != null && raycastInput.action != null)
        {
            raycastInput.action.Enable();
            Debug.Log("Raycast input enabled.");
        }
    }

    private void OnDisable()
    {
        if (raycastInput != null && raycastInput.action != null)
        {
            raycastInput.action.Disable();
            Debug.Log("Raycast input disabled.");
        }
    }

    private void Update()
    {
        // When the raycast input is pressed, perform a raycast and instantiate the selected prefab.
        if (raycastInput.action.WasPressedThisFrame())
        {
            Debug.Log("Raycast input pressed.");
            PerformRaycastAndInstantiate();
        }
    }

    /// <summary>
    /// Casts a ray from the XRRayInteractor's origin and instantiates the currently selected prefab at the hit point.
    /// Aligns the object's up direction with the hit normal.
    /// </summary>
    private void PerformRaycastAndInstantiate()
    {
        if (raycastHand == null)
        {
            Debug.LogError("Raycast hand is not assigned.");
            return;
        }
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogError("Object prefabs array is empty or null.");
            return;
        }

        Vector3 rayOrigin = raycastHand.rayOriginTransform.position;
        Vector3 rayDirection = raycastHand.rayOriginTransform.forward;
        Debug.Log($"Performing raycast from {rayOrigin} in direction {rayDirection}");

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name + " at position: " + hit.point);

            // Use the hit normal to align the object's up direction with the surface.
            Quaternion alignToSurface = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Debug.Log("Hit normal: " + hit.normal + " -- Alignment rotation: " + alignToSurface.eulerAngles);

            GameObject prefabToInstantiate = objectPrefabs[selectedPrefabIndex];
            if (prefabToInstantiate != null)
            {
                Debug.Log("Instantiating prefab: " + prefabToInstantiate.name + " at hit point.");
                // Instantiate using the alignment rotation so its up vector aligns with the surface normal.
                GameObject instantiatedObject = Instantiate(prefabToInstantiate, hit.point, alignToSurface, parentTransform);
                Debug.Log("Instantiated object: " + instantiatedObject.name + " with default scale: " + instantiatedObject.transform.localScale);
            }
            else
            {
                Debug.LogError("Prefab at index " + selectedPrefabIndex + " is null.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any object.");
        }
    }

    /// <summary>
    /// Called by a UI button to select the prefab to instantiate.
    /// </summary>
    /// <param name="index">Index corresponding to the prefab (0 to objectPrefabs.Length - 1).</param>
    public void SelectPrefab(int index)
    {
        Debug.Log("SelectPrefab called with index: " + index);

        if (index < 0 || index >= objectPrefabs.Length)
        {
            Debug.LogError("Invalid prefab index: " + index);
            return;
        }

        selectedPrefabIndex = index;
        Debug.Log("Selected prefab: " + objectPrefabs[selectedPrefabIndex].name + " at index: " + selectedPrefabIndex);
        UpdateButtonColors();
    }

    /// <summary>
    /// Updates the UI button colors so that the selected button is highlighted.
    /// </summary>
    private void UpdateButtonColors()
    {
        if (buttons == null)
        {
            Debug.LogError("Buttons array is null in UpdateButtonColors.");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                ColorBlock cb = buttons[i].colors;
                if (i == selectedPrefabIndex)
                {
                    cb.normalColor = selectedColor;
                    cb.highlightedColor = selectedColor;
                    // Immediately update the button's target graphic color.
                    buttons[i].targetGraphic.color = selectedColor;
                }
                else
                {
                    cb.normalColor = unselectedColor;
                    cb.highlightedColor = unselectedColor;
                    buttons[i].targetGraphic.color = unselectedColor;
                }
                buttons[i].colors = cb;
                Debug.Log("Button " + i + " updated. Is selected: " + (i == selectedPrefabIndex));
            }
            else
            {
                Debug.LogWarning("Button at index " + i + " is null.");
            }
        }
    }
}
