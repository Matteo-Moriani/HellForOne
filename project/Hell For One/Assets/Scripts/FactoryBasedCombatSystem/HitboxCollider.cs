using System;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class HitboxCollider : MonoBehaviour
    {
        #region Fields
        
        private Dash _dash;
        private bool _isVulnerable = true;

        #endregion
    
        #region Delegates and events

        internal event Action OnBeingHit;
    
        #endregion

        #region Unity methods

        private void Awake()
        {
            _dash = transform.root.GetComponent<Dash>();
        }

        private void OnEnable()
        {
            _dash.OnStartDash += OnStartDash; 
            _dash.OnStopDash += OnStopDash;
        }

        private void OnDisable()
        {
            _dash.OnStartDash -= OnStartDash;
            _dash.OnStopDash -= OnStopDash;
        }

        #endregion

        #region Methods
        
        public void NotifyHit()
        {
            // if(!_isVulnerable)
            //     return;
            //
            // hitData.ProcessedDamage.ApplyMultiplierModifier(damageMultiplier);
            //
            // OnBeingHit?.Invoke(hitData);
        }

        #endregion
    
        #region Event handlers

        private void OnStopDash()
        {
            _isVulnerable = true;
        }

        private void OnStartDash()
        {
            _isVulnerable = false;
        }
    
        #endregion    
    }
}