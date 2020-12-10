using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CooldownSystem
{
    public class Cooldowns : MonoBehaviour
    {
        private readonly Dictionary<ICooldown, Coroutine> _cooldowns = new Dictionary<ICooldown, Coroutine>();
        
        public bool TryAbility(ICooldown cooldownObject)
        {
            // If dictionary has key, we used this tool at least once
            // so we have to check for its cooldown
            if (_cooldowns.ContainsKey(cooldownObject) && _cooldowns[cooldownObject] != null) 
                return false;

            StartCooldown(cooldownObject);

            return true;
        }
        
        private void StartCooldown(ICooldown cooldownObject)
        {
            if(_cooldowns.ContainsKey(cooldownObject))
                _cooldowns[cooldownObject] = StartCoroutine(Cooldown(cooldownObject));
            else
                _cooldowns.Add(cooldownObject,StartCoroutine(Cooldown(cooldownObject)));
        }
    
        private void StopCooldown(ICooldown cooldownObject)
        {
            cooldownObject.NotifyCooldownEnd();

            StopCoroutine(_cooldowns[cooldownObject]);
        
            _cooldowns[cooldownObject] = null;
        }

        // TODO - Calculate delay
        private IEnumerator Cooldown(ICooldown cooldownObject)
        {
            cooldownObject.NotifyCooldownStart();

            yield return new WaitForSeconds(cooldownObject.GetCooldown());

            StopCooldown(cooldownObject);
        }
    }
}