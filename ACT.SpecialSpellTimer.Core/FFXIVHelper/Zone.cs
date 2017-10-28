namespace ACT.SpecialSpellTimer.FFXIVHelper
{
    public class Zone
    {
        public int ID = 0;
        public int IDonDB = 0;
        public bool IsAddedByUser = false;
        public string Name = string.Empty;

        public int Rank
        {
            get
            {
                var rank = 255;

                // レイド
                if (this.IDonDB >= 30000 && this.IDonDB < 40000)
                {
                    rank = 119;

                    if (this.Name.Contains("零式") ||
                        this.Name.Contains("Savage"))
                    {
                        rank = 110;
                    }
                }

                // 討滅戦
                if (this.IDonDB >= 20000 && this.IDonDB < 30000)
                {
                    rank = 129;
                }

                // PvP
                if (this.IDonDB >= 40000 && this.IDonDB < 55000)
                {
                    rank = 139;
                }

                // misc
                if (rank == 255)
                {
                    if (this.Name.Contains("Hard"))
                    {
                        rank = 210;
                    }
                }

                return rank;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
