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

using System.Text;

namespace RuneEncoding.Tests;

public class PassthruTestDecoderFallbackBuffer : DecoderFallbackBuffer
{
    private readonly Queue<char> RemainingChars = new();

    public override int Remaining => RemainingChars.Count;

    public override bool Fallback(byte[] bytesUnknown, int index)
    {
        RemainingChars.Clear();

        foreach (byte byteUnknown in bytesUnknown)
            RemainingChars.Enqueue((char)byteUnknown);

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
