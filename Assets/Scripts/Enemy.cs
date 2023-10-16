using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public enum Weight
    {
        Light, Normal, Heavy
    }
    public enum Disposition
    {
        Cautious, Normal, Aggressive
    }

    public Weight weight;
    public Disposition disposition;
    public float damage;
    public float timeToHit;
    public float attackRecoveryTime;
    public float health;
    public float walkingSpeed;
    public float runningSpeed;
    public float footstepDistance;

    private PlayerController playerController;

    private Vector3 velocity;
    private Vector3 velocityOld;
    private Vector3 footstepPosOld;
    private bool isAttacking;
    private int movementPhase;
    private int changeMovePhaseCounter;
    private int changeMovePhaseNum;
    private bool doIncrementChangeMovePhaseCounter;
    private float circleSpeed = 80;
    private int footstepFilterCutoff = 7000;
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        footstepPosOld = transform.position;

        switch (Random.Range(0, 3))
        {
            case 0:
                disposition = Disposition.Cautious;
                movementPhase = 2;
                SetMovePhaseNum(3, 1, 1);
                break;
            case 1:
                disposition = Disposition.Normal;
                movementPhase = 0;
                break;
            case 2:
                disposition = Disposition.Aggressive;
                movementPhase = 0;
                break;
        }
        switch (GameManager.difficulty)
        {
            case GameManager.Difficulty.Easy:
                {
                    health /= 2;
                    damage /= 2;
                    break;
                }
            case GameManager.Difficulty.Hard:
                {
                    health *= 1.5f;
                    damage *= 1.5f;
                    break;
                }
        }
    }

    void Update()
    {
        if (GameManager.isGameActive)
        {
            velocity = Vector3.zero;

            if (playerController.isAttacking && playerController.IsFacingEnemy())// && health > 0)
            {
                health -= playerController.damage;
                playerController.isAttacking = false;
                AudioManager.PlaySound("Hurt Sound", transform.position);
                GameManager.Instance.Cheer(3, 0.8f);
            }
            else if (health <= 0)
            {
                GameManager.Instance.EnemyDeath();
                AudioManager.PlaySound("Hurt Sound", transform.position);
                Destroy(gameObject);
            }

            FilterUpdate();

            switch (movementPhase)
            {
                case 0:
                    MoveTowardPlayer();
                    break;
                case 1:
                    MoveAwayFromPlayer();
                    break;
                case 2:
                    CirclePlayer();
                    break;
            }

            FootstepUpdate();

            transform.position += velocity;
            velocityOld = velocity;
        }
    }
    private void FilterUpdate()
    {
        Vector3 toTarget = (transform.position - playerController.transform.position).normalized;

        if (Vector3.Dot(toTarget, -playerController.transform.forward) > 0)
        {
            AudioManager.GetSound("Footstep").GetComponent<AudioLowPassFilter>().cutoffFrequency = footstepFilterCutoff;
            AudioManager.GetSound(weight + " Swing").GetComponent<AudioLowPassFilter>().cutoffFrequency = footstepFilterCutoff;
        }
        else
        {
            AudioManager.GetSound("Footstep").GetComponent<AudioLowPassFilter>().cutoffFrequency = 15000;
            AudioManager.GetSound(weight + " Swing").GetComponent<AudioLowPassFilter>().cutoffFrequency = 15000;
        }
    }
    private void Attack()
    {
        AudioManager.PlaySound(weight + " Swing");
        StartCoroutine(HitPlayer());
        isAttacking = true;
    }
    private IEnumerator HitPlayer()
    {
        yield return new WaitForSeconds(timeToHit);
        if (playerController.isBlocking && playerController.IsFacingEnemy())
        {
            AudioManager.PlaySound("Blocked Sound");
            if (weight == Weight.Heavy)
            {
                playerController.health -= damage / 3;
                GameManager.Instance.Cheer(2, 0.5f);
            }
        }
        else if (playerController.health > 0)
        {
            playerController.health -= damage;
            GameManager.Instance.Cheer(3, 0.8f);
        }
        yield return new WaitForSeconds(attackRecoveryTime);
        movementPhase = 1;
        isAttacking = false;
    }
    private void FootstepUpdate()
    {
        if (Vector3.Distance(transform.position, footstepPosOld) > footstepDistance || (velocityOld == Vector3.zero && velocity != Vector3.zero))
        {
            footstepPosOld = transform.position;
            AudioManager.PlaySound("Footstep", transform.position, 0.8f, 1f);
        }
    }
    private void MoveTowardPlayer()
    {
        if (Vector3.Distance(transform.position, playerController.transform.position) > 2)
        {
            velocity = (playerController.transform.position - transform.position).normalized * runningSpeed * Time.deltaTime;
        }
        else if (!isAttacking)
        {
            Attack();
        }
    }
    private void MoveAwayFromPlayer()
    {
        if (Vector3.Distance(transform.position, playerController.transform.position) < 10)
        {
            velocity = (transform.position - playerController.transform.position).normalized * walkingSpeed * Time.deltaTime;
        }
        else
        {
            movementPhase = 2;
            changeMovePhaseCounter = 0;
            SetMovePhaseNum(8, 6, 4);
            switch (Random.Range(0, 2))
            {
                case 0:
                    circleSpeed *= -1;
                    break;
            }
        }
    }
    private void CirclePlayer()
    {
        transform.RotateAround(playerController.transform.position, Vector3.up, walkingSpeed * Time.deltaTime * circleSpeed / 
            Vector3.Distance(transform.position, playerController.transform.position));
        if (playerController.CheckCardinal())
        {
            if (doIncrementChangeMovePhaseCounter)
            {
                changeMovePhaseCounter++;
                switch (Random.Range(0, 3))
                {
                    case 0:
                        circleSpeed *= -1;
                        break;
                }
            }
            if (changeMovePhaseNum == changeMovePhaseCounter)
            {
                movementPhase = 0;
            }
            doIncrementChangeMovePhaseCounter = false;
        }
        else
        {
            doIncrementChangeMovePhaseCounter = true;
        }
    }
    private void SetMovePhaseNum(int cautiousMax, int normalMax, int aggressiveMax)
    {
        switch (disposition)
        {
            case Disposition.Cautious:
                changeMovePhaseNum = Random.Range(0, cautiousMax);
                break;
            case Disposition.Normal:
                changeMovePhaseNum = Random.Range(0, normalMax);
                break;
            case Disposition.Aggressive:
                changeMovePhaseNum = Random.Range(0, aggressiveMax);
                break;
        }
    }
}
