namespace FuzzPhyte.Game.HuntFind
{
    using Mono.Cecil.Cil;
    using UnityEngine;
    /// <summary>
    /// Runtime-only state container for a HuntObjective.
    /// Unity Component side of managing our data using the HuntObjective SO
    /// </summary>
    public class FP_HuntObjectiveState : MonoBehaviour
    {
        [SerializeField] protected HuntObjective objectiveData;
        public HuntObjective ObjectiveData => objectiveData;
        [Tooltip("What we want to display for our instructions")]
        public string Instruction = "No Instructions";
        public int CurrentCount { get; protected set; } = 0;

        public int RequiredCount { get => requiredCount; }
        [SerializeField] protected int requiredCount =1;
        public bool HasPlayedOnce { get; protected set; } = false;
        public bool IsCompleted { get; protected set; } = false;
        public void Initialize(HuntObjective objective, string instructions ="No Instructions",int requiredAmount=1)
        {
            objectiveData = objective;
            requiredCount = requiredAmount;
            Instruction= instructions;
            ResetData();
        }
        public void ResetData()
        {
            CurrentCount = 0;
            HasPlayedOnce = false;
            IsCompleted = false;
        }
        public void MarkPhrasePlayed()
        {
            HasPlayedOnce = true;
        }

        public bool IncrementProgress()
        {
            CurrentCount++;

            if (CurrentCount >= RequiredCount)
            {
                IsCompleted = true;
                return true;
            }
            return false;
        }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
}
