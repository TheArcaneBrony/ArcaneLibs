namespace ArcaneLibs;

public class RainbowEnumerator(byte skip = 1, int offset = 0, double lengthFactor = 255.0) {
    private const double FullRotation = Math.PI * 2;
    private readonly double _oneThird = lengthFactor / 3;
    private readonly double _twoThirds = lengthFactor / 3 * 2;

    public (byte r, byte g, byte b) Next() {
        var v = (
            r: (byte) (128 + 127 * Math.Sin(offset / lengthFactor * FullRotation)), 
            g: (byte) (128 + 127 * Math.Sin((offset + lengthFactor * _oneThird) / lengthFactor * FullRotation)), 
            b: (byte) (128 + 127 * Math.Sin((offset + lengthFactor * _twoThirds) / lengthFactor * FullRotation))
        );
        offset += skip;
        return v;
    }
}