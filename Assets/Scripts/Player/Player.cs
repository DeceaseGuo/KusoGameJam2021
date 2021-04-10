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

    public AudioSource mAudio = null;
    public float mMoveSpeed = 0;    
    [Header("使用道具僵直")]
    public float mUseItemStiff = 0;
    [Header("位移")]
    public float mDashStiff = 0;
    public float mDashSpeed = 0;
    [Header("頭")]
    public Transform mHeadPos = null;

    private bool mIsRight = false;
    private string mHaedName = null;
    private Transform mCurHeadTransform = null;
    private float mCurDelayStiff = 0;
    private byte mItemCount = 0;
    private Animator mAnim = null;
    private Vector2 mMoveDirection = Vector2.zero;
    private Vector2 mAtkDirection = Vector2.zero;
    private Rigidbody2D mRig = null;

    public AudioClip mAtkAudio = null;    

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
            mAudio.PlayOneShot(mAtkAudio);
            mRig.velocity = Vector2.zero;
            mAnim.SetTrigger(State.ATTACK.ToString());
            mCurDelayStiff = mDashStiff;
            SetAtkDirection();
            ChangeStep(State.ATTACK);
        }
    }

    private void StepUseItem()
    {
        if (mItemCount > 0 && mCurDelayStiff <= 0 && mCurState != State.USEITEM && Input.GetMouseButton(0))
        {
            mItemCount--;
            mRig.velocity = Vector2.zero;
            mCurDelayStiff = mUseItemStiff;
            mAtkDirection = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            SetAtkDirection();
            //使用道具動畫
            ChangeStep(State.USEITEM);
        }
    }

    private void OnTriggerEnter2D(Collider2D iOther)
    {
        if (mCurState == State.ATTACK)
        {
            VtuberInfo aVtuberInfo = iOther.GetComponent<VtuberInfo>();
            if (aVtuberInfo != null)
            {
                if (!string.IsNullOrEmpty(mHaedName))//有頭在手上情況
                {
                    Vector3 aCurPos = mCurHeadTransform.position;
                    mCurHeadTransform.SetParent(null);
                    mCurHeadTransform.position = aCurPos;
                }

                HeadPerform aHeadPerform = aVtuberInfo.Head.GetComponent<HeadPerform>();
                mHaedName = aVtuberInfo.Head.name;
                VtuberManager.Instance.TouchPlayer(aVtuberInfo);
                aHeadPerform.ExeCute(() =>
                {
                    mCurHeadTransform = aVtuberInfo.Head.transform;
                    mCurHeadTransform.SetParent(mHeadPos);
                    mCurHeadTransform.localPosition = Vector3.zero;
                        //mCurHeadTransform.DOMove(Vector3.zero, .5f);
                    });
                mItemCount++;
            }
        }
    }

    private void SetAtkDirection()
    {
        mAtkDirection = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
        if ((!mIsRight && mAtkDirection.x > 0) || (mIsRight && mAtkDirection.x < 0))
        {
            Flip();
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