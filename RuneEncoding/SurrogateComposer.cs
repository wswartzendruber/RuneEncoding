/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived
 * all copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using System;
using System.Collections.Generic;

namespace RuneEncoding;

internal class SurrogateComposer
{
    public char? HighSurrogate = null;

    public void Input(char value, Action<int> callback)
    {
        if (HighSurrogate is char highSurrogate)
        {
            if (char.IsLowSurrogate(value))
            {
                callback(char.ConvertToUtf32(highSurrogate, value));
                HighSurrogate = null;
            }
            else if (!char.IsSurrogate(value))
            {
                callback(Constants.ReplacementCode);
                HighSurrogate = null;
                callback(value);
            }
            else if (char.IsHighSurrogate(value))
            {
                callback(Constants.ReplacementCode);
                HighSurrogate = value;
            }
            else
            {
                throw new InvalidOperationException("Internal state is irrational.");
            }
        }
        else
        {
            if (!char.IsSurrogate(value))
                callback(value);
            else if (char.IsHighSurrogate(value))
                HighSurrogate = value;
            else if (char.IsLowSurrogate(value))
                callback(Constants.ReplacementCode);
            else
                throw new InvalidOperationException("Internal state is irrational.");
        }
    }

    public void Flush(Action<int> callback)
    {
        if (HighSurrogate is not null)
        {
            callback(Constants.ReplacementCode);
            HighSurrogate = null;
        }
    }

    public void Reset()
    {
        HighSurrogate = null;
    }
}
