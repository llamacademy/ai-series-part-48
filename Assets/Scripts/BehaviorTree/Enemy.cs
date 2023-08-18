using LlamAcademy.Sensors;
using RenownedGames.AITree;
using UnityEngine;

namespace LlamAcademy.BehaviorTree
{
    public class Enemy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player Player;
        [SerializeField] private BehaviourRunner BehaviorRunner;
        [SerializeField] private Transform SpitSpawnLocation;

        [SerializeField] private PlayerSensor ChaseSensor;
        [SerializeField] private PlayerSensor SpitSensor;
        [SerializeField] private PlayerSensor MeleeSensor;
        [SerializeField] private ImpactSensor ImpactSensor;

        [Header("Attack Config")]
        [SerializeField] [Range(0.33f, 5f)] private float MeleeCooldown = 2f;
        [SerializeField] [Range(1, 12f)] private float SpitCooldown = 5f;
        [SerializeField] [Range(3, 20f)] private float BounceCooldown = 8f;
        [SerializeField] [Range(5, 30f)] private float RollCooldown = 11f;

        [Header("Debug")] 
        [SerializeField] private bool IsInChaseRange;
        [SerializeField] private bool IsInMeleeRange;
        [SerializeField] private bool IsInSpitRange;
        
        [SerializeField] private float LastMeleeTime;
        [SerializeField] private float LastSpitTime;
        [SerializeField] private float LastBounceTime;
        [SerializeField] private float LastRollTime;

        private void Start()
        {
            ChaseSensor.OnPlayerEnter += ChaseSensorOnOnPlayerEnter;
            ChaseSensor.OnPlayerExit += ChaseSensorOnOnPlayerExit;

            SpitSensor.OnPlayerEnter += SpitSensorOnOnPlayerEnter;
            SpitSensor.OnPlayerExit += SpitSensorOnOnPlayerExit;
            
            MeleeSensor.OnPlayerEnter += MeleeSensorOnOnPlayerEnter;
            MeleeSensor.OnPlayerExit += MeleeSensorOnOnPlayerExit;
            
            ImpactSensor.OnCollision += ImpactSensorOnOnCollision;
        }

        private void Update()
        {
            if (BehaviorRunner == null)
            {
                return;
            }

            Blackboard blackboard = BehaviorRunner.GetBlackboard();

            SetBlackboardValue(blackboard, "Player", Player.transform);
            SetBlackboardValue(blackboard, "IsInChaseRange", IsInChaseRange);
            
            SetBlackboardValue(blackboard, "IsInMeleeRange", IsInMeleeRange);
            SetBlackboardValue(blackboard, "CanMelee", LastMeleeTime + MeleeCooldown <= Time.time);
            
            SetBlackboardValue(blackboard, "IsInSpitRange", IsInSpitRange);
            SetBlackboardValue(blackboard, "CanSpit", LastSpitTime + SpitCooldown <= Time.time);
            SetBlackboardValue(blackboard, "SpitSpawnPosition", SpitSpawnLocation.position);
            SetBlackboardValue(blackboard, "SpitSpawnRotation", SpitSpawnLocation.rotation);
            
            SetBlackboardValue(blackboard, "CanBounce", LastBounceTime + BounceCooldown <= Time.time);
            SetBlackboardValue(blackboard, "SelfPosition", transform.position);

            SetBlackboardValue(blackboard, "CanRoll", LastRollTime + RollCooldown <= Time.time);
        }

        private void SetBlackboardValue(Blackboard blackboard, string key, Transform transform)
        {
            if (blackboard.TryGetKey(key, out TransformKey transformKey))
            {
                transformKey.SetValue(transform);
            }
        }
        
        private void SetBlackboardValue(Blackboard blackboard, string key, bool boolValue)
        {
            if (blackboard.TryGetKey(key, out BoolKey boolKey))
            {
                boolKey.SetValue(boolValue);
            }
        }
        
        private void SetBlackboardValue(Blackboard blackboard, string key, Vector3 vector3Value)
        {
            if (blackboard.TryGetKey(key, out Vector3Key vector3Key))
            {
                vector3Key.SetValue(vector3Value);
            }
        }
        
        private void SetBlackboardValue(Blackboard blackboard, string key, Quaternion quaternionValue)
        {
            if (blackboard.TryGetKey(key, out QuaternionKey quaternionKey))
            {
                quaternionKey.SetValue(quaternionValue);
            }
        }

        public void OnRoll()
        {
            LastRollTime = Time.time;
            ImpactSensor.Collider.enabled = false;
        }
        
        public void OnMelee() => LastMeleeTime = Time.time;
        public void OnSpit() => LastSpitTime = Time.time;
        public void OnBounce() => LastBounceTime = Time.time;
        public void OnStartRoll() => ImpactSensor.Collider.enabled = true;
        private void ImpactSensorOnOnCollision(Collision collision) => OnRoll();
        private void ChaseSensorOnOnPlayerExit(Vector3 lastKnownPosition) => IsInChaseRange = false;
        private void ChaseSensorOnOnPlayerEnter(Transform player) => IsInChaseRange = true;
        private void SpitSensorOnOnPlayerExit(Vector3 lastKnownPosition) => IsInSpitRange = false;
        private void SpitSensorOnOnPlayerEnter(Transform player) => IsInSpitRange = true;
        private void MeleeSensorOnOnPlayerExit(Vector3 lastKnownPosition) => IsInMeleeRange = false;
        private void MeleeSensorOnOnPlayerEnter(Transform player) => IsInMeleeRange = true;
    }
}