using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float mMoveSpeed = 0;

    private Vector2 mMoveDirection = Vector2.zero;
    private Rigidbody2D mRig = null;

    private void Awake()
    {
        mRig = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        mMoveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        mRig.velocity = new Vector2(mMoveDirection.x * mMoveSpeed, mMoveDirection.y * mMoveSpeed);
    }
}