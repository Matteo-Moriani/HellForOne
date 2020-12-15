using System;
using UnityEngine;

namespace ArenaSystem
{
    public class ArenaSpectatorsRotation : MonoBehaviour
    {
        private ArenaManager _arenaManager;
        private Transform _boss;

        private void Awake()
        {
            _arenaManager = transform.root.GetComponent<ArenaManager>();
        }

        private void OnEnable()
        {
            _boss = _arenaManager.Boss.transform;
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation((_boss.position - transform.position).normalized, Vector3.up);
        }
    }
}