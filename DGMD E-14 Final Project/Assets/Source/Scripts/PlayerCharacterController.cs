
using UnityEngine;

// SOURCE: https://sharpcoderblog.com/blog/unity-3d-fps-controller
public class PlayerCharacterController : MonoBehaviour
{
    CharacterController characterController;
    public Camera playerCamera;
    public float walkSpeed = 6.4f;
    public float runningSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float fallSpeed = 16.0f;
    public float lookSpeed = 2.0f;
    Vector3 moveDirection = Vector3.zero;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forwardDirection = transform.TransformDirection(Vector3.forward);
        Vector3 rightDirection = transform.TransformDirection(Vector3.right);
        float curSpeedX = (Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forwardDirection * curSpeedX) + (rightDirection * curSpeedY);

        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= fallSpeed * Time.deltaTime;
        }
        
        characterController.Move(moveDirection * Time.deltaTime);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}
