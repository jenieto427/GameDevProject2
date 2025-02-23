using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RigidController : MonoBehaviour
{
    public float speed = 8.0f;
    public float jumpForce = 100.0f;
    public Transform cameraTransform; // Assign your Camera's Transform here in the Inspector
    public float sprintMultiplier = 1.375f; // Multiplier to apply to speed when sprinting
    public float doublePressTime = 0.25f; // Time frame for double press
    public float sprintDuration = 3.0f; // Duration of the sprint in seconds
    public LayerMask groundLayer; // Set this in the Inspector to match your terrain's layer

    private Rigidbody rb;
    private PlayerAudioManager playerAudioManager;
    private float lastWPressTime = -1f; // Track the last time "W" was pressed
    private bool wasAirborne = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAudioManager = GameObject.Find("PlayerAudioManager").GetComponent<PlayerAudioManager>();
    }

    void Update()
    {
        Jump();

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime < doublePressTime)
            {
                // Detected a double press, start sprinting
                StartCoroutine(Sprint());
            }
            lastWPressTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        MoveCharacter();
        CheckLanding();
    }

    IEnumerator Sprint()
    {
        float originalSpeed = speed;
        //speed *= sprintMultiplier; // Increase the speed

        // Wait for the duration of the sprint
        yield return new WaitForSeconds(sprintDuration);

        speed = originalSpeed; // Reset to original speed
    }

    void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = (forward * vertical + right * horizontal).normalized;
        if (direction.magnitude > 0 && IsGrounded()) // Only play sound if there is movement
        {
            playerAudioManager.PlayWalkingSound(); // Call to play walking sound
        }
        Vector3 movement = direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }

    void Jump()
    {
        bool isGrounded = IsGrounded();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            if (SceneManager.GetActiveScene().name == "MapGenerationTest")
            { rb.AddForce(Vector3.up * jumpForce * 2, ForceMode.Impulse); }
            else { rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); }
            playerAudioManager.StopWalkingSound();
            wasAirborne = true;
        }
    }

    bool IsGrounded()
    {
        float rayLength = 2.0f; // distance from floor mesh collision
        Vector3 rayStart = transform.position + Vector3.up * 0.1f; // Start slightly above the player's pivot

        bool hasHit = Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayLength, groundLayer);
        Debug.DrawRay(rayStart, Vector3.down * rayLength, Color.blue, 1f); // Now with a duration of 1 second and a more visible color.
        return hasHit;
    }
    void CheckLanding()
    {
        if (wasAirborne && IsGrounded())
        {
            playerAudioManager.PlayWalkingSound(); // Play walking sound when landing
            wasAirborne = false;
        }
        else if (!IsGrounded())
        {
            wasAirborne = true; // Set wasAirborne to true when the character is not grounded
        }
    }
}
