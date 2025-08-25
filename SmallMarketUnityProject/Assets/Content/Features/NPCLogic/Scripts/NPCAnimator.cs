using System;
using Content.Features.NPCSpawner.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Features.NPCLogic.Scripts
{
    public class NPCAnimator : MonoBehaviour
    {
        [SerializeField] private float speedThreshold = 0.1f; 
        [SerializeField]  NavMeshAgent _agent;
        [SerializeField]private Animator _anim;
        [Header("Animations")]
        [SerializeField] private string idleAnimation = "Idle";
        [SerializeField] private string walkAnimation = "Walk";
        
        private string _lastPlayed;

        private void Update()
        {
            if (_agent == null || _anim == null) return;

            string animationToPlay = _agent.velocity.magnitude > speedThreshold ? walkAnimation : idleAnimation;

            if (animationToPlay != _lastPlayed)
            {
                _anim.Play(animationToPlay);
                _lastPlayed = animationToPlay;
            }
        }
    }
}