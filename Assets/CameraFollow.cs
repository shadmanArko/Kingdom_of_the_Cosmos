using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;
    
    [Header("Dead Zone Settings")]
    [SerializeField] private float deadZoneWidth = 2f;
    [SerializeField] private float deadZoneHeight = 2f;
    
    [Header("Look Ahead Settings")]
    [SerializeField] private float lookAheadFactor = 1.5f;
    [SerializeField] private float lookAheadReturnSpeed = 2f;
    [SerializeField] private float lookAheadMoveThreshold = 0.1f;

    private Vector3 lastTargetPosition;
    private Vector3 currentVelocity;
    private Vector3 lookAheadPos;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to camera! Please assign the player transform.");
            return;
        }
        
        lastTargetPosition = target.position;
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate target movement delta
        Vector3 moveDelta = (target.position - lastTargetPosition).normalized;
        lastTargetPosition = target.position;

        // Calculate look ahead based on movement direction
        if (moveDelta.magnitude > lookAheadMoveThreshold)
        {
            // Now the lookAheadPos is in the same direction as movement
            lookAheadPos = Vector3.Lerp(lookAheadPos, moveDelta * lookAheadFactor, Time.deltaTime * lookAheadReturnSpeed);
        }
        else
        {
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, lookAheadReturnSpeed * Time.deltaTime);
        }

        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        // Check if target is outside dead zone
        float deltaX = targetPosition.x - currentPosition.x;
        float deltaY = targetPosition.y - currentPosition.y;

        Vector3 desiredPosition = transform.position;

        // Only adjust X position if outside dead zone
        if (Mathf.Abs(deltaX) > deadZoneWidth / 2)
        {
            float xMovement = deltaX - (deltaX > 0 ? deadZoneWidth / 2 : -deadZoneWidth / 2);
            desiredPosition.x += xMovement;
        }

        // Only adjust Y position if outside dead zone
        if (Mathf.Abs(deltaY) > deadZoneHeight / 2)
        {
            float yMovement = deltaY - (deltaY > 0 ? deadZoneHeight / 2 : -deadZoneHeight / 2);
            desiredPosition.y += yMovement;
        }

        // Add look ahead position
        desiredPosition += lookAheadPos;

        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1 / followSpeed);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10); // Maintain camera Z position
    }

    private void OnDrawGizmos()
    {
        // Draw dead zone in editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneWidth, deadZoneHeight, 0));
    }
}