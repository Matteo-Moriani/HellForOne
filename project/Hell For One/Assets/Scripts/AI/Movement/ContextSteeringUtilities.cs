using System.Collections.Generic;
using System.Linq;
using ActionsBlockSystem;
using UnityEngine;

namespace AI.Movement
{
    public class ContextMap
    {
        private static readonly Vector3 StartDirection = new Vector3(0f,0f,1f);
        
        public enum Resolution
        {
            VeryLow = 4,
            Low = 8,
            Medium = 16,
            High = 32,
            VeryHigh = 64
        }

        public static readonly Dictionary<Resolution, Vector3[]> defaultDirections=
        new Dictionary<Resolution, Vector3[]>()
        {
            {Resolution.VeryLow, GenerateDirectionMap(1f,Resolution.VeryLow)},
            {Resolution.Low, GenerateDirectionMap(1f, Resolution.Low)},
            {Resolution.Medium, GenerateDirectionMap(1f, Resolution.Medium)},
            {Resolution.High, GenerateDirectionMap(1f, Resolution.High)},
            {Resolution.VeryHigh, GenerateDirectionMap(1f, Resolution.VeryHigh)}
        };

        private readonly Resolution _resolution;

        protected readonly Vector3[] Map;
        
        private static Vector3[] GenerateDirectionMap(float startMagnitude, Resolution resolution)
        {
            startMagnitude = Mathf.Clamp(startMagnitude, 0f, 1f);
            
            int parsedResolution = (int) resolution;
            
            float currentAlpha = 0f;
            float step = 360f / parsedResolution;
            
            Vector3[] directions = new Vector3[parsedResolution];

            for (int i = 0; i < parsedResolution; i++)
            {
                directions[i] = Quaternion.Euler(0f, currentAlpha, 0f) * StartDirection * startMagnitude;
                currentAlpha += step;
            }

            return directions;
        }
        
        protected ContextMap(float startMagnitude, Resolution resolution)
        {
            _resolution = resolution;
            Map = GenerateDirectionMap(startMagnitude, resolution);
        }

        private Vector3 GetPreviousDirection(int index) => index == 0 ? Map[Map.Length - 1] : Map[index - 1];

        private Vector3 GetNextDirection(int index) => index == Map.Length - 1 ? Map[0] : Map[index + 1];

        public int GetOppositeDirection(int index) =>
            index < Map.Length / 2 ? Map.Length / 2 + index : index - Map.Length / 2;
        
        public void UniformMap(int passes)
        {
            for (int n = 0; n < passes; n++)
            {
                for (int i = 0; i < Map.Length; i++)
                {
                    float mean =
                        (GetPreviousDirection(i).magnitude + GetNextDirection(i).magnitude) / 2;

                    Map[i] = Map[i].normalized * mean;
                }
            }
        }

        public void InsertValue(int index, float value, int propagation)
        {
            Vector3 newDirection = defaultDirections[_resolution][index] * Mathf.Clamp(value, 0, 1f);

            if (newDirection.magnitude > Map[index].magnitude)
                Map[index] = newDirection;

            for (int i = 1; i < propagation; i++)
            {
                float newValue = Mathf.Clamp(value / (1+i), 0f, 1f);
            
                Vector3 newDirectionL = defaultDirections[_resolution][Mathf.Abs((index - i) % Map.Length)] * newValue;
                Vector3 newDirectionR = defaultDirections[_resolution][Mathf.Abs((index + i) % Map.Length)] * newValue;
            
                if (newDirectionL.magnitude > Map[Mathf.Abs((index - i) % Map.Length)].magnitude)
                    Map[Mathf.Abs((index - i) % Map.Length)] = newDirectionL;
                
                if (newDirectionR.magnitude > Map[Mathf.Abs((index + i) % Map.Length)].magnitude)
                    Map[Mathf.Abs((index + i) % Map.Length)] = newDirectionR;
            }
        }

        public void Blend(ContextMap b)
        {
            for (int i = 0; i < Map.Length; i++)
            {
                Vector3 mean = (Map[i] + b.Map[i]) / 2;
                
                Map[i] = mean;
            }   
        }

        public static ContextMap Combine(ContextMap currentMap, ContextMap lastMap, float loseRateo)
        {
            for (int i = 0; i < currentMap.Map.Length; i++)
            {
                currentMap.Map[i] = currentMap.Map[i].magnitude >= lastMap.Map[i].magnitude
                    ? currentMap.Map[i]
                    : (lastMap.Map[i] * loseRateo).magnitude > (currentMap.Map[i]).magnitude 
                        ? lastMap.Map[i] * loseRateo 
                        : currentMap.Map[i];
            }

            return currentMap;
        }

        public static void Combine(out InterestMap finalInterest, out DangerMap finalDanger, List<InterestMap> interestMaps, List<DangerMap> dangerMaps)
        {
            InterestMap combinedInterest = interestMaps[0];
            DangerMap combinedDanger = dangerMaps[0];

            for(int j = 1; j < interestMaps.Count; j++)
            {
                for (int i = 0; i < interestMaps[j].Map.Length; i++)
                {
                    if(!(combinedInterest.Map[i].magnitude > interestMaps[j].Map[i].magnitude)) 
                        combinedInterest.Map[i] = interestMaps[j].Map[i];
                    
                    if(!(combinedDanger.Map[i].magnitude > dangerMaps[j].Map[i].magnitude))
                        combinedDanger.Map[i] = dangerMaps[j].Map[i];
                }
            }

            finalInterest = combinedInterest;
            finalDanger = combinedDanger;
        }
        
        public static Vector3 CalculateDirection(InterestMap interestMap, DangerMap dangerMap)
        {
            Vector3[][] safeDirections = new Vector3[interestMap.Map.Length][];
            
            int r = 0;
            int c = 0;

            for (int i = 0; i < interestMap.Map.Length; i++)
            {
                if (IsSafeDirection(interestMap.Map[i], dangerMap.Map[i]))
                {
                    if(c == 0)
                        safeDirections[r] = new Vector3[interestMap.Map.Length];
                    
                    safeDirections[r][c] = interestMap.Map[i].normalized * Mathf.Clamp(interestMap.Map[i].magnitude - dangerMap.Map[i].magnitude,0f,1f);
                    c++;
                }
                else
                {
                    c = 0;
                    r++;
                }
            }

            Vector3[] directions = safeDirections.
                Where(entry => entry != null).
                OrderByDescending(entry => entry.Aggregate((acc,next) => acc + next).magnitude).
                FirstOrDefault();

            return directions != null && directions.Length > 0 ? directions.Aggregate((acc, next) => acc + next).normalized : Vector3.zero;
        }

        private static bool IsSafeDirection(Vector3 interest, Vector3 danger) => interest.magnitude > danger.magnitude;
    }

    public class InterestMap : ContextMap
    {
        public InterestMap(float startMagnitude, Resolution resolution) : base(startMagnitude,resolution) { }
        
        public void DebugMap(Vector3 startPosition)
        {
            foreach (Vector3 t in Map)
            {
                Debug.DrawRay(startPosition,t * 10f,Color.green);
            }
        }
    }

    public class DangerMap : ContextMap
    {
        public DangerMap(float startMagnitude, Resolution resolution) : base(startMagnitude,resolution) { }
        
        public void DebugMap(Vector3 startPosition)
        {
            foreach (Vector3 t in Map)
            {
                Debug.DrawRay(startPosition,t * 10f,Color.red);
            }
        }
    }

    public abstract class ContextSteeringBehaviour : MonoBehaviour
    {
        public abstract void GetMaps(out DangerMap dangerMap, out InterestMap interestMap);

        private ActionLock _steeringBehaviourLock = new ActionLock();
        
        public ActionLock SteeringBehaviourLock
        {
            get => _steeringBehaviourLock;
            private set => _steeringBehaviourLock = value;
        }

        public void ActivateBehaviour() => _steeringBehaviourLock.RemoveLock();

        public void DeactivateBehaviour() => _steeringBehaviourLock.AddLock();
    }
}