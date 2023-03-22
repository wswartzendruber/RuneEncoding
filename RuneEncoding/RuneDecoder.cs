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
        return  0;
    }
}
