namespace FuzzPhyte.Game.HuntFind
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    /// <summary>
    /// Runtime-only state container for a HuntObjective.
    /// Unity Component side of managing our data using the HuntObjective SO
    /// </summary>
    public class FP_HuntObjectiveState : MonoBehaviour
    {
        [SerializeField] protected HuntObjective objectiveData;
        public HuntObjective ObjectiveData => objectiveData;
        public AudioSource InstructionAudioSource;
        public bool SetInstructionFromVocabOnAwake = false;
        [Tooltip("What we want to display for our instructions")]
        public string Instruction = "No Instructions";
        public int CurrentCount { get; protected set; } = 0;

        public int RequiredCount { get => requiredCount; }
        [SerializeField] protected int requiredCount =1;
        public bool HasPlayedOnce { get; protected set; } = false;
        public bool IsCompleted { get; protected set; } = false;
        public UnityEvent AfterInstructionFinishedEvent;
        public UnityEvent OnCompletionEventFirstTime;
        public void Awake()
        {
            if(SetInstructionFromVocabOnAwake)
            {
                SetInstructionFromVocab();
            }
        }
        public void Initialize(HuntObjective objective, string instructions ="No Instructions",int requiredAmount=1)
        {
            objectiveData = objective;
            requiredCount = requiredAmount;
            Instruction= instructions;
            ResetData();
        }
        public void SetInstructionFromVocab()
        {
            Instruction = objectiveData.Instruction != null ? objectiveData.Instruction.Word : "No Instructions";
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
                if (!IsCompleted)
                {
                    OnCompletionEventFirstTime?.Invoke();
                }
                IsCompleted = true;
                return true;
            }
            return false;
        }

        public void Complete()
        {
            if (!IsCompleted)
            {
                OnCompletionEventFirstTime?.Invoke();
            }
            IsCompleted = true;
        }
        /// <summary>
        /// Plays Audio for the instruction if it exists. Can be called by the runner or other systems to trigger audio when needed.
        /// </summary>
        public void PlayInstructionAudio()
        {
            if (InstructionAudioSource == null) return;
            if (objectiveData.Instruction == null) return;
            if (objectiveData.Instruction.WordAudio.AudioClip == null) return;
            InstructionAudioSource.PlayOneShot(objectiveData.Instruction.WordAudio.AudioClip);
            StartCoroutine(AfterAudioPlayedEvent(objectiveData.Instruction.WordAudio.AudioClip.length+0.25f));
        }
        IEnumerator AfterAudioPlayedEvent(float value)
        {
            yield return new WaitForSeconds(value);
            AfterInstructionFinishedEvent?.Invoke();
        }
    }
}
