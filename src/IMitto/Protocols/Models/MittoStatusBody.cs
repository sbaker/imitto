﻿namespace IMitto.Protocols.Models;

public class MittoStatusBody : MittoMessageBody
{
	public static readonly MittoStatusBody Completed = ForEvent();

	public MittoStatus Status { get; set; } = new MittoStatus();

	public Dictionary<string, object>? Details { get; set; }

	public static MittoStatusBody Error(string? message = null)
		=> WithStatus(code: (int)MittoEventType.Error, message: message);

	public static MittoStatusBody WithStatus(MittoEventType? code = null, string? message = null, Dictionary<string, object>? details = null) => new()
	{
		Status = new()
		{
			Code = (int)(code ??= MittoEventType.Error),
			Message = message ?? (MittoStatus.IsSuccessfulCode((int)code.Value)
				? nameof(MittoEventType.Completed)
				: nameof(MittoEventType.Error)),
			Details = details
		}
	};

	public static MittoStatusBody WithStatus(int? code = null, string? message = null) => new()
	{
		Status = new()
		{
			Code = code ??= (int)MittoEventType.Error,
			Message = message ?? (MittoStatus.IsSuccessfulCode(code.Value)
				? nameof(MittoEventType.Completed)
				: nameof(MittoEventType.Error))
		}
	};

	public static MittoStatusBody ForEvent(MittoEventType? code = MittoEventType.Completed, string? message = null) => new()
	{
		Status = new()
		{ 
			Code = (int)(code ?? MittoEventType.Completed),
			Message = message ?? nameof(MittoEventType.Completed)
		}
	};
}
