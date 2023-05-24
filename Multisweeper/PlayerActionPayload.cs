public class PlayerActionPayload
{
    public PlayerActionType actionType { get; init; }
    public byte column { get; init; }
    public byte row { get; init; }
}