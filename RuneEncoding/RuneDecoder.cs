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

/// <summmary>
///     Automatically handles the common aspects of character decoding, such as surrogate
///     decomposition, thereby allowing implementers to worry about specific concerns only.
/// </summary>
public abstract class RuneDecoder : Decoder
{
    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        return 0;
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars
        , int charIndex)
    {
        int byteEnd = byteIndex + byteCount;
        int lastByteIndex = byteIndex;
        int currentByteIndex = byteIndex;
        int currentCharIndex = charIndex;

        while (currentByteIndex < byteEnd)
        {
            int scalarValue;

            currentByteIndex += ReadScalarValue(bytes, currentByteIndex, out scalarValue);

            if ((0 <= scalarValue && scalarValue <= 0xD7FF)
                || (0xE000 <= scalarValue && scalarValue <= 0xFFFF))
            {
                chars[currentCharIndex++] = (char)scalarValue;
            }
            else if (0x010000 <= scalarValue && scalarValue <= 0x10FFFF)
            {
                chars[currentCharIndex++] = HighSurrogate(scalarValue);
                chars[currentCharIndex++] = LowSurrogate(scalarValue);
            }
            else
            {
                throw new NotImplementedException("DecoderFallback not handled yet.");
            }

            lastByteIndex = currentByteIndex;
        }

        return currentCharIndex - charIndex;
    }

    protected abstract int ReadScalarValue(byte[] bytes, int byteIndex, out int scalarValue);

    private static char HighSurrogate(int scalarValue) =>
        (char)((((uint)scalarValue - 0x10000U) >> 10) + 0xD800);

    private static char LowSurrogate(int scalarValue) =>
        (char)(((scalarValue - 0x10000) & 0x3FF) + 0xDC00);
}
