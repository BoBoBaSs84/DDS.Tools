using System.Security.Cryptography;
using System.Text.Json;

namespace Shared.Library.Classes;

/// <summary>
/// The helper class.
/// </summary>
[SuppressMessage("Security", "CA5351", Justification = "Not used for encryption.")]
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
	public static string GetJsonResultFromList(IList<Todo> todos)
			=> JsonSerializer.Serialize(todos);

	public static IList<Todo>? GetListFromJsonResult(string jsonResult)
		=> JsonSerializer.Deserialize<List<Todo>>(jsonResult);

	/// <summary>
	/// Should return a byte array.
	/// </summary>
	/// <param name="stream"></param>
	public static byte[] StreamToByteArray(Stream stream)
		=> stream is MemoryStream memoryStream ? memoryStream.ToArray() : ReadFully(stream);

	private static byte[] ReadFully(Stream input)
	{
		using MemoryStream ms = new();
		input.CopyTo(ms);
		return ms.ToArray();
	}
}
