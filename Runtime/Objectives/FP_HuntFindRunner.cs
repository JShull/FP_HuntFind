namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;
    using FuzzPhyte.SystemEvent;
    using FuzzPhyte.Utility.EDU;
    public class FP_HuntFindRunner : MonoBehaviour
    {
        [Header("Active Objective")]
        public FP_HuntObjectiveState CurrentObjective;
        public FP_HuntObjectiveState EndGameObjectiveFrench;
        public FP_HuntObjectiveState EndGameObjectiveSpanish;
        public delegate void HuntObjectiveDelegate(FP_HuntObjectiveState obj);
        public event HuntObjectiveDelegate OnObjectiveStarted;
        public event HuntObjectiveDelegate OnObjectiveCompleted;
        public UnityEvent OnCorrectActionPerformed;
        public void SetStartObjective(FP_HuntObjectiveState objective,float delayTime=1.5f)
        {
            CurrentObjective = objective;
            StartCoroutine(DelayObjectiveStart(delayTime));
        }
        public void StartObjectiveImmediately(FP_HuntObjectiveState objective)
        {
            CurrentObjective = objective;
            OnObjectiveStarted?.Invoke(objective);
        }
        protected IEnumerator DelayObjectiveStart(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            OnObjectiveStarted?.Invoke(CurrentObjective);
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
        public void EndGameSetup(bool French)
        {
            if (French)
            {
                if (EndGameObjectiveFrench != null)
                {
                    CurrentObjective = EndGameObjectiveFrench;
                }
            }
            else
            {
                if (EndGameObjectiveSpanish != null)
                {
                    CurrentObjective = EndGameObjectiveSpanish;
                }
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

            Debug.Log($"Done? {finished} Progress: {CurrentObjective.CurrentCount}/{CurrentObjective.RequiredCount}");

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
