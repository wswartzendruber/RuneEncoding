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
using System.Text;

namespace RuneEncoding;

public abstract partial class RuneEncoder : Encoder
{
#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Measures the number of characters that would be needed to encode any buffered high
    ///     surrogate in the encoder state followed by a span of characters. The encoder state
    ///     <strong>is not</strong> modified. A trailing high surrogate can be counted as an
    ///     encoding error.
    /// </summary>
    /// <param name="chars">
    ///     The span of characters to measure.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing high surrogate will be counted as
    ///                 an encoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing high surrogate will be
    ///                 considered the beginning character of the next encoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes that would be needed to encode any buffered high surrogate in
    ///     the encoder state followed by the span of characters.
    /// </returns>
    /// <exception cref="EncoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Encoder.Fallback" /> is set to
    ///     <see cref="EncoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetByteCount(ReadOnlySpan<char> chars, bool flush)
    {
        fixed (char* pChars = chars)
        {
            var rpChars = (chars.Length > 0)
                ? pChars
                : (char*)IntPtr.Size;

            return GetByteCount(rpChars, chars.Length, flush);
        }
    }
#endif

    /// <summary>
    ///     Measures the number of characters that would be needed to encode any buffered high
    ///     surrogate in the encoder state followed by an array of characters. The encoder state
    ///     <strong>is not</strong> modified. A trailing high surrogate can be counted as an
    ///     encoding error.
    /// </summary>
    /// <param name="chars">
    ///     The array of characters to measure.
    /// </param>
    /// <param name="index">
    ///     The index of the first character to measure.
    /// </param>
    /// <param name="count">
    ///     The number of characters to measure.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing high surrogate will be counted as
    ///                 an encoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing high surrogate will be
    ///                 considered the beginning character of the next encoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes that would be needed to encode any buffered high surrogate in
    ///     the encoder state followed by the array of characters.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="chars" /> is <see langword="null" />.
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
    ///                 taken together lie outside of the <paramref name="chars" /> array.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="EncoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Encoder.Fallback" /> is set to
    ///     <see cref="EncoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetByteCount(char[] chars, int index, int count
        , bool flush)
    {
        if (chars is null)
        {
            throw new ArgumentNullException("chars");
        }
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException("The index parameter is less than zero.");
        }
        if (index + count > chars.Length)
        {
            throw new ArgumentOutOfRangeException("The index and count parameters taken "
                + "together lie outside of the character array.");
        }

        fixed (char* pChars = chars)
        {
            var rpChars = (chars.Length > 0)
                ? pChars
                : (char*)IntPtr.Size;

            return GetByteCount(rpChars + index, count, flush);
        }
    }

    /// <summary>
    ///     Measures the number of characters that would be needed to encode any buffered high
    ///     surrogate in the encoder state followed by a buffer of characters. The encoder state
    ///     <strong>is not</strong> modified. A trailing high surrogate can be counted as an
    ///     encoding error.
    /// </summary>
    /// <param name="chars">
    ///     A pointer to the first character in the buffer of characters to measure.
    /// </param>
    /// <param name="count">
    ///     The number of characters to measure.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing high surrogate will be counted as
    ///                 an encoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing high surrogate will be
    ///                 considered the beginning character of the next encoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes that would be needed to encode any buffered high surrogate in
    ///     the encoder state followed by the buffer of characters.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="chars" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="count" /> is less than zero.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="EncoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Encoder.Fallback" /> is set to
    ///     <see cref="EncoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetByteCount(char* chars, int count, bool flush)
    {
        if (chars is null)
            throw new ArgumentNullException("chars");
        if (count < 0)
            throw new ArgumentOutOfRangeException("The count parameter is less than zero.");

        return 0;
    }
}
