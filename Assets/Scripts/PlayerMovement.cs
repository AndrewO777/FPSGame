using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float speed = 12f;
    public int hp = 100;
    Vector3 velocity;
    bool isGrounded;
    public float gravity = -9.81f;
    public ulong myID;
    public float jumpHeight = 3f;
    public Transform weapon;
    Vector3 weaponOrigin;
    public PlayerSpawner spawner;
    Vector3 weaponBobPosition;
    float idleCounter;
    float movementCounter;
    public Animator animator;
    public bool isMoving;
    public float runSpeedMultiplier = 1.5f; // Added runSpeedMultiplier with default value
    public float MovementSpeed; // Added MovementSpeed field

    private NetworkVariable<int> playerHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> isMovingNetworkVar = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        playerHealth.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; Player's Health: " + playerHealth.Value);
        };
    }

    void Start()
    {
        if (IsOwner)
        {
            weaponOrigin = weapon.localPosition;
            //animator = GetComponent<Animator>();

        }
        spawner = GameObject.Find("SpawnPoints").GetComponent<PlayerSpawner>();
        myID = OwnerClientId;
    }

private void Update()
{
    if (!IsOwner)
    {
        // Update the isMoving variable from the networked variable
        isMoving = isMovingNetworkVar.Value;

        // Enable or disable the Animator component based on the isMoving variable
        animator.enabled = isMoving;
        return;
    }

    // Update the player's health (for testing purposes)
    if (Input.GetKeyDown(KeyCode.T))
    {
        playerHealth.Value = Random.Range(0, 100);
    }

    // ... (The rest of your movement and input code)

    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    if (isGrounded && velocity.y < 0)
    {
        velocity.y = -2f;
    }

    float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");
    Vector3 move = transform.right * x + transform.forward * z;

    if (move != Vector3.zero)
    {
        isMoving = true;
        float speedMultiplier = (Input.GetKey(KeyCode.LeftShift)) ? runSpeedMultiplier : 1f;
        controller.Move(move * speed * speedMultiplier * Time.deltaTime);
        MovementSpeed = move.magnitude * speedMultiplier;
    }
    else
    {
        isMoving = false;
        MovementSpeed = 0f;
    }

    // Update the isMoving parameter in the Animator component
    animator.SetBool("isMoving", isMoving);

    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator.SetTrigger("Jump");
    }

    // Synchronize the isMoving variable across the network
    isMovingNetworkVar.Value = isMoving;

    // Enable or disable the Animator component based on the isMoving variable
    animator.enabled = isMoving;

    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);
}

    [ClientRpc]
    public void TakeDamageClientRpc(int damage, ulong id)
    {
        if (OwnerClientId == id)
        {
            hp -= damage;
            if (hp <= 0)
            {
                hp = 100;
                Destroy(gameObject);
                spawner.SpawnPlayer(OwnerClientId);
            }
            Debug.Log(hp);
        }
    }
    [ServerRpc(RequireOwnership=false)]
    public void TakeDamageServerRpc(ulong id, int damage){
        if (!IsServer)
            return;
        TakeDamageClientRpc(damage, id);
    }
    void HeadBob(float z, float xIntensity, float yIntensity)
    {
        weaponBobPosition = weaponOrigin + new Vector3(Mathf.Cos(z) * xIntensity, Mathf.Sin(z*2) * yIntensity, 0);
    }
}
