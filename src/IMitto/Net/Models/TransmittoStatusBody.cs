namespace IMitto.Net.Models;

public class TransmittoStatusBody : TransmittoMessageBody
{
	public static readonly TransmittoStatusBody Completed = ForEvent();

	public TransmittoStatus Status { get; set; } = new TransmittoStatus();

	public static TransmittoStatusBody Error(string? message = null)
		=> WithStatus(code: (int)TransmittoEventType.Error, message: message);

	public static TransmittoStatusBody WithStatus(int? code = null, string? message = null) => new()
	{
		Status = new()
		{
			Code = code ??= (int)TransmittoEventType.Error,
			Message = message ?? (TransmittoStatus.IsSuccessfulCode(code.Value)
				? nameof(TransmittoEventType.Completed)
				: nameof(TransmittoEventType.Error))
		}
	};

	public static TransmittoStatusBody ForEvent(TransmittoEventType? code = TransmittoEventType.Completed, string? message = null) => new()
	{
		Status = new()
		{ 
			Code = (int)(code ?? TransmittoEventType.Completed),
			Message = message ?? nameof(TransmittoEventType.Completed)
		}
	};
}
