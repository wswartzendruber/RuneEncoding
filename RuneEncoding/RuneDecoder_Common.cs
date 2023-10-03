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
    /// <summary>
    ///     Resets the internal state of the decoder.
    /// </summary>
    public sealed override void Reset() => ResetState();

    /// <summary>
    ///     Resets the implementation-specific state of the decoder.
    /// </summary>
    protected virtual void ResetState() { }

    /// <summary>
    ///     Gets an array of fallback characters according to a byte buffer.
    /// </summary>
    /// <param name="buffer">
    ///     A pointer to the first byte in the buffer of bytes to consider.
    /// </param>
    /// <param name="count">
    ///     The number of bytes to consider.
    /// </param>
    /// <returns>
    ///     The array of fallback characters according to a byte buffer.
    /// </returns>
    /// <exception cref="DecoderFallbackException">
    ///     Thrown when <see cref="Decoder.Fallback" /> is set to
    ///     <see cref="DecoderFallbackException" />.
    /// </exception>
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
