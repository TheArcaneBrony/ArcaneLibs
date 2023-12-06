using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ArcaneLibs.Extensions.Streams;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class StreamExtensions {
    private const bool _debug = false;

    public static long Remaining(this Stream stream) =>
        //if (_debug) if (_debug) Console.WriteLine($"stream pos: {stream.Position}, stream len: {stream.Length}, stream rem: {stream.Length - stream.Position}");
        stream.Length - stream.Position;

    public static int Peek(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't peek a non-readable stream");
        if (!stream.CanSeek)
            throw new InvalidOperationException("Can't peek a non-seekable stream");

        var peek = stream.ReadByte();
        if (peek != -1)
            stream.Seek(-1, SeekOrigin.Current);

        return peek;
    }

    public static IEnumerable<byte> Peek(this Stream stream, long count) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't peek a non-readable stream");
        if (!stream.CanSeek)
            throw new InvalidOperationException("Can't peek a non-seekable stream");
        long i;
        for (i = 0; i < count; i++) {
            var peek = stream.ReadByte();
            if (peek == -1) {
                // if (_debug) Console.WriteLine($"Can't peek {count} bytes, only {i} bytes remaining");
                stream.Seek(-i, SeekOrigin.Current);
                yield break;
            }

            yield return (byte)peek;
        }

        stream.Seek(-i, SeekOrigin.Current);
    }

    public static IEnumerable<byte> ReadBytes(this Stream stream, long count) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        for (long i = 0; i < count; i++) {
            var read = stream.ReadByte();
            if (read == -1)
                yield break;
            yield return (byte)read;
        }
    }

    public static bool StartsWith(this Stream stream, IEnumerable<byte> sequence) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");
        if (!stream.CanSeek)
            throw new InvalidOperationException("Can't read a non-seekable stream");

        // if (_debug) {
        //     if (_debug) Console.WriteLine($"Expected: {sequence.AsHexString()} ({sequence.AsString()})");
        //     if (_debug)
        //         Console.WriteLine(
        //             $"Actual:   {stream.Peek(sequence.Count()).AsHexString()} ({stream.Peek(sequence.Count()).AsString()})");
        // }

        var readCount = 0;
        foreach (int b in sequence) {
            var read = stream.ReadByte();
            readCount++;
            if (read == -1) {
                stream.Seek(-readCount, SeekOrigin.Current);
                return false;
            }

            if (read != b) {
                // if (_debug) {
                //     if (_debug) Console.ForegroundColor = ConsoleColor.Red;
                //     if (_debug) Console.WriteLine("^^".PadLeft((readCount * 3) + 9));
                //     if (_debug) Console.ResetColor();
                // }

                stream.Seek(-readCount, SeekOrigin.Current);
                return false;
            }
        }

        stream.Seek(-readCount, SeekOrigin.Current);

        return true;
    }

    public static bool StartsWith(this Stream stream, string ascii_seq) => stream.StartsWith(ascii_seq.Select(x => (byte)x));

    public static Stream Skip(this Stream stream, long count = 1) {
        if (!stream.CanSeek)
            throw new InvalidOperationException("Can't skip a non-seekable stream");
        stream.Seek(count, SeekOrigin.Current);
        return stream;
    }

    public static IEnumerable<byte> ReadNullTerminatedField(this Stream stream, IEnumerable<byte>? binaryPrefix = null,
        string? asciiPrefix = null) => ReadTerminatedField(stream, 0x00, binaryPrefix,
        asciiPrefix);

    public static IEnumerable<byte> ReadSpaceTerminatedField(this Stream stream, IEnumerable<byte>? binaryPrefix = null,
        string? asciiPrefix = null) => ReadTerminatedField(stream, 0x20, binaryPrefix,
        asciiPrefix);

    public static IEnumerable<byte> ReadTerminatedField(this Stream stream, byte terminator,
        IEnumerable<byte>? binaryPrefix = null, string? asciiPrefix = null) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");
        if (binaryPrefix != null)
            if (!stream.StartsWith(binaryPrefix))
                throw new InvalidDataException(
                    $"Binary prefix {stream.Peek(binaryPrefix.Count()).AsHexString()} does not match expected value of {binaryPrefix.AsHexString()}!");
            else stream.Skip(binaryPrefix.Count());
        else if (asciiPrefix != null)
            if (!stream.StartsWith(asciiPrefix))
                throw new InvalidDataException(
                    $"Text prefix {stream.Peek(asciiPrefix.Length).AsHexString()} ({stream.Peek(asciiPrefix.Length).AsString()}) does not match expected value of {asciiPrefix.AsBytes().AsHexString()} ({asciiPrefix})!");
            else stream.Skip(asciiPrefix.Length);

        var read = 0;
        while (stream.Peek() != terminator) {
            // if (_debug)
            //     Console.WriteLine(
            //         $"ReadTerminatedField -- pos: {stream.Position}/+{stream.Remaining()}/{stream.Length} | next: {(char)stream.Peek()} | Length: {read}");
            if (stream.Peek() == -1) {
                // if (_debug) Console.WriteLine("Warning: Reached end of stream while reading null-terminated field");
                yield break;
            }

            read++;
            yield return (byte)stream.ReadByte();
        }

        if (stream.Peek() == terminator) stream.Skip();
    }

    public static IEnumerable<byte> ReadTerminatedFieldWithoutPeeking(this Stream stream, byte terminator,
        IEnumerable<byte>? binaryPrefix = null, string? asciiPrefix = null) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");
        if (binaryPrefix != null)
            if (!stream.StartsWith(binaryPrefix))
                throw new InvalidDataException(
                    $"Binary prefix {stream.Peek(binaryPrefix.Count()).AsHexString()} does not match expected value of {binaryPrefix.AsHexString()}!");
            else stream.Skip(binaryPrefix.Count());
        else if (asciiPrefix != null)
            if (!stream.StartsWith(asciiPrefix))
                throw new InvalidDataException(
                    $"Text prefix {stream.Peek(asciiPrefix.Length).AsHexString()} ({stream.Peek(asciiPrefix.Length).AsString()}) does not match expected value of {asciiPrefix.AsBytes().AsHexString()} ({asciiPrefix})!");
            else stream.Skip(asciiPrefix.Length);

        var read = 0;
        int b;
        while ((b = stream.ReadByte()) != terminator) {
            // if (_debug)
            //     Console.WriteLine(
            //         $"ReadTerminatedField -- pos: {stream.Position}/+{stream.Remaining()}/{stream.Length} | next: {(char)stream.Peek()} | Length: {read}");
            if (b == -1) {
                // if (_debug) Console.WriteLine("Warning: Reached end of stream while reading null-terminated field");
                yield break;
            }

            read++;
            yield return (byte)b;
        }

        // if (stream.ReadByte() == terminator) stream.Skip();
    }

    public static IEnumerable<byte> ReadToEnd(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var read = 0;
        while ((read = stream.ReadByte()) != -1)
            yield return (byte)read;
    }

#region Read basic datatypes

#region Int16

    public static short ReadInt16LE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(2).ToArray();

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadInt16LE: " + bytes.AsHexString() + " => " + BitConverter.ToInt16(bytes));
        return BitConverter.ToInt16(bytes);
    }

    public static short ReadInt16BE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(2).ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadInt16BE: " + bytes.AsHexString() + " => " + BitConverter.ToInt16(bytes));
        return BitConverter.ToInt16(bytes);
    }

#endregion

#region UInt16

    public static ushort ReadUInt16LE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(2).ToArray();

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadUInt16LE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt16(bytes));
        return BitConverter.ToUInt16(bytes);
    }

    public static ushort ReadUInt16BE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(2).ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadUInt16BE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt16(bytes));
        return BitConverter.ToUInt16(bytes);
    }

#endregion

#region Int32

    public static int ReadInt32LE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(4).ToArray();

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadInt32LE: " + bytes.AsHexString() + " => " + BitConverter.ToInt32(bytes));
        return BitConverter.ToInt32(bytes);
    }

    public static int ReadInt32BE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(4).ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadInt32BE: " + bytes.AsHexString() + " => " + BitConverter.ToInt32(bytes));
        return BitConverter.ToInt32(bytes);
    }

#endregion

#region UInt32

    public static uint ReadUInt32BE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(4).ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadUInt32BE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt32(bytes));
        return BitConverter.ToUInt32(bytes);
    }

    public static uint ReadUInt32LE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(4).ToArray();

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadUInt32LE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt32(bytes));
        return BitConverter.ToUInt32(bytes);
    }

#endregion

#region Int64

    public static long ReadInt64LE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(8).ToArray();

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadInt64LE: " + bytes.AsHexString() + " => " + BitConverter.ToInt64(bytes));
        return BitConverter.ToInt64(bytes);
    }

    public static long ReadInt64BE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(8).ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadInt64BE: " + bytes.AsHexString() + " => " + BitConverter.ToInt64(bytes));
        return BitConverter.ToInt64(bytes);
    }

#endregion

#region UInt64

    public static ulong ReadUInt64LE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(8).ToArray();

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadUInt64LE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt64(bytes));
        return BitConverter.ToUInt64(bytes);
    }

    public static ulong ReadUInt64BE(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(8).ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // if (_debug) Console.WriteLine("ReadUInt64BE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt64(bytes));
        return BitConverter.ToUInt64(bytes);
    }

#endregion

#region Variable Length Number

    /// <summary>
    ///     VLQ but little endian
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Int128 ReadLEB128(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        Int128 result = 0;
        var shift = 0;
        byte b;
        do {
            b = (byte)stream.ReadByte();
            result |= (b & 0b0111_1111) << shift;
            shift += 7;
        } while ((b & 0b1000_0000) != 0);

        return result;
    }

    /// <summary>
    ///     LEB128 but big endian
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Int128 ReadVLQ(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        Int128 result = 0;
        // var shift = 0;
        byte b;
        do {
            b = (byte)stream.ReadByte();
            result = (result << 7) | (b & 0b0111_1111);
        } while ((b & 0b1000_0000) != 0);

        return result;
    }

#endregion

#region String

    public static string ReadStringWithLength(this Stream stream, int length) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var bytes = stream.ReadBytes(length).ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

    public static string ReadStringWithVLQ(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var length = stream.ReadVLQ();
        if (length > int.MaxValue)
            throw new InvalidOperationException("String length is too long");

        var bytes = stream.ReadBytes((int)length).ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

    public static string ReadStringWithLEB128(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var length = stream.ReadLEB128();
        if (length > int.MaxValue)
            throw new InvalidOperationException("String length is too long");

        var bytes = stream.ReadBytes((int)length).ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

#endregion

#region Variable Length 7 bit number

    public static int ReadV7(this Stream stream) {
        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read a non-readable stream");

        var result = 0;
        var shift = 0;
        byte b;
        do {
            b = (byte)stream.ReadByte();
            result |= (b & 0b0111_1111) << shift;
            shift += 7;
        } while ((b & 0b1000_0000) != 0);

        return result;
    }

#endregion

#endregion

#region Write basic data types

#region Int16

    public static void WriteInt16BE(this Stream stream, short value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteInt16LE(this Stream stream, short value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region UInt16

    public static void WriteUInt16BE(this Stream stream, ushort value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteUInt16LE(this Stream stream, ushort value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region Int32

    public static void WriteInt32BE(this Stream stream, int value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteInt32LE(this Stream stream, int value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region UInt32

    public static void WriteUInt32BE(this Stream stream, uint value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteUInt32LE(this Stream stream, uint value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region Int64

    public static void WriteInt64BE(this Stream stream, long value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteInt64LE(this Stream stream, long value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region UInt64

    public static void WriteUInt64BE(this Stream stream, ulong value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteUInt64LE(this Stream stream, ulong value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region Single

    public static void WriteSingleBE(this Stream stream, float value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteSingleLE(this Stream stream, float value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region Double

    public static void WriteDoubleBE(this Stream stream, double value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

    public static void WriteDoubleLE(this Stream stream, double value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        stream.Write(bytes);
    }

#endregion

#region String

    public static void WriteString(this Stream stream, string value, Encoding encoding) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = encoding.GetBytes(value);
        stream.Write(bytes);
    }

    public static void WriteString(this Stream stream, string value) => WriteString(stream, value, Encoding.UTF8);

    public static void WriteStringWithLeb128Length(this Stream stream, string value, Encoding encoding) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var bytes = encoding.GetBytes(value);
        stream.WriteULeb128((ulong)bytes.Length);
        stream.Write(bytes);
    }

    public static void WriteStringWithLeb128Length(this Stream stream, string value) => WriteStringWithLeb128Length(stream, value, Encoding.UTF8);

#endregion

#region Variable Length Number

    public static void WriteULeb128(this Stream stream, ulong value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        do {
            var b = (byte)(value & 0x7f);
            value >>= 7;

            if (value != 0)
                b |= 0x80;

            stream.WriteByte(b);
        } while (value != 0);
    }

    public static void WriteLeb128(this Stream stream, long value) {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Can't write to a non-writable stream");

        var more = true;
        while (more) {
            var b = (byte)(value & 0x7f);
            value >>= 7;

            if ((value == 0 && (b & 0x40) == 0) || (value == -1 && (b & 0x40) != 0))
                more = false;
            else
                b |= 0x80;

            stream.WriteByte(b);
        }
    }

#endregion

#endregion
}