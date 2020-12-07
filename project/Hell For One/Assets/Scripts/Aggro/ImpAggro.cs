using UnityEngine;

namespace Aggro
{
    // TODO :- Refactor Aggro
    public class ImpAggro : MonoBehaviour
    {
        // #region Fields
        //
        // private static float aggroTime = 60f;
        //
        // private static float timer = aggroTime;
        // private static float supportAggroMultiplier = 1.08f;
        // private static float rectuitAggroMultiplier = 1.1f;
        // private static float orderGivenMultiplier = 1.2f;
        // // TODO - remove serializefield after testing
        // [SerializeField]
        // private float aggro = 1f;
        //
        // private Stats.Type type;
        //
        // private GroupAggro[] groupAggros;
        // private GroupManager[] groupManagers;
        // private Reincarnation reincarnation;
        // private NormalCombat normalCombat;
        // private GroupFinder groupFinder;
        // private Stats stats;
        //
        // Coroutine aggroDecreasingCr = null;
        //
        // #endregion
        //
        // #region Delegates and events
        //
        // public delegate void OnImpAggroChanged(ImpAggro sender, float oldValue);
        // public event OnImpAggroChanged onImpAggroChanged;
        //
        // #region Methods
        //
        // private void RiseOnImpAggroChanged(float oldValue)
        // {
        //     onImpAggroChanged?.Invoke(this,oldValue);
        // }
        //
        // #endregion
        //
        // #endregion
        //
        // #region Properties
        //
        // public float Aggro { get => aggro; private set => aggro = value; }
        //
        // #endregion
        //
        // #region Unity methods
        //
        // private void Awake()
        // {
        //     stats = gameObject.GetComponent<Stats>();
        //     reincarnation = gameObject.GetComponent<Reincarnation>();
        //     //combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        //     
        //     // TODO :- Register to CombatSystem events
        //     //normalCombat = gameObject.GetComponentInChildren<NormalCombat>();
        //     
        //     groupFinder = gameObject.GetComponent<GroupFinder>();
        //     //support = GetComponent<Support>();
        // }
        //
        // private void OnEnable()
        // {
        //     reincarnation.onReincarnation += OnReincarnation;
        //     reincarnation.onLateReincarnation += OnLateReincarnation;
        //     stats.onDeath += OnDeath;
        //
        //     // TODO :- Register to CombatSystem events
        //     //normalCombat.onAttackHit += OnAttackHit;
        //     
        //     // TODO - Test for player
        //     groupFinder.onGroupFound += OnGroupFound;
        // }
        //
        // private void OnDisable()
        // {
        //     reincarnation.onReincarnation -= OnReincarnation;
        //     reincarnation.onLateReincarnation -= OnLateReincarnation;
        //     stats.onDeath -= OnDeath;
        //     normalCombat.onAttackHit -= OnAttackHit;
        //     // TODO - Test for player
        //     groupFinder.onGroupFound -= OnGroupFound;
        // }
        //
        // private void Start()
        // {
        //     type = GetComponent<Stats>().ThisUnitType;
        //     
        //     if(type == Stats.Type.Player)
        //         Initialize();
        // }
        //
        // #endregion
        //
        // #region Methods
        //
        // private void Initialize()
        // {
        //     // TODO - these should be static
        //     groupAggros = new GroupAggro[GroupsManager.Instance.Groups.Length];
        //     groupManagers = new GroupManager[GroupsManager.Instance.Groups.Length];
        //     
        //     int i = 0;
        //     
        //     foreach (GameObject group in GroupsManager.Instance.Groups)
        //     {
        //         groupAggros[i] = group.GetComponent<GroupAggro>();
        //         groupManagers[i] = group.GetComponent<GroupManager>();
        //
        //         group.GetComponent<GroupBehaviour>().onOrderChanged += OnOrderChanged;
        //         
        //         i++;
        //     }
        //
        //     if (aggroDecreasingCr == null)
        //     {
        //         aggroDecreasingCr = StartCoroutine(AggroDecreasing());
        //     }  
        // }
        //
        // private void RaiseAggro(float n)
        // {
        //     float oldValue = aggro;
        //     
        //     aggro *= n;
        //     
        //     RiseOnImpAggroChanged(oldValue);
        // }
        //
        // private void LowerAggro(float n)
        // {
        //     float oldValue = aggro;
        //     
        //     aggro = aggro / n;
        //
        //     if (aggro < 1f)
        //         aggro = 1f;
        //     
        //     RiseOnImpAggroChanged(oldValue);
        // }
        //
        // #endregion
        //
        // #region Event handlers
        //
        // private void OnGroupFound(GroupFinder groupFinder)
        // {
        //     GetComponent<Support>().onStaySupport += OnStaySupport;
        //     GetComponent<Recruit>().onStayRecruit += OnStayRecruit;
        // }
        //
        // private void OnStayRecruit(Recruit sender)
        // {
        //     float oldValue = aggro;
        //     
        //     aggro *= rectuitAggroMultiplier;
        //     
        //     RiseOnImpAggroChanged(oldValue);
        // }
        //
        // private void OnStaySupport(Support sender)
        // {
        //     float oldValue = aggro;
        //     
        //     aggro *= supportAggroMultiplier;
        //     
        //     RiseOnImpAggroChanged(oldValue);
        // }
        //
        // private void OnOrderChanged(GroupBehaviour sender, GroupBehaviour.State newState)
        // { 
        //     RaiseAggro(orderGivenMultiplier);
        // }
        //
        // private void OnReincarnation(GameObject newPlayer)
        // {
        //     newPlayer.GetComponent<Support>().onStaySupport -= OnStaySupport;
        //     newPlayer.GetComponent<Recruit>().onStayRecruit -= OnStayRecruit;
        // }
        //
        // private void OnLateReincarnation(GameObject newPlayer)
        // {
        //     type = newPlayer.GetComponent<Stats>().ThisUnitType;
        //
        //     if (type != Stats.Type.Player)
        //     {
        //         Debug.LogError(transform.root.gameObject.name + " " + name + " OnLateReincarnation newPlayer is not Player in Stats");
        //         return;
        //     }
        //     
        //     Initialize();
        // }
        //
        // private void OnDeath(Stats sender)
        // {
        //     if (sender.ThisUnitType != Stats.Type.Player)
        //     {
        //         sender.gameObject.GetComponent<Support>().onStaySupport -= OnStaySupport;
        //         sender.gameObject.GetComponent<Recruit>().onStayRecruit -= OnStayRecruit;
        //         
        //         float aggroBeforeDeath = aggro;
        //         
        //         aggro = 1;
        //         
        //         RiseOnImpAggroChanged(aggroBeforeDeath);
        //     }
        //     else
        //     {
        //         StopAllCoroutines();
        //
        //         foreach (GameObject group in GroupsManager.Instance.Groups)
        //         { 
        //             group.GetComponent<GroupBehaviour>().onOrderChanged -= OnOrderChanged;
        //         }    
        //     }
        // }
        //
        // private void OnAttackHit(GenericAttack attack, GenericIdle targetGenericIdle)
        // {
        //     if(attack.CanRiseAggro(type))
        //         RaiseAggro(attack.AggroModifier);
        //     else
        //     {
        //         Debug.LogError(this.transform.name + " " + this.name + " is trying to increase aggro of a not NormalAttack " );
        //     }
        // }
        //
        // #endregion
        //
        // #region Coroutines
        //
        // // TODO - Create a static event in ImpAggro to raise in this Coroutine in order to avoid imp . GetComponent
        // private IEnumerator AggroDecreasing()
        // {
        //     while (true)
        //     {
        //         yield return new WaitForSeconds(1f);
        //
        //         timer--;
        //
        //         if (!(timer <= 0f)) continue;
        //         
        //         // TODO - if only the player can lower aggro, then this coroutine should run only in player.
        //         // only the player will reduce the aggro to everyone
        //         if (type != Stats.Type.Player) continue;
        //
        //         // LINQ expression that returns max aggro in groupaggros
        //         float maxAggro = groupAggros.Select(groupAggro => groupAggro.GroupAggroValue).Concat(new float[] {1}).Max();
        //
        //         if (maxAggro < aggro)
        //             maxAggro = aggro;
        //
        //         // the max aggro group will have 10 as new value
        //         foreach (GroupManager groupManager in groupManagers)
        //         {
        //             foreach (GameObject imp in groupManager.Imps)
        //             {
        //                 if (imp)
        //                     imp.GetComponent<ImpAggro>().LowerAggro(maxAggro / groupManager.ImpsInGroupNumber * 10);
        //             }
        //         }
        //         
        //         // for the player
        //         LowerAggro(maxAggro / 10f);
        //
        //         timer = aggroTime;
        //     }
        // }
        //
        // #endregion
    }
}
