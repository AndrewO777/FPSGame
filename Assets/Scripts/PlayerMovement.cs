using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;



public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float speed = 12f;
    Vector3 velocity;
    bool isGrounded;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform weapon;
    Vector3 weaponOrigin;
    Vector3 weaponBobPosition;
    float idleCounter;
    float movementCounter;

    private NetworkVariable<int> playerHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
        }
    }

    private void Update()
    {
        //allows user to control only their own model
        if (!IsOwner)
        {
            return;
        }

        //edit health value
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerHealth.Value = Random.Range(0, 100);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x == 0 && z == 0)
        {
            HeadBob(idleCounter,.025f,.025f);
            idleCounter += Time.deltaTime;
            weapon.localPosition = Vector3.Lerp(weapon.localPosition, weaponBobPosition, Time.deltaTime * 2f);
        }
        else
        {
            HeadBob(movementCounter,.035f,.035f);
            movementCounter += Time.deltaTime * 4f;
            weapon.localPosition = Vector3.Lerp(weapon.localPosition, weaponBobPosition, Time.deltaTime * 8f);
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    [ServerRpc]
    public void TakeDamageServerRpc(int damage){
        playerHealth.Value -= damage;
        Debug.Log(playerHealth.Value);
    }
    void HeadBob(float z, float xIntensity, float yIntensity)
    {
        weaponBobPosition = weaponOrigin + new Vector3(Mathf.Cos(z) * xIntensity, Mathf.Sin(z*2) * yIntensity, 0);
    }
}
