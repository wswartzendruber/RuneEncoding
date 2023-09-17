﻿/*
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
    private static readonly IndexOutOfRangeException CharCountExceededException
        = new("Insufficient space for decoded characters.");

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by a span of bytes. The
    ///     decoder state <strong>is</strong> modified.
    /// </summary>
    /// <param name="bytes">
    ///     The span of bytes to decode.
    /// </param>
    /// <param name="chars">
    ///     The character span to contain the decoded characters.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing bytes will be flushed as a
    ///                 decoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing bytes will be seen as the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters written.
    /// </returns>
    /// <exception cref="DecoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Decoder.Fallback" /> is set to
    ///     <see cref="DecoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars
        , bool flush)
    {
        fixed (byte* pBytes = bytes)
        fixed (char* pChars = chars)
        {
            return GetChars(pBytes, bytes.Length, pChars, chars.Length, flush);
        }
    }
#endif

    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by an array of bytes. The
    ///     decoder state <strong>is</strong> modified.
    /// </summary>
    /// <param name="bytes">
    ///     The array of bytes to decode.
    /// </param>
    /// <param name="byteIndex">
    ///     The index of the first byte to decode.
    /// </param>
    /// <param name="byteCount">
    ///     The number of bytes to decode.
    /// </param>
    /// <param name="chars">
    ///     The character array to contain the decoded characters.
    /// </param>
    /// <param name="charIndex">
    ///     The index at which to start writing the decoded characters.
    /// </param>
    /// <returns>
    ///     The number of characters written.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="bytes" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="chars" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteIndex" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charIndex" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteIndex" /> and
    ///                 <paramref name="byteCount" /> taken together lie outside of the
    ///                 <paramref name="bytes" /> array.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charIndex" /> lies ouside of
    ///                 <paramref name="chars" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="DecoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Decoder.Fallback" /> is set to
    ///     <see cref="DecoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount
        , char[] chars, int charIndex) =>
        GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);

    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by an array of bytes. The
    ///     decoder state <strong>is</strong> modified.
    /// </summary>
    /// <param name="bytes">
    ///     The array of bytes to decode.
    /// </param>
    /// <param name="byteIndex">
    ///     The index of the first byte to decode.
    /// </param>
    /// <param name="byteCount">
    ///     The number of bytes to decode.
    /// </param>
    /// <param name="chars">
    ///     The character array to contain the decoded characters.
    /// </param>
    /// <param name="charIndex">
    ///     The index at which to start writing the decoded characters.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing bytes will be flushed as a
    ///                 decoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing bytes will be seen as the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters written.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="bytes" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="chars" /> is <see langword="null" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteIndex" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charIndex" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteIndex" /> and
    ///                 <paramref name="byteCount" /> taken together lie outside of the
    ///                 <paramref name="bytes" /> array.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charIndex" /> lies ouside of
    ///                 <paramref name="chars" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="DecoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Decoder.Fallback" /> is set to
    ///     <see cref="DecoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount
        , char[] chars, int charIndex, bool flush)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException("The bytes parameter is null.");
        }
        if (chars is null)
        {
            throw new ArgumentNullException("The chars parameter is null.");
        }
        if (byteIndex < 0)
        {
            throw new ArgumentOutOfRangeException("The byteIndex parameter is less than zero.");
        }
        if (charIndex < 0)
        {
            throw new ArgumentOutOfRangeException("The charIndex parameter is less than zero.");
        }
        if (byteIndex + byteCount > bytes.Length)
        {
            throw new ArgumentOutOfRangeException("The byteIndex and byteCount parameters "
                + "taken together lie outside of the byte array.");
        }
        if (charIndex > 0 && charIndex >= chars.Length)
        {
            throw new ArgumentOutOfRangeException(
                "The charIndex parameter lies outside of the char array.");
        }

        fixed (byte* pBytes = bytes)
        fixed (char* pChars = chars)
        {
            return GetChars(pBytes + byteIndex, byteCount, pChars + charIndex
                , chars.Length - charIndex, flush);
        }
    }

    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by a sequence of bytes. The
    ///     decoder state <strong>is</strong> modified.
    /// </summary>
    /// <param name="bytes">
    ///     A pointer to the first byte in a sequence of bytes to decode.
    /// </param>
    /// <param name="byteCount">
    ///     The number of bytes to decode.
    /// </param>
    /// <param name="chars">
    ///     A pointer to the location at which to start writing the decoded characters.
    /// </param>
    /// <param name="charCount">
    ///     The maximum number of characters to write.
    /// </param>
    /// <param name="flush">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see langword="true" /> — Any trailing bytes will be flushed as a
    ///                 decoding error.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see langword="false" /> — Any trailing bytes will be seen as the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters written.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="byteCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    /// <exception cref="DecoderFallbackException">
    ///     Thrown when a fallback occurs while <see cref="Decoder.Fallback" /> is set to
    ///     <see cref="DecoderFallbackException" />.
    /// </exception>
    public sealed override unsafe int GetChars(byte* bytes, int byteCount, char* chars
        , int charCount, bool flush)
    {
        if (byteCount < 0)
            throw new ArgumentOutOfRangeException("The byteCount parameter is less than zero.");
        if (charCount < 0)
            throw new ArgumentOutOfRangeException("The charCount parameter is less than zero.");

        int currentByteIndex = 0;
        int currentCharIndex = 0;

        while (currentByteIndex < byteCount)
        {
            var byteLimit = byteCount - currentByteIndex;

            currentByteIndex += DecodeScalarValue(bytes + currentByteIndex, byteLimit
                , out int? scalarValue);

            if (scalarValue is int scalarValue_)
            {
                if ((0 <= scalarValue_ && scalarValue_ <= 0xD7FF)
                    || (0xE000 <= scalarValue_ && scalarValue_ <= 0xFFFF))
                {
                    if (currentCharIndex < charCount)
                        chars[currentCharIndex++] = (char)scalarValue_;
                    else
                        throw CharCountExceededException;
                }
                else if (0x010000 <= scalarValue && scalarValue <= 0x10FFFF)
                {
                    if (currentCharIndex < charCount)
                        chars[currentCharIndex++] = HighSurrogate(scalarValue_);
                    else
                        throw CharCountExceededException;

                    if (currentCharIndex < charCount)
                        chars[currentCharIndex++] = LowSurrogate(scalarValue_);
                    else
                        throw CharCountExceededException;
                }
                else
                {
                    if (currentCharIndex < charCount)
                        chars[currentCharIndex++] = Constants.ReplacementChar;
                    else
                        throw CharCountExceededException;
                }
            }
        }

        // TODO: Handle error flushing.

        return currentCharIndex;
    }

    /// <summary>
    ///     Resets the internal state of the decoder.
    /// </summary>
    public sealed override void Reset() =>
        ResetState();

    /// <summary>
    ///     Attempts to decode the scalar value at the start of a byte sequence.
    /// </summary>
    /// <param name="bytes">
    ///     A pointer to the first byte of the encoded scalar value.
    /// </param>
    /// <param name="count">
    ///     The maximum number of bytes to read in order to decode the scalar value.
    /// </param>
    /// <param name="scalarValue">
    ///     The decoded scalar value, which should lie within either of the following inclusive
    ///     ranges:
    ///     <list type="bullet">
    ///         <item><description><pre>U+0000</pre> - <pre>U+D7FF</pre></description></item>
    ///         <item><description><pre>U+E000</pre> - <pre>U+10FFFF</pre></description></item>
    ///     </list>
    ///     This parameter should be set to <see langword="null" /> if the provided byte
    ///     sequence is exhausted (according to <paramref name="count" />) before the scalar
    ///     value could be can be decoded.
    /// </param>
    /// <returns>
    ///     The number of bytes read attempting to decode the scalar value.
    /// </returns>
    protected abstract unsafe int DecodeScalarValue(byte* bytes, int count
        , out int? scalarValue);

    /// <summary>
    ///     Resets the implementation-specific state of the decoder.
    /// </summary>
    protected virtual void ResetState() { }

    private static char HighSurrogate(int scalarValue) =>
        (char)((((uint)scalarValue - 0x10000U) >> 10) + 0xD800);

    private static char LowSurrogate(int scalarValue) =>
        (char)(((scalarValue - 0x10000) & 0x3FF) + 0xDC00);
}