using UnityEngine;
using System.Collections;

public class IdleRunJump : MonoBehaviour {


	protected Animator animator;
	public float DirectionDampTime = 1.5f;
	public bool ApplyGravity = true;

    public float moveSpeed;
	Rigidbody rb;
    public float jumpSpeed;


    // Use this for initialization
    void Start () 
	{
		rb = GetComponent<Rigidbody>();
        
        animator = GetComponent<Animator>();
		
		if(animator.layerCount >= 2)
			animator.SetLayerWeight(1, 1);

        moveSpeed = 25f;
        jumpSpeed = 9.8f;
    }

    // Update is called once per frame
    void Update () 
	{
        //Debug.Log(animator.GetLayerName(0));
        //Debug.Log(animator.GetLayerName(1));
        Vector3 moveVec = Vector3.zero;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveVec = new Vector3(x, 0, z);
        moveVec = moveVec.normalized * moveSpeed * Time.deltaTime;
        //gameObject.transform.Translate(moveVec);

        if (animator)
		{
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);			

			if (stateInfo.IsName("Base Layer.Run"))
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
                    animator.SetBool("Jump", true);
					Invoke("JumpTo", 0.75f);
                }
            }
			else
			{
				animator.SetBool("Jump", false);                
            }

			if(Input.GetButtonDown("Fire2") && animator.layerCount >= 2)
			{
				animator.SetBool("Hi", !animator.GetBool("Hi"));
			}

            animator.SetFloat("Speed", x * x + z * z);
            animator.SetFloat("Direction", x, DirectionDampTime, Time.deltaTime);	
		}
    }
    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * jumpSpeed);
    }
    void JumpTo()
	{
        Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpSpeed * -2f * Physics.gravity.y);
        rb.AddForce(jumpVelocity, ForceMode.VelocityChange);
    }
}
