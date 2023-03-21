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

using System.Collections.Generic;
using System.Text;

namespace RuneEncoding.Tests;

public class TransformTestEncoderFallbackBuffer : EncoderFallbackBuffer
{
    private readonly Queue<char> RemainingChars = new();

    private char CharUnknown = ' ';
    private int Index = 0;

    public override int Remaining => RemainingChars.Count;

    public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
    {
        throw new NotImplementedException();
    }

    public override bool Fallback(char charUnknown, int index)
    {
        CharUnknown = charUnknown;
        Index = index;

        RemainingChars.Clear();

        if ((charUnknown & 0xF800) == 0xD800)
            RemainingChars.Enqueue((char)(charUnknown >> 8));
        else
            throw new InvalidOperationException("Encountered non-surrogate as input.");

        return true;
    }

    public override char GetNextChar()
    {
        if (RemainingChars.Count > 0)
            return RemainingChars.Dequeue();
        else
            return '\u0000';
    }

    public override bool MovePrevious()
    {
        throw new NotImplementedException();
    }
}
