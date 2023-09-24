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

using System;
using System.Collections.Generic;
using System.Text;

namespace RuneEncoding;

public abstract partial class RuneDecoder : Decoder
{
#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Measures the number of characters that would be needed to decode any pending bytes
    ///     in the decoder state followed by a span of bytes. The decoder state <strong>is
    ///     not</strong> modified. Trailing bytes can be counted as a decoding error.
    /// </summary>
    /// <param name="bytes">
    ///     The span of bytes to measure.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing bytes will be counted as a
    ///                 decoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing bytes will be considered the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters that would be needed to decode any pending bytes in the
    ///     decoder state followed by the span of bytes.
    /// </returns>
    public sealed override unsafe int GetCharCount(ReadOnlySpan<byte> bytes, bool flush)
    {
        fixed (byte* pBytes = bytes)
        {
            return GetCharCount(pBytes, bytes.Length, flush);
        }
    }
#endif

    /// <summary>
    ///     Measures the number of characters that would be needed to decode any pending bytes
    ///     in the decoder state followed by an array of bytes. The decoder state <strong>is
    ///     not</strong> modified. Trailing bytes will not be counted as a decoding error.
    /// </summary>
    /// <param name="bytes">
    ///     The array of bytes to measure.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte to measure.
    /// </param>
    /// <param name="count">
    ///     The number of bytes to measure.
    /// </param>
    /// <returns>
    ///     The number of characters that would be needed to decode any pending bytes in the
    ///     decoder state followed by the array of bytes.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="bytes" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="index" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="count" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="index" /> and <paramref name="count" />
    ///                 taken together lie outside of the <paramref name="bytes" /> array.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    public sealed override int GetCharCount(byte[] bytes, int index, int count) =>
        GetCharCount(bytes, index, count, false);

    /// <summary>
    ///     Measures the number of characters that would be needed to decode any pending bytes
    ///     in the decoder state followed by an array of bytes. The decoder state <strong>is
    ///     not</strong> modified. Trailing bytes can be counted as a decoding error.
    /// </summary>
    /// <param name="bytes">
    ///     The array of bytes to measure.
    /// </param>
    /// <param name="index">
    ///     The index of the first byte to measure.
    /// </param>
    /// <param name="count">
    ///     The number of bytes to measure.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing bytes will be counted as a
    ///                 decoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing bytes will be considered the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters that would be needed to decode any pending bytes in the
    ///     decoder state followed by the array of bytes.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="bytes" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="index" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="count" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="index" /> and <paramref name="count" />
    ///                 taken together lie outside of the <paramref name="bytes" /> array.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    public sealed override unsafe int GetCharCount(byte[] bytes, int index, int count
        , bool flush)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException("The bytes parameter is null.");
        }
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException("The index parameter is less than zero.");
        }
        if (index + count > bytes.Length)
        {
            throw new ArgumentOutOfRangeException(
                "The index and count parameters taken together lie outside of the byte array.");
        }

        fixed (byte* pBytes = bytes)
        {
            return GetCharCount(pBytes + index, count, flush);
        }
    }

    /// <summary>
    ///     Measures the number of characters that would be needed to decode any pending bytes
    ///     in the decoder state followed by a buffer of bytes. The decoder state <strong>is
    ///     not</strong> modified. Trailing bytes can be counted as a decoding error.
    /// </summary>
    /// <param name="bytes">
    ///     A pointer to the first byte in the buffer of bytes to measure.
    /// </param>
    /// <param name="count">
    ///     The number of bytes to measure.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing bytes will be counted as a
    ///                 decoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing bytes will be considered the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters that would be needed to decode any pending bytes in the
    ///     decoder state followed by the buffer of bytes.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="count" /> is less than zero.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    public sealed override unsafe int GetCharCount(byte* bytes, int count, bool flush)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException("The count parameter is less than zero.");

        var returnValue = 0;
        var index = 0;
        var first = true;
        bool? isBmp;

        while (true)
        {
            var limit = count - index;
            var bytesRead = AssessScalarValue(bytes + index, limit, first, out isBmp);

            first = false;

            if (isBmp is bool isBmp_)
            {
                returnValue += isBmp_ ? 1 : 2;
            }
            else
            {
                if (flush && bytesRead > 0)
                    returnValue += GetFallbackChars(bytes + index, limit).Length;

                return returnValue;
            }

            index += bytesRead;
        }
    }

    /// <summary>
    ///     Attempts to determine if the encoded scalar value at the start of a byte buffer lies
    ///     within the Basic Multilingual Plane.
    /// </summary>
    /// <param name="bytes">
    ///     A pointer to the first byte of the encoded scalar value.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of bytes to read in order to assess the scalar value.
    /// </param>
    /// <param name="first">
    ///     Indicates if this is the first time this method is being invoked for a given
    ///     invocation of <see cref="o:System.Text.Decoder.GetCharCount" />. If
    ///     <see langword="true" />, the decoder state should be read and factored in as there
    ///     may be trailing bytes left over from the last invocation of
    ///     <see cref="o:System.Text.Decoder.GetChars" />.
    /// </param>
    /// <param name="isBmp">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — The assessed scalar value lies within the BMP
    ///                 and is therefore represented by a single <see cref="System.Char" />
    ///                 value.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — The assessed scalar value lies outside of the
    ///                 BMP and is therefore represented by two <see cref="System.Char" />
    ///                 values.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="null" /> — The provided byte buffer was exhausted
    ///                 (according to <paramref name="limit" />) before the scalar value could
    ///                 be assessed.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes read attempting to assess the scalar value.
    /// </returns>
    protected abstract unsafe int AssessScalarValue(byte* bytes, int limit, bool first
        , out bool? isBmp);
}
