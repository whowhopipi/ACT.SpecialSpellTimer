using System;
using System.Collections.Generic;
using FFXIV.Framework.FFXIVHelper;

namespace ACT.SpecialSpellTimer.FFXIVHelper
{
    public class Combatant
    {
        public int CastBuffID;
        public float CastDurationCurrent;
        public float CastDurationMax;
        public string CastSkillName = string.Empty;
        public uint CastTargetID;
        public int CurrentCP;
        public int CurrentGP;
        public int CurrentHP;
        public int CurrentMP;
        public int CurrentTP;
        public uint ID;
        public bool IsCasting;
        public int Job;
        public int Level;
        public int MaxCP;
        public int MaxGP;
        public int MaxHP;
        public int MaxMP;
        public int MaxTP;

        /// <summary>フルネーム</summary>
        public string Name = string.Empty;

        /// <summary>イニシャル Naoki Y.</summary>
        public string NameFI = string.Empty;

        /// <summary>イニシャル N. Yoshida</summary>
        public string NameIF = string.Empty;

        /// <summary>イニシャル N. Y.</summary>
        public string NameII = string.Empty;

        public int Order;

        public uint OwnerID;

        public float PosX;

        public float PosY;

        public float PosZ;

        public byte type;

        public double CurrentCastRate =>
            this.CastDurationMax == 0 ?
            0 :
            (double)this.CastDurationCurrent / (double)this.CastDurationMax;

        public double CurrentHPRate =>
            this.MaxHP == 0 ?
            0 :
            (double)this.CurrentHP / (double)this.MaxHP;

        public double Distance =>
            this.Player != null ?
            this.GetDistance(this.Player) : 0;

        public double HorizontalDistance =>
            this.Player != null ?
            this.GetHorizontalDistance(this.Player) : 0;

        public MobType MobType => (MobType)this.type;

        public string Names
        {
            get
            {
                var names = new List<string>();

                if (!string.IsNullOrEmpty(this.Name))
                {
                    names.Add(this.Name);
                }

                if (!string.IsNullOrEmpty(this.NameFI))
                {
                    names.Add(this.NameFI);
                }

                if (!string.IsNullOrEmpty(this.NameIF))
                {
                    names.Add(this.NameIF);
                }

                if (!string.IsNullOrEmpty(this.NameII))
                {
                    names.Add(this.NameII);
                }

                return string.Join("|", names.ToArray());
            }
        }

        public string NamesRegex =>
            this.Names.Replace(@".", @"\.");

        public Combatant Player { get; set; }

        public Job AsJob()
        {
            return Jobs.Find(this.Job);
        }

        public double GetDistance(Combatant target) =>
            (double)Math.Sqrt(
                Math.Pow(this.PosX - target.PosX, 2) +
                Math.Pow(this.PosY - target.PosY, 2) +
                Math.Pow(this.PosZ - target.PosZ, 2));

        public double GetHorizontalDistance(Combatant target) =>
            (double)Math.Sqrt(
                Math.Pow(this.PosX - target.PosX, 2) +
                Math.Pow(this.PosY - target.PosY, 2));

        public string GetName(
            NameStyles style)
        {
            switch (style)
            {
                case NameStyles.FullName:
                    return this.Name;

                case NameStyles.FullInitial:
                    return !string.IsNullOrEmpty(this.NameFI) ? this.NameFI : this.Name;

                case NameStyles.InitialFull:
                    return !string.IsNullOrEmpty(this.NameIF) ? this.NameIF : this.Name;

                case NameStyles.InitialInitial:
                    return !string.IsNullOrEmpty(this.NameII) ? this.NameII : this.Name;

                default:
                    return this.Name;
            }
        }

        public void SetName(
            string fullName)
        {
            this.Name = fullName.Trim();

            if (this.MobType != MobType.Player)
            {
                return;
            }

            var blocks = this.Name.Split(' ');
            if (blocks.Length < 2)
            {
                return;
            }

            this.NameFI = $"{blocks[0]} {blocks[1].Substring(0, 1)}.";
            this.NameIF = $"{blocks[0].Substring(0, 1)}. {blocks[1]}";
            this.NameII = $"{blocks[0].Substring(0, 1)}. {blocks[1].Substring(0, 1)}.";
        }
    }
}
