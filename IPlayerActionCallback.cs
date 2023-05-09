using System.ServiceModel;
using System.ComponentModel;

public interface IPlayerActionCallback
{
    [OperationContract(IsOneWay = true)]
    void UpdateField(ServerResponsePayload paload);
}