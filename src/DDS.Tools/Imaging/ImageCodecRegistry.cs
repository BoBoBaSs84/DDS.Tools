// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Imaging;

namespace DDS.Tools.Imaging;

/// <summary>
/// The image codec registry class.
/// </summary>
/// <param name="decoders">The registered decoders.</param>
/// <param name="encoders">The registered encoders.</param>
internal sealed class ImageCodecRegistry(IEnumerable<IImageDecoder> decoders, IEnumerable<IImageEncoder> encoders) : IImageCodecRegistry
{
	private readonly Dictionary<ImageType, IImageDecoder> _decoders = decoders.ToDictionary(decoder => decoder.Format);
	private readonly Dictionary<ImageType, IImageEncoder> _encoders = encoders.ToDictionary(encoder => encoder.Format);

	/// <inheritdoc/>
	public IImageDecoder GetDecoder(ImageType format)
		=> _decoders.TryGetValue(format, out IImageDecoder? decoder)
			? decoder
			: throw new CommandException($"Reading '{format}' files is not supported.");

	/// <inheritdoc/>
	public IImageEncoder GetEncoder(ImageType format)
		=> _encoders.TryGetValue(format, out IImageEncoder? encoder)
			? encoder
			: throw new CommandException($"Writing '{format}' files is not supported.");
}
