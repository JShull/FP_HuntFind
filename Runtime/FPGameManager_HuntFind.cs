namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine.Events;
    using FuzzPhyte.SystemEvent;
    public class FPGameManager_HuntFind : FPGenericGameUtility
    {
        [Header("HuntFind Core Systems")]
        public FP_HuntFindRunner HuntRunner;

        [Tooltip("All objectives in order (linear progression)")]
        public List<FP_HuntObjectiveState> ObjectiveSequence = new List<FP_HuntObjectiveState>();
        public float DelayBeforeNextObjective = 2f;
        protected bool pastFirstObjective = false;
        protected FP_HuntObjectiveState currentStateInstance;
        protected int currentIndex = 0;
        public int GetCurrentIndex => currentIndex;
        [Tooltip("Event is triggered right at 0, we then use the delay to start the next objective")]
        public UnityEvent BeforeNextObjective;

        // ---------------------------------------------------
        // FP_Game Lifecycle
        // ---------------------------------------------------

        public override void StartEngine()
        {
            base.StartEngine();

            Debug.Log("HuntFind Game Started!");

            StartNextObjective();
        }

        public override void StopEngine()
        {
            base.StopEngine();

            Debug.Log("HuntFind Game Finished!");
        }

        #region Objective Flow

        protected void StartNextObjective()
        {
            if (ObjectiveSequence == null || ObjectiveSequence.Count == 0)
            {
                Debug.LogWarning("No HuntFind objectives assigned.");
                StopEngine();
                return;
            }

            if (currentIndex >= ObjectiveSequence.Count)
            {
                Debug.Log("All objectives completed!");
                StopEngine();
                return;
            }

            currentStateInstance = ObjectiveSequence[currentIndex];
            BeforeNextObjective?.Invoke();
            if (!pastFirstObjective)
            {
                HuntRunner.StartObjective(currentStateInstance);
                Debug.Log($"Started Objective #{currentIndex + 1}: {currentStateInstance.Instruction}");
                pastFirstObjective = true;
            }
            else
            {
                StartCoroutine(DelayNextObjective());
                Debug.Log($"Started Objective #{currentIndex + 1}: {currentStateInstance.Instruction}");
            }
           
        }
        IEnumerator DelayNextObjective()
        {
            yield return new WaitForSecondsRealtime(DelayBeforeNextObjective);
            HuntRunner.StartObjective(currentStateInstance);
        }
        #endregion

        #region Event Wiring

        protected virtual void OnEnable()
        {
            if (HuntRunner != null)
            {
                HuntRunner.OnObjectiveCompleted += HandleObjectiveCompleted;
            }
        }

        protected virtual void OnDisable()
        {
            if (HuntRunner != null)
            {
                HuntRunner.OnObjectiveCompleted -= HandleObjectiveCompleted;
            }
        }

        protected void HandleObjectiveCompleted(FP_HuntObjectiveState obj)
        {
            Debug.Log($"Objective Completed: {obj.Instruction}");

            currentIndex++;

            StartNextObjective();
        }
        #endregion
    }
}
