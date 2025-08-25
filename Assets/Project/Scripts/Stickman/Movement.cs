using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class MovementKeys {
    [SerializeField] private KeyCode m_jumpKey = KeyCode.Space;
    [SerializeField] public KeyCode m_leftKey = KeyCode.A;
    [SerializeField] public KeyCode m_rightKey = KeyCode.D;
} 

public class Movement : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;
    public GameObject lowerLeftLeg;
    public GameObject lowerRightLeg;
    Rigidbody2D leftLegRB;
    Rigidbody2D lowerLeftLegRB;
    Rigidbody2D rightLegRB;
    Rigidbody2D lowerRightLegRB;

    
    [SerializeField] Animator anim;
    [SerializeField] float speed = 2f;
    [SerializeField] float lowerSpeed = 2f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float legWait = .5f;
    
    
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float jumpForce = 10f;
    private bool isGrounded = false;
    public float positionRadius;
    public LayerMask groundLayer;
    public Transform playerPos;
    
    public MovementKeys keys = new();
    
    void Start()
    {
        leftLegRB = leftLeg.GetComponent<Rigidbody2D>();
        rightLegRB = rightLeg.GetComponent<Rigidbody2D>();
        lowerLeftLegRB = lowerLeftLeg.GetComponent<Rigidbody2D>();
        lowerRightLegRB = lowerRightLeg.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }

    void Update()
    {
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            if(Input.GetAxis("Horizontal") > 0)
            {
                anim.Play("WalkLeft");
                StartCoroutine(MoveRight(legWait));
            }
            else
            {
                anim.Play("WalkRight");
                StartCoroutine(MoveLeft(legWait));
            
            }
            
        }
        else
        {
            anim.Play("idle");
        }
        
        isGrounded = Physics2D.OverlapCircle(playerPos.position, positionRadius, groundLayer);
        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
       
    }


    
    IEnumerator MoveRight(float seconds)
    {
        leftLegRB.AddForce(Vector2.right * (speed*1000) * Time.deltaTime);
        lowerLeftLegRB.AddForce(Vector2.right * (lowerSpeed*1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        rightLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
        lowerRightLegRB.AddForce(Vector2.right * (lowerSpeed * 1000) * Time.deltaTime);
    }

    IEnumerator MoveLeft(float seconds)
    {
        rightLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
        lowerRightLegRB.AddForce(Vector2.left * (lowerSpeed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        lowerLeftLegRB.AddForce(Vector2.left * (lowerSpeed * 1000) * Time.deltaTime);
        leftLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
    }
}