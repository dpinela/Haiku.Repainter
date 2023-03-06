namespace Haiku.Repainter
{
    internal struct Palette
    {
        public double Xr;
        public double Yr;
        public double Xb;
        public double Yb;

        public Palette(RNG rng)
        {
            var angle = rng.NextFloat() * Math.PI * 2;
            var m = angle % (Math.PI / 2);
            var invdiag = m < Math.PI / 4 ? Math.Cos(m) : Math.Sin(m);

            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            var flip = rng.NextBool() ? -1 : 1;

            Xr = cos * invdiag;
            Yr = sin * invdiag;
            Xb = -sin * invdiag * flip;
            Yb = cos * invdiag * flip;
        }

        public override String ToString() => $"{{Xr={Xr}, Yr={Yr}, Xb={Xb}, Yb={Yb}}}";

        public static Palette[] GenerateN(int n, RNG rng)
        {
            var pals = new Palette[n];
            for (var i = 0; i < n; i++)
            {
                pals[i] = new(rng);
            }
            return pals;
        }
    }
}