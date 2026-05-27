// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

using BB84.SourceGenerators.Attributes;

using DDS.Tools.Interfaces.Providers;

namespace DDS.Tools.Providers;

/// <summary>
/// The path provider class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Wrapper class.")]
[GenerateAbstraction(typeof(Path), typeof(IPathProvider), typeof(PathProvider))]
internal sealed partial class PathProvider : IPathProvider
{ }
