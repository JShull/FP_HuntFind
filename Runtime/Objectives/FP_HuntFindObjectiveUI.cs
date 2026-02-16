namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using TMPro;
    public class FP_HuntFindObjectiveUI : MonoBehaviour
    {
        public FP_HuntFindRunner Runner;
        public TextMeshProUGUI ObjectiveText;
        public AudioSource AudioSource;

        public void Refresh()
        {
            if (Runner.CurrentObjective == null)
                return;

            ObjectiveText.text =
                Runner.CurrentObjective.Instruction;
        }

        public void OnObjectiveClicked()
        {
            Runner.PhrasePlayed();
            Runner.PlayInstructionAudio(AudioSource);
        }
    }
}
