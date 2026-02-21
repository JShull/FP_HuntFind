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
        public Color TextColor;
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
        public Button ObjectiveButton;
        public AudioSource AudioSource;
        protected Coroutine animationCoroutine;
        public Image ActionIconRef;
        public Image ActionIconBackgroundRef;
        public List<Image>OtherPanelRefs = new List<Image>();
        [Space]
        public TextMeshProUGUI NumberIndexRef;
        public Image NumberIndexFillRef;
        [Space]
        [Header("Icons")]
        public List<HuntFindUIInfo> HuntUIItems = new List<HuntFindUIInfo>();
        public HuntFindUIInfo ActiveUIDetails;
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
                    ActiveUIDetails=item;
                    ActionIconRef.sprite = item.UIIcon;
                    ActionIconRef.color = item.UIColor;
                    for(int j=0;j< OtherPanelRefs.Count; j++)
                    {
                        var otherPanel = OtherPanelRefs[j];
                        otherPanel.color = item.UIBackgroundColor;
                    }
                    ActionIconBackgroundRef.color = item.UIBackgroundColor;
                    if (ObjectiveText != null)
                    {
                        ObjectiveText.color = item.TextColor;
                    }
                    if (ObjectiveButton != null)
                    {
                        //hover button color inverse of UIBackgroundColor
                        ObjectiveButton.colors = new ColorBlock
                        {
                            normalColor = ObjectiveButton.colors.normalColor,
                            highlightedColor = InvertColor(item.UIBackgroundColor),
                            pressedColor = ObjectiveButton.colors.pressedColor,
                            selectedColor = ObjectiveButton.colors.selectedColor,
                            disabledColor = ObjectiveButton.colors.disabledColor,
                            colorMultiplier = 1,
                            fadeDuration = 0.1f
                        };
                    }
                    if (item.AnimatorUIRef != null)
                    {
                        item.AnimatorUIRef.enabled = true;
                        item.AnimatorUIRef.color = item.UIBackgroundColor;
                    }
                    if (item.AnimatorUIItem != null)
                    {
                        item.AnimatorUIItem.playbackTime = 0;
                        item.AnimatorUIItem.Play(item.AnimationState, item.AnimationLayer);
                        if (animationCoroutine != null)
                        {
                            StopCoroutine(animationCoroutine);
                        }
                        animationCoroutine = StartCoroutine(AnimationCoroutine(result));
                    }
                    break;
                }
                
            }

            OnObjectiveClicked();
        }
        public static Color InvertColor(Color color)
        {
            // Subtract each color channel from 1.0
            color.r = 1.0f - color.r;
            color.g = 1.0f - color.g;
            color.b = 1.0f - color.b;
            // Keep the original alpha value
            return color;
        }
        protected IEnumerator AnimationCoroutine(HuntFindUIInfo anim)
        {
            yield return new WaitForSecondsRealtime(anim.AnimationDuration);
            if (anim.AnimatorUIItem != null)
            {
                anim.AnimatorUIItem.playbackTime = 0;
                anim.AnimatorUIItem.StopPlayback();
                anim.AnimatorUIRef.enabled = false;
            }
            else
            {
                Debug.LogError($"Requested some animation but it looks like we are missing an animator?! ID={anim.UniqueID}");
            }
            animationCoroutine = null;
        }
        
        public void UpdateRemainingUI(int index, int maxOut)
        {
            if (NumberIndexRef == null) return;
            NumberIndexRef.text = index.ToString("{0:N0}") + "|" + maxOut.ToString("{0:N0}");
            if (NumberIndexFillRef == null) return;
            float ratio = (float)index / (float)maxOut;
            NumberIndexFillRef.fillAmount = ratio;
        }
        public void OnObjectiveClicked()
        {
            Runner.PhrasePlayed();
            Runner.PlayInstructionAudio(AudioSource);
        }
    }
}
