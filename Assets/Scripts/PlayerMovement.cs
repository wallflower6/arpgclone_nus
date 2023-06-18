using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputAction mouseClickAction;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private LayerMask walkableLayerMask;
    [SerializeField] private GameObject marker;

    private Camera mainCamera;
    private NavMeshAgent agent;
    private Animator playerAnim;
    private Vector3 targetPosition;

    private bool held;

    private void Awake() 
    {
        mainCamera = Camera.main;
        playerAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() 
    {
        // Player starts at spawn point
        transform.position = spawnPoint.transform.position;
    }

    private void Update() 
    {
        // Continuously move if right mouse button is held down, otherwise stop moving
        if (held) 
        {
            Move();
        } else 
        {
            StopMove();
        }
    }

    // Enable and subscribe to events
    private void OnEnable() 
    {
        mouseClickAction.Enable();
        mouseClickAction.performed += Held;
        mouseClickAction.canceled += Released;
    }

    // Unsubscribe from events and disable
    private void OnDisable() 
    {
        mouseClickAction.performed -= Held;
        mouseClickAction.canceled -= Released;
        mouseClickAction.Disable();
    }

    // Called when player presses and / or holds down right mouse button
    private void Held(InputAction.CallbackContext ctx) 
    {
        held = true;
    }

    // Called when player releases right mouse button
    private void Released(InputAction.CallbackContext ctx) 
    {
        held = false;
    }

    private void Move() 
    {
        // Get point on walkable layer that was clicked
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit, walkableLayerMask)) {
            targetPosition = hit.point;
            MoveToPoint(hit.point);
        }
    }

    private void MoveToPoint(Vector3 point) 
    {
        // Optional marker to indicate where player has clicked
        marker.SetActive(true);
        marker.transform.position = point;

        // Set player's NavMeshAgent's destination to clicked point, transition to run animation
        agent.SetDestination(point);
        playerAnim.SetBool("isMoving", true);
    }

    private void StopMove() 
    {
        // If player's NavMeshAgent is already near destination, transition to idle animation, remove click marker (indicator)
        if(agent.remainingDistance < 0.01f) {
            playerAnim.SetBool("isMoving", false);
            marker.SetActive(false);
        }
    }

    private void OnDrawGizmos() 
    {
        // For debug purposes in Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 1);
    }
}
