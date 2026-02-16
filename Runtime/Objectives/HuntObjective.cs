namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using FuzzPhyte.Utility.EDU;
    [CreateAssetMenu(fileName = "HuntObjective", menuName = "FuzzPhyte/Game/HuntFind/HuntObjective")]
    public class HuntObjective : FP_Data
    {
        [Header("Objective Type")]
        public HuntObjectiveType Type;

        [Header("Language Instruction")]
        public FP_Vocab Instruction;

        [Header("Matching Rules")]
        public HuntMatchMode MatchMode = HuntMatchMode.SpecificID;
        public List<string> ValidIDs = new List<string>();
        public List<string> ValidTags = new List<string>();
        [Header("Action Objective Settings")]
        public HuntEquipmentActionType RequiredAction = HuntEquipmentActionType.None;

        [Header("Multi-Step Destination")]
        public string SecondaryID;

    }
}
