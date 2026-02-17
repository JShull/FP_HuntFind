namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using FuzzPhyte.Tools;
    using FuzzPhyte.Placement.Interaction;
    public class FP_HuntWorldListener : MonoBehaviour
    {
        [Header("Core Systems")]
        public FP_HuntFindRunner Runner;
        public FP_PlacementInteractionBehaviour PlacementBehaviour;

        /// <summary>
        /// JOHN
        /// Could update this to be generic to FP_EquipmentBase, 
        /// and introduce an IFPHuntEquipmentNotifier interface
        /// </summary>
        [Header("Misc. Equipment References")]
        public FP_EquipmentMicrowave[] Microwaves;
        protected virtual void OnEnable()
        {
            SubscribePlacement();
            SubscribeEquipment();
        }

        protected virtual void OnDisable()
        {
            UnsubscribePlacement();
            UnsubscribeEquipment();
        }
        protected void SubscribePlacement()
        {
            if (PlacementBehaviour == null) return;

            PlacementBehaviour.singleClickEvent
                .AddListener(OnSingleClick);
            PlacementBehaviour.doubleClickEvent
                .AddListener(OnDoubleClick);

            PlacementBehaviour.dragEndSocketSuccessEvent
                .AddListener(OnSocketSuccess);
        }
        protected void UnsubscribePlacement()
        {
            if (PlacementBehaviour == null) return;

            PlacementBehaviour.singleClickEvent
                .RemoveListener(OnSingleClick);
            PlacementBehaviour.doubleClickEvent
                .RemoveListener(OnDoubleClick);

            PlacementBehaviour.dragEndSocketSuccessEvent
                .RemoveListener(OnSocketSuccess);
        }
        void SubscribeEquipment()
        {
            if (Microwaves == null) return;

            foreach (var microwave in Microwaves)
            {
                if (microwave == null) continue;

                microwave.OnStarted.AddListener(OnMicrowaveStarted);
                microwave.OnFinished.AddListener(OnMicrowaveFinished);
                microwave.OnDoorOpen.AddListener(OnMicrowaveDoorOpened);
            }
        }
        void UnsubscribeEquipment()
        {
            if (Microwaves == null) return;

            foreach (var microwave in Microwaves)
            {
                if (microwave == null) continue;

                microwave.OnStarted.RemoveListener(OnMicrowaveStarted);
                microwave.OnFinished.RemoveListener(OnMicrowaveFinished);
                microwave.OnDoorOpen.RemoveListener(OnMicrowaveDoorOpened);
            }
        }
        #region Click Recognition

        private void OnSingleClick(
            PlacementObjectComponent obj,
            FP_PlacementSocketComponent socket)
        {
            Debug.LogWarning($"Single click listener: {obj.name} with socket?");
            ValidateRecognition(obj);
        }

        private void OnDoubleClick(
            PlacementObjectComponent obj,
            FP_PlacementSocketComponent socket)
        {
            ValidateRecognition(obj);
        }
        #region Validation Methods
        private void ValidateRecognition(PlacementObjectComponent obj)
        {
            if (Runner.CurrentObjective == null) return;

            var objective = Runner.CurrentObjective.ObjectiveData;

            if (objective.Type != HuntObjectiveType.Recognition)
                return;

            // need to use our match type now to determine if we should be checking ID, tag, or category
            bool isValid = false;
            switch(objective.MatchMode)
            {
                case HuntMatchMode.AnyOfIDs:
                    isValid=ValidateRecognitionByID(obj, Runner.CurrentObjective);
                    break;
                case HuntMatchMode.ByTag:
                    isValid=ValidateRecognitionByTag(obj, Runner.CurrentObjective);
                    break;
                case HuntMatchMode.ByCategory:
                    isValid=ValidateRecognitionByCategory(obj, Runner.CurrentObjective);
                    break;
            }
            if (isValid)
            {
                Runner.RegisterCorrectAction();
            }
        }
        /// <summary>
        /// ID uses gameobject name
        /// </summary>
        /// <param name="placementOBJ"></param>
        /// <param name="huntOBJ"></param>
        /// <returns></returns>
        private bool ValidateRecognitionByID(PlacementObjectComponent placementOBJ,FP_HuntObjectiveState huntOBJ)
        {
            for(int i=0;i< huntOBJ.ObjectiveData.ValidIDs.Count; i++)
            {
                var id = huntOBJ.ObjectiveData.ValidIDs[i];
                if (placementOBJ.name == id)
                {
                     return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Using Unity Tag System, check our huntOBJ valid tags against the tag of the placement object
        /// </summary>
        /// <param name="placementOBJ"></param>
        /// <param name="huntOBJ"></param>
        /// <returns></returns>
        private bool ValidateRecognitionByTag(PlacementObjectComponent placementOBJ, FP_HuntObjectiveState huntOBJ)
        {
            for (int i = 0; i < huntOBJ.ObjectiveData.ValidTags.Count; i++)
            {
                var tag = huntOBJ.ObjectiveData.ValidTags[i];
                if (placementOBJ.CompareTag(tag))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Using our custom category system
        /// </summary>
        /// <param name="placementOBJ"></param>
        /// <param name="huntOBJ"></param>
        /// <returns></returns>
        private bool ValidateRecognitionByCategory(PlacementObjectComponent placementOBJ, FP_HuntObjectiveState huntOBJ)
        {
            // looking for any singular match across two lists

            for(int i=0;i< placementOBJ.PlacementData.Categories.Count; i++)
            {
                var category = placementOBJ.PlacementData.Categories[i];
                if (huntOBJ.ObjectiveData.Categories.Contains(category))
                {
                    //Runner.RegisterCorrectAction();
                    return true;
                }
            }
            return false;
        }
        #endregion
        #endregion
        #region Equipment Validation

        void OnMicrowaveStarted(FP_EquipmentMicrowave microwave)
        {
            ValidateEquipmentAction(
                microwave.name,
                HuntEquipmentActionType.Started);
        }

        void OnMicrowaveFinished(FP_EquipmentMicrowave microwave)
        {
            ValidateEquipmentAction(
                microwave.name,
                HuntEquipmentActionType.Finished);
        }

        void OnMicrowaveDoorOpened(FP_EquipmentMicrowave microwave)
        {
            ValidateEquipmentAction(
                microwave.name,
                  HuntEquipmentActionType.DoorOpened);
        }

        void ValidateEquipmentAction(string equipmentID, HuntEquipmentActionType actionType)
        {
            if (Runner.CurrentObjective == null)
                return;

            var objective = Runner.CurrentObjective.ObjectiveData;

            // Must be an Action objective
            if (objective.Type != HuntObjectiveType.Action)
                return;

            // Equipment must match
            if (!objective.ValidIDs.Contains(equipmentID))
                return;

            // Action type must match
            if (objective.RequiredAction != actionType)
                return;

            Debug.Log(
                $"[HuntFind] Equipment Action Matched: {equipmentID} -> {actionType}"
            );

            Runner.RegisterCorrectAction();
        }

        #endregion
        #region Placement System
        private void OnSocketSuccess(
            PlacementObjectComponent obj,
            FP_PlacementSocketComponent socket)
        {
            Debug.LogWarning($"Socket Success: {obj.name} -> {socket.name}");
            if (Runner.CurrentObjective == null) return;

            var objective = Runner.CurrentObjective.ObjectiveData;

            if (objective.Type != HuntObjectiveType.Multistep)
                return;

            string objectID = obj.name;
            string socketID = socket != null ? socket.name : "";

            bool objectMatch =
                objective.ValidIDs.Contains(objectID);

            bool socketMatch =
                string.IsNullOrEmpty(objective.SecondaryID) ||
                objective.SecondaryID == socketID;

            if (objectMatch && socketMatch)
            {
                Runner.RegisterCorrectAction();
            }
        }

        private void OnMovedLocation(
            PlacementObjectComponent obj,
            FP_PlacementSocketComponent socket)
        {
            // Optional: handle surface drop multi-step logic here
        }
        #endregion
    }
}
