using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientAnimationController : MonoBehaviour
{
    private Animator animator;

    public enum PostureState
    {
        Standing,
        Sitting
    }

    public enum LegState
    {
        Normal,
        CrossLegged
    }

    public enum ArmState
    {
        Down,
        Up
    }

    private PostureState postureState = PostureState.Standing;
    private LegState legState = LegState.Normal;
    private ArmState armState = ArmState.Down;

    private static readonly int CrossLeggedHash = Animator.StringToHash("CrossLegged");
    private static readonly int SitDownHash = Animator.StringToHash("SitDown");
    private static readonly int ArmsUpHash = Animator.StringToHash("ArmsUp");

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetBool(CrossLeggedHash, legState == LegState.CrossLegged);
        animator.SetBool(SitDownHash, postureState == PostureState.Sitting);
        animator.SetBool(ArmsUpHash, armState == ArmState.Up);
    }

    public void ResetPosture()
    {
        postureState = PostureState.Standing;
        legState = LegState.Normal;
        armState = ArmState.Down;
        UpdateAnimator();
    }

    public void SitDown()
    {
        postureState = PostureState.Sitting;
        UpdateAnimator();
    }

    public void StandUp()
    {
        postureState = PostureState.Standing;
        UpdateAnimator();
    }

    public void ToggleCrossLegged(bool state)
    {
        if (postureState == PostureState.Sitting)
        {
            legState = state ? LegState.CrossLegged : LegState.Normal;
            UpdateAnimator();
        }
    }

    public void ToggleArmsUp(bool state)
    {
        if (postureState == PostureState.Sitting)
        {
            armState = state ? ArmState.Up : ArmState.Down;
            UpdateAnimator();
        }
    }

    // Added method to check if the patient is sitting
    public bool IsSitting()
    {
        return postureState == PostureState.Sitting;
    }

    // Direct methods to set the states
    public void SetPostureState(PostureState state)
    {
        postureState = state;
        UpdateAnimator();
    }

    public void SetLegState(LegState state)
    {
        legState = state;
        UpdateAnimator();
    }

    public void SetArmState(ArmState state)
    {
        armState = state;
        UpdateAnimator();
    }
}
