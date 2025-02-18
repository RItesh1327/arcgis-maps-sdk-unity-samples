using UnityEngine;

public class CompassController : MonoBehaviour
{
    [Tooltip("The transform of the player or camera. The compass will rotate inversely to this transform's yaw.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Time (in seconds) to smooth the rotation.")]
    [SerializeField] private float smoothTime = 0.2f;

    // Store the initial local Z rotation (assumed to represent north).
    private float initialZAngle;

    // This variable will be modified by SmoothDampAngle to track velocity.
    private float smoothVelocity = 0f;

    private void Start()
    {
        // If no playerTransform is assigned, default to the main camera.
        if (playerTransform == null && Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }

        // Record the initial local Z rotation as the north reference.
        initialZAngle = transform.localEulerAngles.z;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Get the player's current yaw (rotation around the Y-axis).
            float playerYaw = playerTransform.eulerAngles.y;

            // Calculate the target Z angle for the compass.
            // When the player rotates to the right (increasing yaw), the compass rotates in the opposite direction.
            float targetZAngle = initialZAngle - playerYaw;

            // Smoothly damp the current Z angle toward the target Z angle.
            float currentZAngle = transform.localEulerAngles.z;
            float newZAngle = Mathf.SmoothDampAngle(currentZAngle, targetZAngle, ref smoothVelocity, smoothTime);

            // Update the local Euler angles (preserving X and Y, changing Z).
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newZAngle);
        }
    }
}
