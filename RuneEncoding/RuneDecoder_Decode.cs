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

public abstract partial class RuneDecoder : Decoder
{
    private static readonly ArgumentException CharCountExceededException
        = new("Character store is too small to hold the decoded characters.");

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by a span of bytes. The
    ///     decoder state <strong>is</strong> modified. Trailing bytes can be buffered for the
    ///     next decoding operation.
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
    ///                 <see langword="false" /> — Any trailing bytes will be buffered as the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters decoded.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the character span is too small to hold the decoded
    ///                 characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
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
            var rpBytes = (bytes.Length > 0)
                ? pBytes
                : (byte*)IntPtr.Size;
            var rpChars = (chars.Length > 0)
                ? pChars
                : (char*)IntPtr.Size;

            return GetChars(rpBytes, bytes.Length, rpChars, chars.Length, flush);
        }
    }
#endif

    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by an array of bytes. The
    ///     decoder state <strong>is</strong> modified. Trailing bytes will be buffered for the
    ///     next decoding operation.
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
    ///     The number of characters decoded.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the character array is too small to hold the decoded
    ///                 characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
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
    ///                 <paramref name="byteCount" /> taken together lie outside of
    ///                 <paramref name="bytes" />.
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
    ///     decoder state <strong>is</strong> modified. Trailing bytes can be buffered for the
    ///     next decoding operation.
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
    ///                 <see langword="false" /> — Any trailing bytes will be buffered as the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters decoded.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the character array is too small to hold the decoded
    ///                 characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
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
    ///                 <paramref name="byteCount" /> taken together lie outside of
    ///                 <paramref name="bytes" />.
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
            throw new ArgumentNullException("bytes");
        }
        if (chars is null)
        {
            throw new ArgumentNullException("chars");
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
            var rpBytes = (bytes.Length > 0)
                ? pBytes
                : (byte*)IntPtr.Size;
            var rpChars = (chars.Length > 0)
                ? pChars
                : (char*)IntPtr.Size;

            return GetChars(rpBytes + byteIndex, byteCount, rpChars + charIndex
                , chars.Length - charIndex, flush);
        }
    }

    /// <summary>
    ///     Decodes any pending bytes in the decoder state followed by a buffer of bytes. The
    ///     decoder state <strong>is</strong> modified. Trailing bytes can be buffered for the
    ///     next decoding operation.
    /// </summary>
    /// <param name="bytes">
    ///     A pointer to the first byte in the buffer of bytes to decode.
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
    ///                 <see langword="false" /> — Any trailing bytes will be buffered as the
    ///                 beginning bytes of the next decoding operation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     The number of characters decoded.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <list>
    ///         <item>
    ///             <description>
    ///                 Thrown when the character buffer is too small to hold the decoded
    ///                 characters.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
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
    ///                 Thrown when <paramref name="byteCount" /> is less than zero.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Thrown when <paramref name="charCount" /> is less than zero.
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
        if (bytes is null)
            throw new ArgumentNullException("bytes");
        if (chars is null)
            throw new ArgumentNullException("chars");
        if (byteCount < 0)
            throw new ArgumentOutOfRangeException("The byteCount parameter is less than zero.");
        if (charCount < 0)
            throw new ArgumentOutOfRangeException("The charCount parameter is less than zero.");

        int byteIndex = 0;
        int charIndex = 0;

        while (true)
        {
            var byteLimit = byteCount - byteIndex;
            var bytesRead = DecodeScalarValue(bytes + byteIndex, byteLimit
                , out int? scalarValue);

            if (scalarValue is int scalarValue_)
            {
                if ((0 <= scalarValue_ && scalarValue_ <= 0xD7FF)
                    || (0xE000 <= scalarValue_ && scalarValue_ <= 0xFFFF))
                {
                    if (charIndex < charCount)
                        chars[charIndex++] = (char)scalarValue_;
                    else
                        throw CharCountExceededException;
                }
                else if (0x010000 <= scalarValue_ && scalarValue_ <= 0x10FFFF)
                {
                    if (charIndex < charCount)
                        chars[charIndex++] = HighSurrogate(scalarValue_);
                    else
                        throw CharCountExceededException;

                    if (charIndex < charCount)
                        chars[charIndex++] = LowSurrogate(scalarValue_);
                    else
                        throw CharCountExceededException;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Implementation returned an invalid scalar value ({scalarValue_}).");
                }
            }
            else
            {
                if (flush && bytesRead > 0)
                {
                    var fallbackChars = GetFallbackChars(bytes + byteIndex, byteLimit);

                    foreach (var fallbackChar in fallbackChars)
                    {
                        if (charIndex < charCount)
                            chars[charIndex++] = fallbackChar;
                        else
                            throw CharCountExceededException;
                    }
                }

                return charIndex;
            }

            byteIndex += bytesRead;
        }
    }

    /// <summary>
    ///     Attempts to decode the scalar value at the start of a byte buffer.
    /// </summary>
    /// <param name="bytes">
    ///     A pointer to the first byte of the encoded scalar value.
    /// </param>
    /// <param name="limit">
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
    ///     buffer is exhausted (according to <paramref name="limit" />) before the scalar value
    ///     could be can be decoded.
    /// </param>
    /// <returns>
    ///     The number of bytes read attempting to decode the scalar value.
    /// </returns>
    protected abstract unsafe int DecodeScalarValue(byte* bytes, int limit
        , out int? scalarValue);

    private static char HighSurrogate(int scalarValue) =>
        (char)((((uint)scalarValue - 0x10000U) >> 10) + 0xD800);

    private static char LowSurrogate(int scalarValue) =>
        (char)(((scalarValue - 0x10000) & 0x3FF) + 0xDC00);
}
