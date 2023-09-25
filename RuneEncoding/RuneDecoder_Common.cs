/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RuneEncoding;

/// <summary>
///     Automatically handles the common aspects of character decoding, such as surrogate
///     decomposition, thereby allowing implementers to worry about specific concerns only.
/// </summary>
public abstract partial class RuneDecoder : Decoder
{
    protected unsafe char[] GetFallbackChars(byte* buffer, int count)
    {
        var fallbackChars = new List<char>();
        var errorBuffer = new byte[count];
        var fallbackBuffer = FallbackBuffer;

        Marshal.Copy((nint)buffer, errorBuffer, 0, count);

        if (fallbackBuffer.Fallback(errorBuffer, 0))
        {
            while (fallbackBuffer.Remaining > 0)
                fallbackChars.Add(fallbackBuffer.GetNextChar());
        }
        else
        {
            fallbackChars.Add(Constants.ReplacementChar);
        }

        return fallbackChars.ToArray();
    }
}
