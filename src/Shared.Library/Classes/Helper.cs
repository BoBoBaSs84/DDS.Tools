using System.Security.Cryptography;
using System.Text.Json;

namespace Shared.Library.Classes;

/// <summary>
/// The helper class.
/// </summary>
public static class Helper
{
	/// <summary>
	/// Should return an md5 byte array.
	/// </summary>
	/// <param name="input">The byte array input.</param>
	public static byte[] GetMD5(byte[] input)
			=> MD5.HashData(input);

	/// <summary>
	/// Should return an md5 byte array as string.
	/// </summary>
	/// <param name="input">The byte array input.</param>	
	public static string GetMD5String(byte[] input)
			=> Convert.ToHexString(GetMD5(input));

	/// <summary>
	/// Should return a json result.
	/// </summary>
	/// <param name="todos"></param>
	public static string JsonResult(IList<Todo> todos)
			=> JsonSerializer.Serialize(todos);
}
