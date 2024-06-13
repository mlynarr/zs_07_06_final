using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMove : MonoBehaviour
{
    public float movementSpeed = 5.0f; // Normalna prêdkoœæ poruszania siê
    public float sprintMultiplier = 2.0f; // Mno¿nik prêdkoœci podczas sprintu
    public Camera playerCamera; // Kamera œledz¹ca postaæ
    public float jumpForce = 8f; // Si³a skoku
    public float energyConsumptionRate = 20f;
    public float energyRecoveryRate = 20f;
    public float energy = 100f;


    public float turnSpeed = 5f; // Szybkoœæ obracania siê postaci
    public int maxJumps = 2; // Maksymalna liczba skoków zanim postaæ musi dotkn¹æ ziemi

    private Rigidbody rb;
    private Vector3 movement;
    private bool isGrounded;
    private int jumpCount = 0; // Aktualna liczba wykonanych skoków

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // U¿yj kierunków kamery do okreœlenia kierunku ruchu
        Vector3 forwardMovement = playerCamera.transform.forward * verticalInput;
        Vector3 rightMovement = playerCamera.transform.right * horizontalInput;

        // Nie uwzglêdniaj ruchu w osi y (w górê/dó³)
        forwardMovement.y = 0;
        rightMovement.y = 0;

        movement = (forwardMovement + rightMovement).normalized;

        HandleSprinting();
        HandleRotation();
        HandleJump();
    }

    void FixedUpdate()
    {
        if (movement.magnitude > 0)
        {
            rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleSprinting()
    {
        // SprawdŸ, czy u¿ytkownik naciska klawisz 'Shift' dla sprintu
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if(energy > 0)
            {
                movement *= sprintMultiplier; // Zwiêksz prêdkoœæ o mno¿nik sprintu
                energy -= (energyConsumptionRate * Time.deltaTime);
                energy = Mathf.Max(energy, 0);
            }    
        }
        else
        {
            energy += (energyRecoveryRate * Time.deltaTime);
            energy = Mathf.Min(energy, 100);
        }
        Debug.Log("energia: " + energy);
    }

    void HandleRotation()
    {
        if (movement.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < maxJumps))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Resetowanie prêdkoœci y, aby skok by³ zawsze taki sam
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Resetowanie licznika skoków po dotkniêciu ziemi
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
