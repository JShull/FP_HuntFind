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
            ValidateRecognition(obj);
        }

        private void OnDoubleClick(
            PlacementObjectComponent obj,
            FP_PlacementSocketComponent socket)
        {
            ValidateRecognition(obj);
        }

        private void ValidateRecognition(PlacementObjectComponent obj)
        {
            if (Runner.CurrentObjective == null) return;

            var objective = Runner.CurrentObjective.ObjectiveData;

            if (objective.Type != HuntObjectiveType.Recognition)
                return;

            string objectID = obj.name; // or your custom ID source

            if (objective.ValidIDs.Contains(objectID))
            {
                Runner.RegisterCorrectAction();
            }
        }

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
