using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    public float health;
    public int damage;

    public bool isBlocking;
    public bool isAttacking;

    private bool canBlock = true;
    private float attackCooldown = 1;
    private float attackCounter;
    private bool doAttack;
    private float healthOld;

    private GameObject mainCamera;

    void Start()
    {
        StartCoroutine(Heartbeat());
        mainCamera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (GameManager.isGameActive)
        {
            Block();
            if (!isBlocking && !isAttacking)
            {
                Turn();
            }
            AttackUpdate();
            if (health <= 0)
            {
                GameManager.Instance.PlayerDeath();
                AudioManager.PlaySound("Hurt Sound", transform.position);
            }
            else if (health < healthOld)
            {
                AudioManager.PlaySound("Hurt Sound", transform.position);
            } 
            healthOld = health;
        }
    }
    private void Turn()
    {
        if (Input.GetButtonDown("Turn Left"))
        {
            transform.Rotate(new Vector3(0, -90, 0));
            mainCamera.transform.Rotate(new Vector3(0, -90, 0));
            AudioManager.PlaySound("Turn Sound");
        }
        else if (Input.GetButtonDown("Turn Right"))
        {
            transform.Rotate(new Vector3(0, 90, 0));
            mainCamera.transform.Rotate(new Vector3(0, 90, 0));
            AudioManager.PlaySound("Turn Sound");
        }
    }
    private void AttackUpdate()
    {
        if (Input.GetButtonDown("Attack") && isBlocking)
        {
            doAttack = true;
        }
        if (attackCounter > 0)
        {
            attackCounter -= Time.deltaTime;
        }
        else if (!isBlocking && (doAttack || Input.GetButtonDown("Attack")))
        {
            AudioManager.PlaySound("Attack Sound");
            attackCounter = attackCooldown;
            doAttack = false;
            StartCoroutine(Attack());
        }
    }
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.2f);
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    private void Block()
    {
        if (Input.GetButtonDown("Block") && canBlock && !isAttacking)
        {
            isBlocking = true;
            canBlock = false;
            AudioManager.PlaySound("Block Sound");
            StartCoroutine(StopBlocking());
        }
    }
    private IEnumerator StopBlocking()
    {
        yield return new WaitForSeconds(1);
        isBlocking = false;
        StartCoroutine(EnableBlock());
    }
    private IEnumerator EnableBlock()
    {
        yield return new WaitForSeconds(1);
        canBlock = true;
    }
    public bool IsFacingEnemy()
    {
        return Physics.Raycast(transform.position, transform.forward, 2f);
    }
    public bool CheckCardinal()
    {
        return Physics.Raycast(transform.position, Vector3.forward, 20f) || Physics.Raycast(transform.position, Vector3.back, 20f) ||
            Physics.Raycast(transform.position, Vector3.left, 20f) || Physics.Raycast(transform.position, Vector3.right, 20f);
    }
    private IEnumerator Heartbeat()
    {
        if (health > 1)
        {
            yield return new WaitForSeconds(health / 5);
        }
        else
        {
            yield return new WaitForSeconds(health / 2);
        }
        if (GameManager.isGameActive)
        {
            AudioManager.PlaySound("Heartbeat Sound", 1 - health / 10 + 0.1f);
        }
        if (health > 0)
        {
            StartCoroutine(Heartbeat());
        }
    }
}
