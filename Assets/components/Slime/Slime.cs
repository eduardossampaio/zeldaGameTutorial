using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Slime : MonoBehaviour
{
    [SerializeField] private int HP;
    [SerializeField] private EnemyState state;

    private Animator animator;
    private bool isDie = false;
    private int rand;
   

    private GameManager _gameManager;

    //IA
    private NavMeshAgent agent;
    private Vector3 destination;
    private int idWaypoint = 0;
    private bool isPlayerVisible;
    private bool isAttack;

    //movimentacao
    private bool isWalking;
    private bool isAlert;
    

    void Start()
    {
        animator = GetComponent<Animator>();
       
        _gameManager = FindObjectOfType<GameManager>();       
        agent = GetComponent<NavMeshAgent>();

        ChangeState(state);
    }

    
    void Update()
    {
        StateManager();
        isWalking = (agent.desiredVelocity.magnitude >= 0.1f);
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isAlert", isAlert);

    }
    
    IEnumerator CompleteDie()
    {
        yield return new WaitForSeconds(2.5f);
        _gameManager.SpawGem(transform.position );
        Destroy(gameObject);        
    }
    public void GetHit(int amount)
    {
        if (isDie)
        {
            return;
        }
        HP -= amount;
        if (HP <= 0)
        {
            animator.SetTrigger("Die");
            isDie = true;
            ChangeState(EnemyState.DEAD);
            StartCoroutine(CompleteDie());
        }
        else
        {
            ChangeState(EnemyState.FURY);
            animator.SetTrigger("GetHit");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _gameManager.gameState == GameState.GAMEPLAY)
        {
            isPlayerVisible = true;
            if (state == EnemyState.IDLE || state == EnemyState.PATROL)
            {
                ChangeState(EnemyState.ALERT);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerVisible = false;
            //if(state == EnemyState.FOLLOW)
            //{
            //    ChangeState(EnemyState.PATROL);
            //}
        }
    }
    void StateManager()
    {
        if(_gameManager.gameState == GameState.GAMEOVER &&
            (state == EnemyState.FURY || state == EnemyState.FOLLOW || state == EnemyState.ALERT))
        {
            ChangeState(EnemyState.PATROL);
        }
        switch(state)
        {
            case EnemyState.IDLE:
                break;           
            case EnemyState.FOLLOW:
                LookAt();
                destination = _gameManager.player.transform.position;
                agent.destination = destination;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }
                break;
            case EnemyState.FURY:
                LookAt();
                destination = _gameManager.player.transform.position;
                agent.destination = destination;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }
                break;
            case EnemyState.ALERT:
                LookAt();
                break;
            case EnemyState.PATROL:
                break;
        }
    }

    void ChangeState(EnemyState newState)
    {              
        isAlert = false;
        StopAllCoroutines();
        switch (newState)
        {
            case EnemyState.IDLE:
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine(OnIdle());
                break;
            case EnemyState.PATROL:
                idWaypoint = Random.Range(0, _gameManager.smileWaypoints.Length);
                destination = _gameManager.smileWaypoints[idWaypoint].position;
                agent.destination = destination;
                agent.stoppingDistance = 0;
                StartCoroutine(OnPatrol());
                break;
            case EnemyState.FOLLOW:
                destination = _gameManager.player.transform.position;
                agent.destination = destination;
                agent.stoppingDistance = _gameManager.slimeDistanceToAttack;          
                break;
            case EnemyState.FURY:
                destination = _gameManager.player.transform.position;
                agent.destination = destination;
                agent.stoppingDistance = _gameManager.slimeDistanceToAttack;
                break;
            case EnemyState.ALERT:
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine(OnAlert());
                break;
            case EnemyState.DEAD:
                destination = transform.position;
                agent.destination = destination;
                break;
        }
        state = newState;
    }

    

    IEnumerator OnIdle()
    {
        yield return new WaitForSeconds(_gameManager.slimeIdleWaitTime);
        DecideIfPatrolOrIdle(50);
    }

    IEnumerator OnPatrol()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        DecideIfPatrolOrIdle(30);
    }

    IEnumerator OnAlert()
    {
        yield return new WaitForSeconds(_gameManager.slimeAlertWaitTime);
        if (isPlayerVisible)
        {
            ChangeState(EnemyState.FOLLOW);
        }
        else
        {
            DecideIfPatrolOrIdle(10);
        }

    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(_gameManager.slimeAttackDelay);
        isAttack = false;
    }
    private void DecideIfPatrolOrIdle(int yes)
    {
        if (Rand() <= yes)
        {
            ChangeState(EnemyState.IDLE);
        }
        else
        {
            ChangeState(EnemyState.PATROL);
        }
    }
    int Rand()
    {
        rand = Random.Range(0, 100);
        return rand;
    }

    void Attack()
    {
        if (!isAttack && isPlayerVisible)
        {
            isAttack = true;
            animator.SetTrigger("Attack");
        }
    }
    void LookAt()
    {
        
        Vector3 lookDirection = (_gameManager.player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _gameManager.slimeLookAtSpeed * Time.deltaTime);
    }
    void AttackDone()
    {
        StartCoroutine(ResetAttack());
    }

}
