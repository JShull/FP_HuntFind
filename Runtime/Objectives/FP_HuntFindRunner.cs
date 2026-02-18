namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using UnityEngine.Events;
    using FuzzPhyte.SystemEvent;
    using FuzzPhyte.Utility.EDU;
    public class FP_HuntFindRunner : MonoBehaviour
    {
        [Header("Active Objective")]
        public FP_HuntObjectiveState CurrentObjective { get; protected set; } = null;

        public delegate void HuntObjectiveDelegate(FP_HuntObjectiveState obj);
        public event HuntObjectiveDelegate OnObjectiveStarted;
        public event HuntObjectiveDelegate OnObjectiveCompleted;
        public UnityEvent OnCorrectActionPerformed;
        public void StartObjective(FP_HuntObjectiveState objective)
        {
            CurrentObjective = objective;

            OnObjectiveStarted?.Invoke(objective);
        }
        
        public void PhrasePlayed()
        {
            if (CurrentObjective == null)
                return;

            if (!CurrentObjective.HasPlayedOnce)
            {
                CurrentObjective.MarkPhrasePlayed();
                Debug.Log("Phrase played for the first time!");
            }
            else
            {
                Debug.Log("Phrase replayed.");
            }
        }
        public void PlayInstructionAudio(AudioSource source)
        {
            if (CurrentObjective?.ObjectiveData?.Instruction?.WordAudio != null)
            {
                source.PlayOneShot(CurrentObjective?.ObjectiveData?.Instruction?.WordAudio.AudioClip);
            }
        }
        public void RegisterCorrectAction()
        {
            if (CurrentObjective == null) return;

            var finished=CurrentObjective.IncrementProgress();

            Debug.Log($"Progress: {CurrentObjective.CurrentCount}/{CurrentObjective.RequiredCount}");

            if (finished)
            {
                CompleteObjective();
            }
            OnCorrectActionPerformed?.Invoke();
        }
        public void CompleteObjective()
        {
            if (CurrentObjective == null) return;

            CurrentObjective.Complete();
            OnObjectiveCompleted?.Invoke(CurrentObjective);
        }
    }
}
