namespace ArcaneLibs;

public class RainbowEnumerator(byte skip = 1, int offset = 0, double lengthFactor = 255.0) {
    private const double FullRotation = Math.PI * 2;
    private readonly double _oneThird = lengthFactor / 3;
    private readonly double _twoThirds = lengthFactor / 3 * 2;
    private int _offset = offset;

    public (byte r, byte g, byte b) Next() {
        var v = (
            r: (byte) (128 + 127 * Math.Sin(_offset / lengthFactor * FullRotation)), 
            g: (byte) (128 + 127 * Math.Sin((_offset + lengthFactor * _oneThird) / lengthFactor * FullRotation)), 
            b: (byte) (128 + 127 * Math.Sin((_offset + lengthFactor * _twoThirds) / lengthFactor * FullRotation))
        );
        _offset += skip;
        return v;
    }
}