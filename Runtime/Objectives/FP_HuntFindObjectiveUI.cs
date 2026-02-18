namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using TMPro;
    public class FP_HuntFindObjectiveUI : MonoBehaviour
    {
        public FP_HuntFindRunner Runner;
        public TextMeshProUGUI ObjectiveText;
        public AudioSource AudioSource;
        [Space]
        [Header("Icons")]
        public GameObject HuntTypeActionIcon;
        public GameObject HuntTypeRecognitionIcon;
        public GameObject HuntTypeMultiStepIcon;
      
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
            //set text
            ObjectiveText.text = Runner.CurrentObjective.Instruction;
            //set icon
            if (HuntTypeActionIcon != null && HuntTypeRecognitionIcon != null && HuntTypeMultiStepIcon != null)
            {
                switch (Runner.CurrentObjective.ObjectiveData.Type)
                {
                    case HuntObjectiveType.Action:
                        HuntTypeActionIcon.SetActive(true);
                        HuntTypeRecognitionIcon.SetActive(false);
                        HuntTypeMultiStepIcon.SetActive(false);
                        break;
                    case HuntObjectiveType.Recognition:
                        HuntTypeActionIcon.SetActive(false);
                        HuntTypeRecognitionIcon.SetActive(true);
                        HuntTypeMultiStepIcon.SetActive(false);
                        break;
                    case HuntObjectiveType.Multistep:
                        HuntTypeActionIcon.SetActive(false);
                        HuntTypeRecognitionIcon.SetActive(false);
                        HuntTypeMultiStepIcon.SetActive(true);
                        break;
                }
            }
            OnObjectiveClicked();
        }
        [ContextMenu("Play Objective Audio")]
        public void OnObjectiveClicked()
        {
            Runner.PhrasePlayed();
            Runner.PlayInstructionAudio(AudioSource);
        }
    }
}
