namespace ArcaneLibs.Collections;

public class SeekableStream : Stream {
    private readonly Stream _innerStream;
    private readonly MemoryStream _bufferStream;
    private long _position;

    public SeekableStream(Stream innerStream) {
        _innerStream = innerStream;
        _bufferStream = new MemoryStream();
        _position = 0;
    }

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _bufferStream.Length;

    public override long Position {
        get => _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override void Flush() => _innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) {
        if (_position < _bufferStream.Length) {
            _bufferStream.Position = _position;
            int bytesRead = _bufferStream.Read(buffer, offset, count);
            _position += bytesRead;
            return bytesRead;
        }
        else {
            int bytesRead = _innerStream.Read(buffer, offset, count);
            if (bytesRead > 0) {
                _bufferStream.Write(buffer, offset, bytesRead);
                _position += bytesRead;
            }

            return bytesRead;
        }
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken()) {
        if (_position < _bufferStream.Length) {
            _bufferStream.Position = _position;
            int bytesRead = _bufferStream.Read(buffer.Span);
            _position += bytesRead;
            Console.WriteLine($"Read {bytesRead} bytes from buffer: {_position}");
            return new ValueTask<int>(bytesRead);
        }
        else {
            return _innerStream.ReadAsync(buffer, cancellationToken);
        }
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
        if (_position < _bufferStream.Length) {
            _bufferStream.Position = _position;
            int bytesRead = _bufferStream.Read(buffer, offset, count);
            _position += bytesRead;
            Console.WriteLine($"Read {bytesRead} bytes from buffer: {_position}");
            return Task.FromResult(bytesRead);
        }
        else {
            return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        }
    }

    public override long Seek(long offset, SeekOrigin origin) {
        switch (origin) {
            case SeekOrigin.Begin:
                _position = offset;
                break;
            case SeekOrigin.Current:
                _position += offset;
                break;
            case SeekOrigin.End:
                _position = _bufferStream.Length + offset;
                break;
        }

        return _position;
    }

    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}