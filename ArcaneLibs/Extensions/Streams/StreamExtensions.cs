using System.Text;

namespace ArcaneLibs.Extensions;

public static class StreamExtensions {
	private const bool _debug = false;

	public static long Remaining(this Stream stream) {
		//if (_debug) Console.WriteLine($"stream pos: {stream.Position}, stream len: {stream.Length}, stream rem: {stream.Length - stream.Position}");
		return stream.Length - stream.Position;
	}

	public static int Peek(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't peek a non-readable stream");
		if (!stream.CanSeek)
			throw new InvalidOperationException("Can't peek a non-seekable stream");

		int peek = stream.ReadByte();
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
			int peek = stream.ReadByte();
			if (peek == -1) {
				if (_debug) Console.WriteLine($"Can't peek {count} bytes, only {i} bytes remaining");
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
			int read = stream.ReadByte();
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

		if (_debug) {
			Console.WriteLine($"Expected: {sequence.AsHexString()} ({sequence.AsString()})");
			Console.WriteLine(
				$"Actual:   {stream.Peek(sequence.Count()).AsHexString()} ({stream.Peek(sequence.Count()).AsString()})");
		}

		int readCount = 0;
		foreach (int b in sequence) {
			int read = stream.ReadByte();
			readCount++;
			if (read == -1) {
				stream.Seek(-readCount, SeekOrigin.Current);
				return false;
			}

			if (read != b) {
				if (_debug) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("^^".PadLeft(readCount * 3 + 9));
					Console.ResetColor();
				}

				stream.Seek(-readCount, SeekOrigin.Current);
				return false;
			}
		}

		stream.Seek(-readCount, SeekOrigin.Current);

		return true;
	}

	public static bool StartsWith(this Stream stream, string ascii_seq) {
		return stream.StartsWith(ascii_seq.Select(x => (byte)x));
	}

	public static Stream Skip(this Stream stream, long count = 1) {
		if (!stream.CanSeek)
			throw new InvalidOperationException("Can't skip a non-seekable stream");
		stream.Seek(count, SeekOrigin.Current);
		return stream;
	}

	public static IEnumerable<byte> ReadNullTerminatedField(this Stream stream, IEnumerable<byte>? binaryPrefix = null,
		string? asciiPrefix = null) => ReadTerminatedField(stream: stream, terminator: 0x00, binaryPrefix: binaryPrefix,
		asciiPrefix: asciiPrefix);

	public static IEnumerable<byte> ReadSpaceTerminatedField(this Stream stream, IEnumerable<byte>? binaryPrefix = null,
		string? asciiPrefix = null) => ReadTerminatedField(stream: stream, terminator: 0x20, binaryPrefix: binaryPrefix,
		asciiPrefix: asciiPrefix);

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
			if (_debug)
				Console.WriteLine(
					$"ReadTerminatedField -- pos: {stream.Position}/+{stream.Remaining()}/{stream.Length} | next: {(char)stream.Peek()} | Length: {read}");
			if (stream.Peek() == -1) {
				Console.WriteLine($"Warning: Reached end of stream while reading null-terminated field");
				yield break;
			}

			read++;
			yield return (byte)stream.ReadByte();
		}

		if (stream.Peek() == terminator) stream.Skip();
	}

	public static IEnumerable<byte> ReadToEnd(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		int read = 0;
		while ((read = stream.ReadByte()) != -1)
			yield return (byte)read;
	}

#region Read basic datatypes

	public static ushort ReadUInt16LE(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		var bytes = stream.ReadBytes(2).ToArray();

		if (BitConverter.IsLittleEndian)
			Array.Reverse(bytes);

		Console.WriteLine("ReadUInt16LE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt16(bytes));
		return BitConverter.ToUInt16(bytes);
	}

	public static ushort ReadUInt16BE(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		var bytes = stream.ReadBytes(2).ToArray();

		if (!BitConverter.IsLittleEndian)
			Array.Reverse(bytes);

		Console.WriteLine("ReadUInt16BE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt16(bytes));
		return BitConverter.ToUInt16(bytes);
	}

	public static int ReadInt32BE(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		var bytes = stream.ReadBytes(4).ToArray();

		if (BitConverter.IsLittleEndian)
			Array.Reverse(bytes);

		Console.WriteLine("ReadInt32BE: " + bytes.AsHexString() + " => " + BitConverter.ToInt32(bytes));
		return BitConverter.ToInt32(bytes);
	}

	public static uint ReadUInt32BE(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		var bytes = stream.ReadBytes(4).ToArray();

		if (BitConverter.IsLittleEndian)
			Array.Reverse(bytes);

		Console.WriteLine("ReadUInt32BE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt32(bytes));
		return BitConverter.ToUInt32(bytes);
	}

	public static uint ReadUInt32LE(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		var bytes = stream.ReadBytes(4).ToArray();

		if (!BitConverter.IsLittleEndian)
			Array.Reverse(bytes);

		Console.WriteLine("ReadUInt32LE: " + bytes.AsHexString() + " => " + BitConverter.ToUInt32(bytes));
		return BitConverter.ToUInt32(bytes);
	}

	//read variable length number
	public static int ReadVLQ(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		int result = 0;
		int shift = 0;
		byte b;
		do {
			b = (byte)stream.ReadByte();
			result |= (b & 0b0111_1111) << shift;
			shift += 7;
		} while ((b & 0b1000_0000) != 0);

		return result;
	}

	public static int ReadVLQBigEndian(this Stream stream) {
		if (!stream.CanRead)
			throw new InvalidOperationException("Can't read a non-readable stream");

		int result = 0;
		int shift = 0;
		byte b;
		do {
			b = (byte)stream.ReadByte();
			result = (result << 7) | (b & 0b0111_1111);
		} while ((b & 0b1000_0000) != 0);

		return result;
	}

#endregion

#region Write basic data types

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

	public static void WriteString(this Stream stream, string value) {
		if (!stream.CanWrite)
			throw new InvalidOperationException("Can't write to a non-writable stream");

		var bytes = Encoding.UTF8.GetBytes(value);
		stream.Write(bytes);
	}

#endregion
}
