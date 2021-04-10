using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private enum State
    {
        IDLE,
        RUN,
        ATTACK,
        USEITEM,
        DIE
    }
    private State mCurState = State.IDLE;
    public float mMoveSpeed = 0;

    [Header("¦ì²¾")]
    public float mDashStiff = 0;
    public float mDashSpeed = 0;

    private bool mIsRight = false;
    private float mCurDelayStiff = 0;
    private byte mItemCount = 0;
    private Animator mAnim = null;
    private Vector2 mMoveDirection = Vector2.zero;
    private Vector2 mAtkDirection = Vector2.zero;
    private Rigidbody2D mRig = null;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
        mRig = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        StateAction();        
    }

    private void StateAction()
    {
        switch (mCurState)
        {
            case State.IDLE:
            case State.RUN:
                StepMove();
                StepAttack();
                StepUseItem();
                break;
            case State.ATTACK:
                mRig.velocity = mAtkDirection * mDashSpeed;
                break;
            case State.USEITEM:

                break;
            case State.DIE:

                break;
            default:
                break;
        }
    }

    private void ChangeStep(State iState)
    {
        mCurState = iState;
    }

    private void StepMove()
    {
        if (mCurDelayStiff <= 0)
        {
            mMoveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            mRig.velocity = new Vector2(mMoveDirection.x * mMoveSpeed, mMoveDirection.y * mMoveSpeed);
            if (mMoveDirection.x != 0 || mMoveDirection.y != 0)
            {
                if ((!mIsRight && mMoveDirection.x > 0) || (mIsRight && mMoveDirection.x < 0))
                {
                    Flip();
                }
                mAnim.SetFloat(State.RUN.ToString(), 1);
            }
            else
                mAnim.SetFloat(State.RUN.ToString(), -1);
        }
        else
        {
            mCurDelayStiff -= Time.deltaTime;
        }
    }
    
    private void StepAttack()
    {
        if (mCurDelayStiff <= 0 && mCurState != State.ATTACK && Input.GetMouseButton(0))
        {
            mRig.velocity = Vector2.zero;
            mAnim.SetTrigger(State.ATTACK.ToString());
            mCurDelayStiff = mDashStiff;
            mAtkDirection = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            if ((!mIsRight && mAtkDirection.x > 0) || (mIsRight && mAtkDirection.x < 0))
            {
                Flip();
            }
            ChangeStep(State.ATTACK);
        }
    }

    private void StepUseItem()
    {

    }

    private void OnTriggerEnter2D(Collider2D iOther)
    {
        if (mCurState == State.ATTACK)
        {
            VtuberInfo aVtuberInfo = iOther.GetComponent<VtuberInfo>();
            if (aVtuberInfo != null)
            {
                Debug.Log("Get");
            }

            Debug.Log("Trigger Enter");
        }
    }

    private void Flip()
    {
        mIsRight = !mIsRight;
        Vector3 aLocalScale = transform.localScale;
        aLocalScale.x *= -1;
        transform.localScale = aLocalScale;
    }

    public void ReturnIDLE()
    {
        mRig.velocity = Vector2.zero;
        mAnim.SetFloat(State.RUN.ToString(), -1);
        ChangeStep(State.IDLE);
    }
}