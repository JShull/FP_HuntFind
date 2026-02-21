namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using TMPro;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.UI;
    using System.Collections;

    [Serializable]
    public struct HuntFindUIInfo
    {
        public int UniqueID;
        public HuntObjectiveType HuntType;
        //public GameObject MainIcon;
        public Color UIColor;
        public Color UIBackgroundColor;
        public Sprite UIIcon;
        public Animator AnimatorUIItem;
        public Image AnimatorUIRef;
        public string AnimationState;
        public int AnimationLayer;
        public float AnimationDuration;
    }
    public class FP_HuntFindObjectiveUI : MonoBehaviour
    {
        public FP_HuntFindRunner Runner;
        public TextMeshProUGUI ObjectiveText;
        public AudioSource AudioSource;
        protected Coroutine animationCoroutine;
        public Image ActionIconRef;
        public Image ActionIconBackgroundRef;
        public List<Image>OtherPanelRefs = new List<Image>();
        [Space]
        [Header("Icons")]
        //public GameObject HuntTypeActionIcon;
        //public GameObject HuntTypeRecognitionIcon;
        //public GameObject HuntTypeMultiStepIcon;
        public List<HuntFindUIInfo> HuntUIItems = new List<HuntFindUIInfo>();
        
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
            //pull info
            if (HuntUIItems.Count <= 0) return; 
            if (ActionIconRef == null || ActionIconBackgroundRef == null) return;

            //go through the list and find a match for the type
            var result = HuntUIItems.FirstOrDefault(huntType => huntType.HuntType == Runner.CurrentObjective.ObjectiveData.Type);
            for(int i = 0; i < HuntUIItems.Count; i++)
            {
                var item = HuntUIItems[i];
                if (item.UniqueID == result.UniqueID)
                {
                    //set the color and sprite
                    ActionIconRef.sprite = item.UIIcon;
                    ActionIconRef.color = item.UIColor;
                    for(int j=0;j< OtherPanelRefs.Count; j++)
                    {
                        var otherPanel = OtherPanelRefs[j];
                        otherPanel.color = item.UIColor;
                    }
                    ActionIconBackgroundRef.color = item.UIBackgroundColor;
                    
                    if (item.AnimatorUIRef != null)
                    {
                        item.AnimatorUIRef.color = item.AnimatorUIRef.color;
                        item.AnimatorUIRef.enabled = true;
                    }
                    if (item.AnimatorUIItem != null)
                    {
                        item.AnimatorUIItem.Play(item.AnimationState, item.AnimationLayer);
                        if (animationCoroutine != null)
                        {
                            StopCoroutine(animationCoroutine);
                        }
                        animationCoroutine = StartCoroutine(AnimationCoroutine(result));
                    }
                }
                else
                { 
                    if (item.AnimatorUIRef != null)
                    {
                        item.AnimatorUIRef.enabled = false;
                    }
                }
            }

            OnObjectiveClicked();
        }
        protected IEnumerator AnimationCoroutine(HuntFindUIInfo anim)
        {
            yield return new WaitForSecondsRealtime(anim.AnimationDuration);
            if (anim.AnimatorUIItem != null)
            {
                anim.AnimatorUIItem.StopPlayback();
                anim.AnimatorUIItem.playbackTime = 0;
                anim.AnimatorUIRef.enabled = false;
            }
            else
            {
                Debug.LogError($"Requested some animation but it looks like we are missing an animator?! ID={anim.UniqueID}");
            }
            animationCoroutine = null;
        }
        [ContextMenu("Play Objective Audio")]
        public void OnObjectiveClicked()
        {
            Runner.PhrasePlayed();
            Runner.PlayInstructionAudio(AudioSource);
        }
    }
}
