using System;
using System.Collections;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animations
{
    public class Spectator : MonoBehaviour
    {
        private Animator _animator;
        private CombatSystem _combatSystem;

        private void Awake() 
        {
            _animator = GetComponent<Animator>();
            _combatSystem = GetComponentInChildren<CombatSystem>();
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _combatSystem.OnStopAttack += OnStopAttack;
        }

        private void OnDisable()
        {
            _combatSystem.OnStartAttack -= OnStartAttack;
            _combatSystem.OnStopAttack -= OnStopAttack;
        }

        private void Start() 
        {
            StartCoroutine(RandomDelay());
        }

        private void StartAnimation() {
            //AnimationClip animation = GetComponent<AnimationsManager>().GetRandomAnimation();
            //Debug.Log(animation.name);
            switch(Random.Range(0,3)) {
                case 0:
                    _animator.SetBool("clap", true);
                    break;
                case 1:
                    _animator.SetBool("excited", true);
                    break;
                case 2:
                    _animator.SetBool("rally", true);
                    break;
                default:
                    //Debug.Log("animation missing");
                    break;
            }
        }
        
        private void OnStopAttack(Attack attack)
        {
            StartAnimation();
        }

        private void OnStartAttack(Attack obj)
        {
            _animator.SetBool("clap",false);
            _animator.SetBool("excited",false);
            _animator.SetBool("rally",false);
            
            _animator.SetTrigger(obj.name);
        }
        
        private IEnumerator RandomDelay() 
        {
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            StartAnimation();
        }
    }
}
