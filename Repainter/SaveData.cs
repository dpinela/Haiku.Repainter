namespace Haiku.Repainter
{
    internal class SaveData
    {
        private const string algorithmKey = "repainterAlgorithm";
        private const string seedKey = "repainterSeed";
        private const int currentAlgorithm = 1;

        public int Algorithm;
        public string Seed;

        public static SaveData? Load(ES3File saveFile)
        {
            var algo = saveFile.Load<int>(algorithmKey, 0);
            return algo == 0 ? null : new(saveFile, algo);
        }

        public SaveData(string seed)
        {
            Algorithm = currentAlgorithm;
            Seed = seed;
        }

        private SaveData(ES3File saveFile, int algo)
        {
            Algorithm = algo;
            Seed = saveFile.Load<string>(seedKey, "");
        }

        public void SaveTo(ES3File saveFile)
        {
            saveFile.Save(algorithmKey, currentAlgorithm);
            saveFile.Save(seedKey, Seed);
            saveFile.Sync();
        }
    }
}