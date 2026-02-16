namespace FuzzPhyte.Game.HuntFind
{
    using UnityEngine;
    using FuzzPhyte.SystemEvent;
    using FuzzPhyte.Tools;
    using FuzzPhyte.Tools.Connections;
    using FuzzPhyte.Placement.Interaction;
    public class FP_HuntWorldListener : MonoBehaviour
    {
        public FP_HuntFindRunner Runner;
        public FP_PlacementInteractionBehaviour PlacementBehaviour;

        protected virtual void OnEnable()
        {
            if (PlacementBehaviour != null)
            {
                PlacementBehaviour.singleClickEvent
                    .AddListener(OnSingleClick);

                PlacementBehaviour.doubleClickEvent
                    .AddListener(OnDoubleClick);

                PlacementBehaviour.dragEndSocketSuccessEvent
                    .AddListener(OnSocketSuccess);

                PlacementBehaviour.dragEndMovedLocationEvent
                    .AddListener(OnMovedLocation);
            }
        }

        protected virtual void OnDisable()
        {
            if (PlacementBehaviour != null)
            {
                PlacementBehaviour.singleClickEvent
                    .RemoveListener(OnSingleClick);

                PlacementBehaviour.doubleClickEvent
                    .RemoveListener(OnDoubleClick);

                PlacementBehaviour.dragEndSocketSuccessEvent
                    .RemoveListener(OnSocketSuccess);

                PlacementBehaviour.dragEndMovedLocationEvent
                    .RemoveListener(OnMovedLocation);
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
