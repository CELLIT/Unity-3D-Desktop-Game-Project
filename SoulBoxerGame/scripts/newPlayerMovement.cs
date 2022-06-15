using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//yumruk atmalara animasyon ekle

public class newPlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Collision rightH;
    public Collision leftH;
    public float speed = 6f;
    public float slowAmp = 0.65f;
    public Camera cam;
    //public Transform obj;
    public Transform anchor;
    public float playerHealth = 100;
    //public float nothing;
    private float xRotation = 0f;
    private Vector3 FixedScale;
    private Vector3 FixedUpScale;
    public float mouseSensitivity = 80f;
    private float gravity = -9.81f;
    public float mass = 10f;
    public float temp = 0.65f;
    public Transform groundChecker;
    public float groundCheckRadius;
    public LayerMask obstacle_layer;
    public LayerMask enemy_layer;
    //private bool isGrounded;
    private bool isCrouched = false;
    private float fixedCam;
    public float jumpSpeed = 9f;
    public Animator animator;
    public Transform newT;
    public Vector3 moveInputs;
    public float timer;
    private Vector3 velocity;

    public bool SpeedBuff = false;
    // Start is called before the first frame update
    void Start()
    {
        fixedCam = cam.transform.localPosition.y;
        FixedScale = newT.localScale;       
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {      
        //hareket inputlarına göre hareketin saglanması
        moveInputs = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        Vector3 moveVelocity = moveInputs * Time.deltaTime * speed;
        Vector3 crouch = new Vector3(1f, 0.5f, 1f);      
        
        controller.Move(moveVelocity);
        //kamera ve karakterin fare ile kontrolü
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity, 0);
        xRotation -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -65f, 60f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //idle durumu kontrolü
        if(moveInputs == new Vector3(0f, 0f, 0f))
        {
            animator.ResetTrigger("jumping");
            animator.ResetTrigger("running");
            animator.ResetTrigger("walking");
            animator.SetTrigger("idle");            
        }
        else if(moveInputs != new Vector3(0f, 0f, 0f))
        {            
            animator.ResetTrigger("running");
            animator.ResetTrigger("idle");            
            animator.SetTrigger("walking");
        }
        //
        if (!CheckGrounded())
        {
            animator.SetTrigger("jumping");
            velocity.y += gravity * mass * Time.deltaTime/2;
            speed = jumpSpeed;        
        }
        if(CheckGrounded())
        {
            animator.ResetTrigger("jumping");
            velocity.y = 0.01f;
            speed = 6f;
            isCrouched = true;
        }
        if(Input.GetKeyDown(KeyCode.Space) && CheckGrounded() && !Input.GetKey(KeyCode.LeftControl))
        {
            velocity.y = 9f;
            isCrouched = false;
        }
       
        if(Input.GetAxis("Fire3")!= 0 && CheckGrounded() && !Input.GetKey(KeyCode.LeftControl))
        {        
            animator.SetTrigger("running");
            if (SpeedBuff)
            {
                speed = 12f;
            }
            else speed = 9f;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && CheckGrounded())
        {
            animator.SetTrigger("crouch");
            speed = 3f;         
            anchor.localScale = new Vector3(anchor.localScale.x, 0.5f, anchor.localScale.z);
            newT.localScale = new Vector3(0.8027f,0.8027f *2,0.8027f);
            cam.transform.localPosition =  new Vector3(0f,cam.transform.localPosition.y*2,-4f);

        }
        if(Input.GetKeyUp(KeyCode.LeftControl) && isCrouched)        
        {
            animator.ResetTrigger("crouch");
            anchor.localScale = new Vector3(anchor.localScale.x, 1f, anchor.localScale.z);
            newT.localScale = FixedScale;
            cam.transform.localPosition = new Vector3(0f, fixedCam  , -4f);
            controller.Move(new Vector3 (0f, temp, 0f));           
        }  
        if(Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("attack");
        }
        if (Input.GetMouseButtonUp(0) && !Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.ResetTrigger("attack");
        }

        controller.Move(velocity * Time.deltaTime);
        if (SpeedBuff)
        {
            SetSpeed(9f);
            if(Input.GetAxisRaw("Fire3") != 0 && !Input.GetKey(KeyCode.LeftControl)) SetSpeed(12f);
            if(!CheckGrounded()) SetSpeed(12f);
            timer += Time.deltaTime;
            if (timer >= 29.9f)
            {
                SpeedBuff = false;
                timer = 0;
                SetSpeed((6f));
            }
        }
        

        
    }
    private bool CheckGrounded()
    {
       return Physics.CheckSphere(groundChecker.position, groundCheckRadius, obstacle_layer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 12)
        {
            playerHealth -= 4;
        }

        if (other.gameObject.layer == 13)
        {
            SetSpeed((9));
            SpeedBuff = true;
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == 14)
        {
            playerHealth = 100f;
            Destroy(other.gameObject);
        }
    }

    private void SetSpeed(float newspeed)
    {
        speed = newspeed;
    }
    
}
