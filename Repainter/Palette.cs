namespace Haiku.Repainter
{
    internal struct Palette
    {
        public double Xr;
        public double Yr;
        public double Xb;
        public double Yb;

        public Palette(Random rng)
        {
            var angle = rng.NextDouble() * Math.PI * 2;
            var m = angle % (Math.PI / 2);
            var invdiag = m < Math.PI / 4 ? Math.Cos(m) : Math.Sin(m);

            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            var flip = rng.Next(2) == 0 ? -1 : 1;

            Xr = cos * invdiag;
            Yr = sin * invdiag;
            Xb = -sin * invdiag * flip;
            Yb = cos * invdiag * flip;
        }

        public override String ToString() => $"{{Xr={Xr}, Yr={Yr}, Xb={Xb}, Yb={Yb}}}";
    }
}