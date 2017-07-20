using System;

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
        public string Name = string.Empty;
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

        public Combatant Player { get; set; }

        public Job AsJob()
        {
            return SpecialSpellTimer.Job.Instance.FromId(Job);
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
    }
}
