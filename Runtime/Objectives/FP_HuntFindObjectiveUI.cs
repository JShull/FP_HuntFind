namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using TMPro;
    public class FP_HuntFindObjectiveUI : MonoBehaviour
    {
        public FP_HuntFindRunner Runner;
        public TextMeshProUGUI ObjectiveText;
        public AudioSource AudioSource;

        public void OnEnable()
        {
            if (Runner != null)
            {
                Runner.OnObjectiveStarted += OnObjectiveStarted;
            }
        }
        public void OnDisable()
        {
            if (Runner != null)
            {
                Runner.OnObjectiveStarted -= OnObjectiveStarted;
            }
        }
        protected void OnObjectiveStarted(FP_HuntObjectiveState obj)
        {
            Refresh();
        }
        public void Refresh()
        {
            if (Runner.CurrentObjective == null)
                return;

            ObjectiveText.text =
                Runner.CurrentObjective.Instruction;
        }
        [ContextMenu("Play Objective Audio")]
        public void OnObjectiveClicked()
        {
            Runner.PhrasePlayed();
            Runner.PlayInstructionAudio(AudioSource);
        }
    }
}
