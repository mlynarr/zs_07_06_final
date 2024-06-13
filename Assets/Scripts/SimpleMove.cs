using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMove : MonoBehaviour
{
    public float movementSpeed = 5.0f; // Normalna pr�dko�� poruszania si�
    public float sprintMultiplier = 2.0f; // Mno�nik pr�dko�ci podczas sprintu
    public Camera playerCamera; // Kamera �ledz�ca posta�
    public float jumpForce = 8f; // Si�a skoku
    public float energyConsumptionRate = 20f;
    public float energyRecoveryRate = 20f;
    public float energy = 100f;


    public float turnSpeed = 5f; // Szybko�� obracania si� postaci
    public int maxJumps = 2; // Maksymalna liczba skok�w zanim posta� musi dotkn�� ziemi

    private Rigidbody rb;
    private Vector3 movement;
    private bool isGrounded;
    private int jumpCount = 0; // Aktualna liczba wykonanych skok�w

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // U�yj kierunk�w kamery do okre�lenia kierunku ruchu
        Vector3 forwardMovement = playerCamera.transform.forward * verticalInput;
        Vector3 rightMovement = playerCamera.transform.right * horizontalInput;

        // Nie uwzgl�dniaj ruchu w osi y (w g�r�/d�)
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
        // Sprawd�, czy u�ytkownik naciska klawisz 'Shift' dla sprintu
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if(energy > 0)
            {
                movement *= sprintMultiplier; // Zwi�ksz pr�dko�� o mno�nik sprintu
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
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Resetowanie pr�dko�ci y, aby skok by� zawsze taki sam
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Resetowanie licznika skok�w po dotkni�ciu ziemi
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
