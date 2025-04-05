

using System.Net;

namespace IMitto.Net.Models;

public class MittoStatus
{
	public int Code { get; set; }

	public string? Message { get; set; }

	public object? Details { get; set; }

	public bool Success => IsSuccessfulCode(Code);

	public static bool IsSuccessfulCode(int code)
	{
		return code != 0 && code != (int)HttpStatusCode.Unauthorized
			&& code != (int)HttpStatusCode.Forbidden
			&& code != (int)HttpStatusCode.BadRequest
			&& code != (int)HttpStatusCode.InternalServerError;
	}
}