using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shark : MonoBehaviour
{
    private const float JUMP_AMOUNT = 100f;
    private Rigidbody2D sharkRigidBody2D;
    private State state;
    private static Shark instance;

    public event EventHandler OnDied;
    public event EventHandler OnStartPlaying;

    private enum State 
    {
        WaitingToStart,
        Playing,
        Dead
    }

    public static Shark GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        sharkRigidBody2D = GetComponent<Rigidbody2D>();
        sharkRigidBody2D.bodyType = RigidbodyType2D.Static;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    sharkRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartPlaying != null) OnStartPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;
        }
        
    }

    private void Jump()
    {
        sharkRigidBody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        sharkRigidBody2D.bodyType = RigidbodyType2D.Static;
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
