using System.ServiceModel;
using System.ComponentModel;

[ServiceContract(Namespace = "http://EintrachtJava.Multisweeper", SessionMode=SessionMode.Required,
                 CallbackContract=typeof(IPlayerActionCallback))]
public interface IPlayerAction
{
    [OperationContract(IsOneWay = true)]
    void PlayerAction(PlayerActionPayload payload);
}