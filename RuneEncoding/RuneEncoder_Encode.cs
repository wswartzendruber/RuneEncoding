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
    ///     Encodes any buffered high surrogate in the encoder state followed by a span of
    ///     characters. The encoder state <strong>is</strong> modified. A trailing high
    ///     surrogate can be flushed as an encoding error.
    /// </summary>
    /// <param name="chars">
    ///     The span of characters to encode.
    /// </param>
    /// <param name="bytes">
    ///     The byte span to contain the encoded characters.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing high surrogate will be flushed as
    ///                 an encoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing high surrogate will be buffered
    ///                 as the beginning character of the next encoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes written.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the byte span is too small to hold the encoded characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="EncoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Encoder.Fallback" /> is set to
    ///     <see cref="EncoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes
        , bool flush)
    {
        fixed (char* pChars = chars)
        fixed (byte* pBytes = bytes)
        {
            var rpChars = (chars.Length > 0)
                ? pChars
                : (char*)IntPtr.Size;
            var rpBytes = (bytes.Length > 0)
                ? pBytes
                : (byte*)IntPtr.Size;

            return GetBytes(rpChars, chars.Length, rpBytes, bytes.Length, flush);
        }
    }
#endif

    /// <summary>
    ///     Encodes any buffered high surrogate in the encoder state followed by an array of
    ///     characters. The encoder state <strong>is</strong> modified. A trailing high
    ///     surrogate can be flushed as an encoding error.
    /// </summary>
    /// <param name="chars">
    ///     The array of characters to encode.
    /// </param>
    /// <param name="charIndex">
    ///     The index of the first character to encode.
    /// </param>
    /// <param name="charCount">
    ///     The number of characters to encode.
    /// </param>
    /// <param name="bytes">
    ///     The byte array to contain the encoded characters.
    /// </param>
    /// <param name="byteIndex">
    ///     The index at which to start writing the encoded characters.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing high surrogate will be flushed as
    ///                 an encoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing high surrogate will be buffered
    ///                 as the beginning character of the next encoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes written.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the byte array is too small to hold the encoded characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     <list>
    ///         <item>W
    ///             <description>
    ///                 Thrown when <paramref name="chars" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="bytes" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charIndex" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteIndex" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charIndex" /> and
    ///                 <paramref name="charCount" /> taken together lie outside of
    ///                 <paramref name="chars" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteIndex" /> lies ouside of
    ///                 <paramref name="bytes" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="EncoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Encoder.Fallback" /> is set to
    ///     <see cref="EncoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetBytes(char[] chars, int charIndex, int charCount
        , byte[] bytes, int byteIndex, bool flush)
    {
        if (chars is null)
        {
            throw new ArgumentNullException("chars");
        }
        if (bytes is null)
        {
            throw new ArgumentNullException("bytes");
        }
        if (charIndex < 0)
        {
            throw new ArgumentOutOfRangeException("The charIndex parameter is less than zero.");
        }
        if (byteIndex < 0)
        {
            throw new ArgumentOutOfRangeException("The byteIndex parameter is less than zero.");
        }
        if (charIndex + charCount > chars.Length)
        {
            throw new ArgumentOutOfRangeException("The charIndex and charCount parameters "
                + "taken together lie outside of the character array.");
        }
        if (byteIndex > 0 && byteIndex >= bytes.Length)
        {
            throw new ArgumentOutOfRangeException(
                "The byteIndex parameter lies outside of the byte array.");
        }

        fixed (char* pChars = chars)
        fixed (byte* pBytes = bytes)
        {
            var rpChars = (chars.Length > 0)
                ? pChars
                : (char*)IntPtr.Size;
            var rpBytes = (bytes.Length > 0)
                ? pBytes
                : (byte*)IntPtr.Size;

            return GetBytes(rpChars + charIndex, charCount, rpBytes + byteIndex
                , bytes.Length - byteIndex, flush);
        }
    }

    /// <summary>
    ///     Encodes any buffered high surrogate in the encoder state followed by a buffer of
    ///     characters. The encoder state <strong>is</strong> modified. A trailing high
    ///     surrogate can be flushed as an encoding error.
    /// </summary>
    /// <param name="chars">
    ///     A pointer to the first character in the buffer of characters to encode.
    /// </param>
    /// <param name="charCount">
    ///     The number of characters to encode.
    /// </param>
    /// <param name="bytes">
    ///     A pointer to the location at which to start writing the encoded characters.
    /// </param>
    /// <param name="byteCount">
    ///     The maximum number of bytes to write.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing high surrogate will be flushed as
    ///                 an encoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing high surrogate will be buffered
    ///                 as the beginning character of the next encoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of bytes written.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the byte buffer is too small to hold the encoded characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="chars" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="bytes" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="EncoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Encoder.Fallback" /> is set to
    ///     <see cref="EncoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetBytes(char* chars, int charCount, byte* bytes
        , int byteCount, bool flush)
    {
        if (chars is null)
            throw new ArgumentNullException("chars");
        if (bytes is null)
            throw new ArgumentNullException("bytes");
        if (charCount < 0)
            throw new ArgumentOutOfRangeException("The charCount parameter is less than zero.");
        if (byteCount < 0)
            throw new ArgumentOutOfRangeException("The byteCount parameter is less than zero.");

        return 0;
    }

    protected abstract unsafe int EncodeScalarValue(int scalarValue, byte* bytes, int limit);
}
