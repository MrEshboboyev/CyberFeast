namespace BuildingBlocks.Abstractions.Messaging.PersistMessage;

/// <summary>
/// Represents a stored message with properties like ID, data type, data, delivery type, and status.
/// </summary>
/// <param name="id">The unique identifier of the message.</param>
/// <param name="dataType">The type of the data.</param>
/// <param name="data">The actual message data.</param>
/// <param name="deliveryType">The delivery type of the message.</param>
public class StoreMessage(
    Guid id,
    string dataType,
    string data,
    MessageDeliveryType deliveryType)
{
    public Guid Id { get; private set; } = id;
    public string DataType { get; private set; } = dataType;
    public string Data { get; private set; } = data;
    public DateTime Created { get; private set; } = DateTime.Now;
    public int RetryCount { get; private set; }
    public MessageStatus MessageStatus { get; private set; } = MessageStatus.Stored;
    public MessageDeliveryType DeliveryType { get; private set; } = deliveryType;

    /// <summary>
    /// Changes the status of the message.
    /// </summary>
    /// <param name="messageStatus">The new status of the message.</param>
    public void ChangeState(MessageStatus messageStatus)
    {
        MessageStatus = messageStatus;
    }

    /// <summary>
    /// Increases the retry count of the message.
    /// </summary>
    public void IncreaseRetry()
    {
        RetryCount++;
    }
}