using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : StateMachineBehaviour
{
    [SerializeField] private ThirdPersonShooterController thirdPersonShooterController;
    [SerializeField] private AudioSource audioSource;

    public AudioClip reloadSound1;
    public AudioClip reloadSound2;
    bool hasReloaded = false;
    bool pullTrigger = false;

    public float soundTime;
    float reloadTime = 0.9f;

    public float reloadDone = 0.9f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        thirdPersonShooterController = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonShooterController>();
        audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        hasReloaded = false;
        pullTrigger = false;
        audioSource.PlayOneShot(reloadSound1);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //재장전 시간만큼 애니메이션이 충분히 진행되었다면 리로드
        if (stateInfo.normalizedTime >= soundTime && !pullTrigger)
        {
            audioSource.PlayOneShot(reloadSound2);
            pullTrigger = true;
        }

        //재장전 시간만큼 애니메이션이 충분히 진행되었다면 리로드
        if (stateInfo.normalizedTime >= reloadTime && !hasReloaded)
        {
            thirdPersonShooterController.ReloadComplete();
            hasReloaded = true;
        }
        
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasReloaded = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
