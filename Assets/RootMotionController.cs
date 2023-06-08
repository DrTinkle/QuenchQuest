using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RootMotionController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;

    private void OnAnimatorMove()
    {
        agent.velocity = animator.deltaPosition / Time.deltaTime;
    }
}
