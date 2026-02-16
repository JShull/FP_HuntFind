namespace FuzzPhyte.Game.HuntFind
{
    [System.Serializable]
    public enum HuntEquipmentActionType
    {
        None,
        Started,
        Finished,
        DoorOpened,
        DoorClosed,
        Paused,
        Resumed,
        Cancelled,
        Broken,
        Repaired
    }
}
