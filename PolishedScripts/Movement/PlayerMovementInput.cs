using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Moisty))]

public class PlayerMovementInput : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runSpeed = 15;
    [SerializeField] private float walkspeed = 10;
    [SerializeField] private float SwimmingSpeed = 1;
    [Header("Rotation")]
    [SerializeField] private GameObject FirstPersonCamera;
    [SerializeField] private float rotationSensitivity = 0.1f;
    [Header("SlideSettings")]
    [SerializeField] private float slideStrength = 1.1f;
    [SerializeField] private float slideDelay = 0.7f;
    [SerializeField] private float slideSpeed = 350;
    [SerializeField] private float slideHeight = 1.3f;
    [Header("OtherMovement")]
    [SerializeField] private float jumpStrength = 4f;
    [SerializeField] private float crouchStrength = 1.3f;
    [SerializeField] private float SlopeLimit = 30;
    [Header("GroundCheck")]
    [SerializeField] private GameObject GroundCheck;
    [SerializeField] private float GroundCheckDistance = .12f;
    [Header("Sounds")]
    [SerializeField] private float RunningPitch;
    [SerializeField] private GameObject GrassWalking;
    [Header("FOV")]
    [SerializeField] private float FOVLerpStrength;
    [SerializeField] private float RunningFOV;

    public bool isRunning { get; private set; }
    public bool isCrouching { get; private set; }
    public bool isSliding { get; private set; }
    public bool hasJumped { get; private set; }

    public bool isSwimming {get; private set;}

    private Rigidbody CharacterRigidBody;
    private PlayerActions PlayerActions;
    private Vector2 PlayerMovement;
    private Vector2 FirstPersonCameraMovement;
    private bool jumpPressed;
    private float slideCounter;
    private bool lastCrouchPress = false;
    public bool isGrounded {get; set;}
    private bool lastIsGrounded;
    private bool lastMove = false;
    private float OriginalFOV;

    void Awake()
    {
        PlayerActions = new PlayerActions();
        PlayerActions.Enable();
    }

    void Start()
    {
        CharacterRigidBody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OriginalFOV = FirstPersonCamera.GetComponent<Camera>().fieldOfView;
    }

    void Update()
    {
        //The if(){return;} values for SlopeCheck and SlideCheck cancle movement options when active

        UpdateCameraRotation();

        if(UpdateCharacterWaterPosition())
        {
            return;
        }
        
        CrouchCheck();

        //If the slope check == false 
        if (SlopeCheck())
        {
            SlideCheck();

            //Updates the previous values
            UpdatePreviousValues();
            return;
        }

        JumpCheck();

        //This checks if you can slide along with adding to the slide, if you can you won't play the rest of the function
        //(Note: lastCrouchPress will be updated every frame and carry over to the next)
        if (SlideCheck())
        {
            //Updates the previous values
            UpdatePreviousValues();
            return;
        }
        //Updates the previous values
        UpdatePreviousValues();

        UpdateCharacterPosition();
    }

    private bool UpdateCharacterWaterPosition()
    {
        var Submergence = gameObject.GetComponent<Moisty>().Submergence;
        if(isSwimming && !isGrounded || (Submergence == 1 && isGrounded))
        {
            
            //Get the movement direction
            PlayerMovement = PlayerActions.Movement.Move.ReadValue<Vector2>();
            
            //Apply the movement amount to the movement direction
            PlayerMovement = new Vector2(PlayerMovement.x * SwimmingSpeed, PlayerMovement.y * SwimmingSpeed);

            //Apply the movement to the character
            if(PlayerMovement != new Vector2(0f, 0f))
            {
                CharacterRigidBody.velocity = FirstPersonCamera.transform.rotation * new Vector3(PlayerMovement.x, 0f, PlayerMovement.y);    
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }
    //Updates the values of the current iteration so it can be used the next iteration
    private void UpdatePreviousValues()
    {
        lastCrouchPress = isCrouching;
        lastIsGrounded = isGrounded;
    }
    private bool SlopeCheck()
    {
        RaycastHit hit;

        //Casts a ray based on GroundCheckDistance and returns if it hit and object with the tag "Ground"
        //Asks if the RayCast's angle and compares it with the Vector3.up which is 'flat' and finds the difference (Note: It enters the state if the difference is bigger than the slope limit
        if (Physics.Raycast(GroundCheck.transform.position, transform.TransformDirection(Vector3.down), out hit) && (Vector3.Angle(Vector3.up, hit.normal) >= SlopeLimit))
        {
            //Sets the friction
            gameObject.GetComponent<Collider>().material.dynamicFriction = 0f;
            //Returns true which cancles the rest of the update
            return true;
        }
        //Sets the friction
        gameObject.GetComponent<Collider>().material.dynamicFriction = 1f;
        //Returns false which allows the rest of the update to go on
        return false;

    }
    private bool SlideCheck()
    {
        //Adds to the counter with the time since last frame
        slideCounter += Time.deltaTime;

        //This plays only once because of isSliding
        //Checks for every varable that is needed to enter slide state (Note: lastCrouchPress = if you were standing before and crouching now)(Note: You have to be moving a key to slide)
        if (isRunning && isCrouching && !lastCrouchPress && !isSliding && slideDelay < slideCounter && !PlayerActions.Movement.Move.ReadValue<Vector2>().Equals(new Vector2(0, 0)))
        {
            //Adds force to the rigidBody
            CharacterRigidBody.AddForce(gameObject.transform.forward * slideSpeed);

            //Updates the global varable
            isSliding = true;

            //gameObject.GetComponent<CapsuleCollider>().transform.localScale = new Vector3(1, gameObject.GetComponent<Collider>().transform.localScale.y / crouchStrength, 1);
            gameObject.GetComponent<CapsuleCollider>().height = gameObject.GetComponent<CapsuleCollider>().height / slideHeight;

            //Sets the GroundCheck transform to the world position of the CapsuleColliders lowest point
            GroundCheck.transform.position = transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center.x, (gameObject.GetComponent<CapsuleCollider>().center.y - gameObject.GetComponent<CapsuleCollider>().height / 2.1f), gameObject.GetComponent<CapsuleCollider>().center.z);

            //Changes the slide speed
            gameObject.GetComponent<Collider>().material.dynamicFriction = slideStrength;
        }
        //Checks the only varables that exit slide state (Note: You need to only exit once and loop around to wait again)
        else if ((!isCrouching || !isRunning || (CharacterRigidBody.velocity.x >= 0 && CharacterRigidBody.velocity.x <= .05f) || jumpPressed) && isSliding)
        {
            //Resets counter
            slideCounter = 0;

            //Updates global varable
            isSliding = false;

            //gameObject.GetComponent<CapsuleCollider>().transform.localScale = new Vector3(1, gameObject.GetComponent<Collider>().transform.localScale.y / crouchStrength, 1);
            gameObject.GetComponent<CapsuleCollider>().height = gameObject.GetComponent<CapsuleCollider>().height * slideHeight;

            //Sets the GroundCheck transform to the world position of the CapsuleColliders lowest point
            GroundCheck.transform.position = transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center.x, (gameObject.GetComponent<CapsuleCollider>().center.y - gameObject.GetComponent<CapsuleCollider>().height / 2.1f), gameObject.GetComponent<CapsuleCollider>().center.z);

            //Changes the slide speed
            gameObject.GetComponent<Collider>().material.dynamicFriction = 1f;
        }
        //Returns the varable for every update
        return isSliding;
    }
    private void CrouchCheck()
    {
        //Sets global varable
        isCrouching = PlayerActions.Movement.Crouch.ReadValue<float>().Equals(1f);
        //Works as a GetKeyDown && GetKeyUp for InputActions (It switchs between states based on the previous value)(Note: lastCrouchPress is the previous value)
        if (isCrouching && !lastCrouchPress)
        {
            //gameObject.GetComponent<CapsuleCollider>().transform.localScale = new Vector3(1, gameObject.GetComponent<Collider>().transform.localScale.y / crouchStrength, 1);
            gameObject.GetComponent<CapsuleCollider>().height = gameObject.GetComponent<CapsuleCollider>().height / crouchStrength;

            //Sets the GroundCheck transform to the world position of the CapsuleColliders lowest point
            GroundCheck.transform.position = transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center.x, (gameObject.GetComponent<CapsuleCollider>().center.y - gameObject.GetComponent<CapsuleCollider>().height / 2.05f), gameObject.GetComponent<CapsuleCollider>().center.z);
        }
        else if (!isCrouching && lastCrouchPress)
        {
            //gameObject.GetComponent<CapsuleCollider>().transform.localScale = new Vector3(1, gameObject.GetComponent<Collider>().transform.localScale.y * crouchStrength, 1);
            gameObject.GetComponent<CapsuleCollider>().height = gameObject.GetComponent<CapsuleCollider>().height * crouchStrength;
            //Sets the GroundCheck transform to the world position of the CapsuleColliders lowest point
            GroundCheck.transform.position = transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center.x, (gameObject.GetComponent<CapsuleCollider>().center.y - gameObject.GetComponent<CapsuleCollider>().height / 2.05f), gameObject.GetComponent<CapsuleCollider>().center.z);
        }
        //lastCrouchPress will be updated at the very end of the script
    }
    private void OnCollisionStay(Collision other) 
    {
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            isSwimming = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            isSwimming = false;
        }
    }
    private void JumpCheck()
    {
        //'Triggered' = GetKeyDown
        jumpPressed = PlayerActions.Movement.Jump.triggered;

        if (jumpPressed && isGrounded)
        {
            //Adds force to the 'rigidBody' on the 'y' axis. Applies it in one frame with Impulse based on mass
            CharacterRigidBody.AddForce(0f, jumpStrength, 0f, ForceMode.Impulse);
            //Sets hasJumpedInAir boolean that only updates once every jump frame
            hasJumped = true;
        }
        else if (isGrounded && !lastIsGrounded)
        {
            //Sets hasJumpedInAir boolean that only updates once every jump frame
            hasJumped = false;
        }
    }
    private void UpdateCameraRotation()
    {
        float CameraRotation;

        //Get camera rotaton amount
        FirstPersonCameraMovement = PlayerActions.Looking.Move.ReadValue<Vector2>();

        //Rotate character by FirstPersonCameraMovement
        gameObject.transform.Rotate(0.0f, FirstPersonCameraMovement.x * rotationSensitivity, 0.0f, Space.Self);

        //Applies the current rotation of the camera + the rotation amount * the rotationSensitivity
        CameraRotation = FirstPersonCamera.transform.localEulerAngles.x - (FirstPersonCameraMovement.y * rotationSensitivity);

        //Based on a circle rotation (Note: If a player has a rotation of exactly 180 then it won't clamp)
        if (CameraRotation > 90 && CameraRotation < 180)
        {
            CameraRotation = 90;
        }
        else if (CameraRotation < 270 && CameraRotation > 180)
        {
            CameraRotation = 270;
        }
        //Applies the rotation with CameraRotation
        FirstPersonCamera.transform.localEulerAngles = new Vector3(CameraRotation, 0f, 0f);
    }

    private void UpdateCharacterPosition()
    {
        
        //If it is grounded and !hasJumpedInAir(Is not in air BECAUSE YOU JUMPED)(The reason for this is becuase it updates multiple times before the isGrounded is updated)
        if (isGrounded && !hasJumped && PlayerActions.Movement.Move.ReadValue<Vector2>().Equals(new Vector2(0f, 0f)))
        {
            CharacterRigidBody.velocity = new Vector3(0f, 0f, 0f);
        }
        //Gets the running value as a 'float' and turns it into a 'bool' though comparison to 1f
        isRunning = PlayerActions.Movement.Run.ReadValue<float>().Equals(1f);

        if(isRunning)
        {
            FirstPersonCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(FirstPersonCamera.GetComponent<Camera>().fieldOfView, RunningFOV, FOVLerpStrength);
        }
        else
        {
            FirstPersonCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(FirstPersonCamera.GetComponent<Camera>().fieldOfView, OriginalFOV, FOVLerpStrength);
        }

        //Get the movement direction
        PlayerMovement = PlayerActions.Movement.Move.ReadValue<Vector2>();

        
        if((PlayerMovement.x > 0 || PlayerMovement.y > 0) && isGrounded)
        {
            GrassWalking.SetActive(true);

            if (isRunning)
            {
                GrassWalking.GetComponent<AudioSource>().pitch = RunningPitch;
            }
            else
            {
                GrassWalking.GetComponent<AudioSource>().pitch = 1;
            }
        }
        else
        {
            GrassWalking.SetActive(false);
        }


        if (!isGrounded)
        {
            return;
        }

        //Get the movement amount
        float currentMaxMovingSpeed = isRunning ? runSpeed : walkspeed;

        //Apply the movement amount to the movement direction
        PlayerMovement = new Vector2(PlayerMovement.x * currentMaxMovingSpeed, PlayerMovement.y * currentMaxMovingSpeed);

        //Apply the movement to the character
        CharacterRigidBody.velocity = transform.rotation * new Vector3(PlayerMovement.x, CharacterRigidBody.velocity.y, PlayerMovement.y);

        //Sets the bool lastMove based off the last PlayerActions.Movement.Move.ReadValue<Vector2>().Equals(new Vector2(0f, 0f)) value
        lastMove = !PlayerActions.Movement.Move.ReadValue<Vector2>().Equals(new Vector2(0f, 0f));
    }
}
