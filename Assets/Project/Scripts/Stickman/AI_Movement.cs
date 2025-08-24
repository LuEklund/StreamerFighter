using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AI_MovementKeys {
    [SerializeField] private KeyCode m_jumpKey = KeyCode.Space;
    [SerializeField] public KeyCode m_leftKey = KeyCode.A;
    [SerializeField] public KeyCode m_rightKey = KeyCode.D;
} 

public class AI_Movement : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;
    Rigidbody2D leftLegRB;
    Rigidbody2D rightLegRB;

    
    [SerializeField] Animator anim;
    [SerializeField] float speed = 2f;
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
        yield return new WaitForSeconds(seconds);
        rightLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
    }

    IEnumerator MoveLeft(float seconds)
    {
        rightLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        leftLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
    }
}