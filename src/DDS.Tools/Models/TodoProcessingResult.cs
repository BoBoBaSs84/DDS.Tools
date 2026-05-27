// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
namespace DDS.Tools.Models;

/// <summary>
/// Result data from processing todos.
/// </summary>
/// <param name="TodosDoneCount">Number of completed todos.</param>
/// <param name="DuplicatesCount">Number of detected duplicates.</param>
internal readonly record struct TodoProcessingResult(int TodosDoneCount, int DuplicatesCount);
