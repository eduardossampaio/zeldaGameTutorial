using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private int HP;
    [SerializeField] public float gravity = -19.62f;

    [Header("Attack")]
    [SerializeField] private ParticleSystem fxAttack;
    [SerializeField] private Transform hitbox;
    [SerializeField] [Range(0, 1)] private float hitRange = 0.5f;
    [SerializeField] LayerMask attackLayers;
    [SerializeField] private int damageAmmount;

    [Header("Jump Controller")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public int jumpHeight;

    private CharacterController controller;

    private Vector3 direction;
    private Animator animator;
    private bool isWallking;
    [SerializeField] private bool isGrounded;
    private bool isAttacking;
    private bool isDead = false;

    private GameManager _gameManager;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if(_gameManager.gameState != GameState.GAMEPLAY)
        {
            return;
        }
        HandleInput();

        MoveCharacter();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundLayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("TakeDamage"))
        {
            GetHit(1);
        }
    }

    #region
    void HandleInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        direction = new Vector3(horizontal, 0, vertical).normalized;

        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Attack();
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        fxAttack.Emit(1);

        var hitinfo = Physics.OverlapSphere(hitbox.position, hitRange, attackLayers);
        foreach(Collider collider in hitinfo)
        {
            collider.gameObject.SendMessage("GetHit", damageAmmount, SendMessageOptions.DontRequireReceiver);
        }
    }

    void GetHit(int damage)
    {
        if (isDead)
        {
            return;
        }
        HP -= damage;

        if(HP>0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            isDead = true;
            _gameManager.ChangeGameState(GameState.GAMEOVER);
            animator.SetTrigger("Die");
        }
    }
    void AttackDone()
    {
        isAttacking = false;
    }
    void MoveCharacter()
    {
     
        isWallking = direction.magnitude > 0.1;
        if (isWallking)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
       
        controller.Move(moveSpeed * Time.deltaTime * direction);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        animator.SetBool("isWalking", isWallking);
    }
    #endregion


    private void OnDrawGizmosSelected()
    {
        if (hitbox != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(hitbox.position, hitRange);
        }
    }
}
