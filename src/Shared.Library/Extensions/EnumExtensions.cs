namespace Shared.Library.Extensions;

/// <summary>
/// The enum extensions class.
/// </summary>
internal static class EnumExtensions
{
	/// <summary>
	/// The <see cref="GetListFromEnum{T}(T)"/> method should return a list of all enumerators of the given type of enum.
	/// </summary>
	/// <typeparam name="TEnum">The enmuerator itself.</typeparam>
	/// <param name="enumValue">The value of the enumerator.</param>
	/// <returns>A list of all enums of the provided type.</returns>
	public static List<TEnum> GetListFromEnum<TEnum>(this TEnum enumValue) where TEnum : Enum =>
		Enum.GetValues(enumValue.GetType()).Cast<TEnum>().ToList();
}
