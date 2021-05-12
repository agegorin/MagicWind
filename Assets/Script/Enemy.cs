using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState
{
    Wait,
    Move,
    Fall,
    GetHit
}

public class Enemy : MonoBehaviour
{
    private Vector3 lastPoint;
    private Vector3 targetPoint;
    private float startMoveTime;

    private Vector3 startFallScale;
    private bool isStartFall;
    private float startFallTime;
    private float getDamageTime;

    private EnemyState currentState = EnemyState.Wait;

    private Animator animator;

    void Start()
    {
        lastPoint = transform.position;
        targetPoint = transform.position;
        startMoveTime = Time.time;
        isStartFall = false;
        currentState = EnemyState.Wait;

        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (currentState == EnemyState.Fall)
        {
            transform.localScale = Vector3.Lerp(startFallScale, Vector3.zero, (Time.time - startFallTime) * 2f);
            transform.position = Vector3.Lerp(lastPoint, targetPoint, (Time.time - startFallTime) * 2f);

            if (transform.localScale.magnitude < 0.05f)
            {
                Destroy(gameObject);
            }
        }

        if (currentState == EnemyState.GetHit)
        {
            Debug.Log(Vector3.Distance(transform.position, targetPoint));
            
            transform.position = lastPoint + new Vector3(Mathf.Sin(Time.time * 50f) * 0.1f, 0, 0); // Vector3.Lerp(lastPoint, targetPoint, (Time.time - startFallTime) * 10f);

            if (Time.time - getDamageTime > 0.7f)
            {
                Destroy(gameObject);
            }
        }

        if (currentState == EnemyState.Move)
        {
            transform.position = Vector3.Lerp(lastPoint, targetPoint, (Time.time - startMoveTime) * 2.5f);

            if (isStartFall && Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                currentState = EnemyState.Fall;
                startFallScale = transform.localScale;
                lastPoint = transform.position;
                targetPoint = transform.position - new Vector3(0, 1.2f, 0);
                startFallTime = Time.time;
            }
            else if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
            {
                currentState = EnemyState.Wait;
            }
        }

        if (currentState == EnemyState.Wait)
        {
            transform.position = targetPoint;
            animator.Play("stay");
        }        
    }

    public void StartMove(Vector3 _targetPoint)
    {
        if (currentState == EnemyState.Wait)
        {
            currentState = EnemyState.Move;
            startMoveTime = Time.time;
            lastPoint = transform.position;
            targetPoint = _targetPoint;
            GetComponent<Animator>().Play("walk");
        }
    }

    public void WindMove(Vector3 _targetPoint, bool isFallToWater)
    {
        if (currentState == EnemyState.Wait)
        {
            currentState = EnemyState.Move;
            startMoveTime = Time.time;
            lastPoint = transform.position;
            targetPoint = _targetPoint;
            isStartFall = isFallToWater;
        }
    }

    public void GetDamage()
    {
        if (currentState != EnemyState.GetHit)
        {
            currentState = EnemyState.GetHit;
            getDamageTime = Time.time;
        }
    }
}
