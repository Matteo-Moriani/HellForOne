using System;
using UnityEngine;

namespace CrownSystem
{
    public class GroundCrownParticlesBehaviour : MonoBehaviour
    {
        private void Update() => transform.rotation = Quaternion.Euler(-90,0f,0f);
    }
}