﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace XIVDBDownloader.Constants
{
    public enum JobIDs
    {
        Unknown = -1,
        ADV = 0,
        GLA = 1,
        PUG = 2,
        MRD = 3,
        LNC = 4,
        ARC = 5,
        CNJ = 6,
        THM = 7,
        CRP = 8,
        BSM = 9,
        ARM = 10,
        GSM = 11,
        LTW = 12,
        WVR = 13,
        ALC = 14,
        CUL = 15,
        MIN = 16,
        BOT = 17,
        FSH = 18,
        PLD = 19,
        MNK = 20,
        WAR = 21,
        DRG = 22,
        BRD = 23,
        WHM = 24,
        BLM = 25,
        ACN = 26,
        SMN = 27,
        SCH = 28,
        ROG = 29,
        NIN = 30,
        MCH = 31,
        DRK = 32,
        AST = 33,
        SAM = 34,
        RDM = 35,
    }

    public enum Roles
    {
        Unknown = 0,
        Tank = 113,
        Healer = 117,
        MeleeDPS = 114,
        RangeDPS = 115,
        MagicDPS = 116,
        PhysicalDPS = 118,
        DPS = 119,
        Magic = 120,
        Crafter = 210,
        Gatherer = 220,
        PetsEgi = 28,
        PetsFairy = 29,
    }

    public class Job
    {
        public JobIDs ID { get; set; } = JobIDs.Unknown;
        public string NameDE { get; set; } = string.Empty;
        public string NameEN { get; set; } = string.Empty;
        public string NameFR { get; set; } = string.Empty;
        public string NameJA { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.Unknown;

        public string GetName(
            Language language)
        {
            switch (language)
            {
                case Language.EN: return this.NameEN;
                case Language.JA: return this.NameJA;
                case Language.FR: return this.NameFR;
                case Language.DE: return this.NameDE;
                default: return this.NameEN;
            }
        }
    }

    public class Jobs
    {
        private static readonly Job[] jobs = new Job[]
        {
            new Job() {ID = JobIDs.Unknown, Role = Roles.Unknown, NameEN = "Unknown", NameJA = "Unknown", NameFR = "Unknown", NameDE = "Unknown" },
            new Job() {ID = JobIDs.ADV, Role = Roles.Unknown, NameEN = "Adventurer", NameJA = "冒険者", NameFR = "Aventurier", NameDE = "Abenteurer" },
            new Job() {ID = JobIDs.GLA, Role = Roles.Tank, NameEN = "Gladiator", NameJA = "剣術士", NameFR = "Gladiateur", NameDE = "Gladiator" },
            new Job() {ID = JobIDs.PUG, Role = Roles.MeleeDPS, NameEN = "Pugilist", NameJA = "拳闘士", NameFR = "Pugiliste", NameDE = "Faustk\u00e4mpfer" },
            new Job() {ID = JobIDs.MRD, Role = Roles.Tank, NameEN = "Marauder", NameJA = "斧術士", NameFR = "Maraudeur", NameDE = "Marodeur" },
            new Job() {ID = JobIDs.LNC, Role = Roles.MeleeDPS, NameEN = "Lancer", NameJA = "槍術士", NameFR = "Ma\u00eetre D'hast", NameDE = "Pikenier" },
            new Job() {ID = JobIDs.ARC, Role = Roles.RangeDPS, NameEN = "Archer", NameJA = "弓術士", NameFR = "Archer", NameDE = "Waldl\u00e4ufer" },
            new Job() {ID = JobIDs.CNJ, Role = Roles.Healer, NameEN = "Conjurer", NameJA = "幻術士", NameFR = "\u00e9l\u00e9mentaliste", NameDE = "Druide" },
            new Job() {ID = JobIDs.THM, Role = Roles.MagicDPS, NameEN = "Thaumaturge", NameJA = "呪術師", NameFR = "Occultiste", NameDE = "Thaumaturg" },
            new Job() {ID = JobIDs.CRP, Role = Roles.Crafter, NameEN = "Carpenter", NameJA = "木工師", NameFR = "Menuisier", NameDE = "Zimmerer" },
            new Job() {ID = JobIDs.BSM, Role = Roles.Crafter, NameEN = "Blacksmith", NameJA = "鍛冶師", NameFR = "Forgeron", NameDE = "Grobschmied" },
            new Job() {ID = JobIDs.ARM, Role = Roles.Crafter, NameEN = "Armorer", NameJA = "甲冑師", NameFR = "Armurier", NameDE = "Plattner" },
            new Job() {ID = JobIDs.GSM, Role = Roles.Crafter, NameEN = "Goldsmith", NameJA = "彫金師", NameFR = "Orf\u00e8vre", NameDE = "Goldschmied" },
            new Job() {ID = JobIDs.LTW, Role = Roles.Crafter, NameEN = "Leatherworker", NameJA = "革細工師", NameFR = "Tanneur", NameDE = "Gerber" },
            new Job() {ID = JobIDs.WVR, Role = Roles.Crafter, NameEN = "Weaver", NameJA = "裁縫師", NameFR = "Couturier", NameDE = "Weber" },
            new Job() {ID = JobIDs.ALC, Role = Roles.Crafter, NameEN = "Alchemist", NameJA = "錬金術師", NameFR = "Alchimiste", NameDE = "Alchemist" },
            new Job() {ID = JobIDs.CUL, Role = Roles.Crafter, NameEN = "Culinarian", NameJA = "調理師", NameFR = "Cuisinier", NameDE = "Gourmet" },
            new Job() {ID = JobIDs.MIN, Role = Roles.Gatherer, NameEN = "Miner", NameJA = "採掘師", NameFR = "Mineur", NameDE = "Minenarbeiter" },
            new Job() {ID = JobIDs.BOT, Role = Roles.Gatherer, NameEN = "Botanist", NameJA = "園芸師", NameFR = "Botaniste", NameDE = "G\u00e4rtner" },
            new Job() {ID = JobIDs.FSH, Role = Roles.Gatherer, NameEN = "Fisher", NameJA = "漁師", NameFR = "P\u00eacheur", NameDE = "Fischer" },
            new Job() {ID = JobIDs.PLD, Role = Roles.Tank, NameEN = "Paladin", NameJA = "ナイト", NameFR = "Paladin", NameDE = "Paladin" },
            new Job() {ID = JobIDs.MNK, Role = Roles.MeleeDPS, NameEN = "Monk", NameJA = "モンク", NameFR = "Moine", NameDE = "M\u00f6nch" },
            new Job() {ID = JobIDs.WAR, Role = Roles.Tank, NameEN = "Warrior", NameJA = "戦士", NameFR = "Guerrier", NameDE = "Krieger" },
            new Job() {ID = JobIDs.DRG, Role = Roles.MeleeDPS, NameEN = "Dragoon", NameJA = "竜騎士", NameFR = "Chevalier Dragon", NameDE = "Dragoon" },
            new Job() {ID = JobIDs.BRD, Role = Roles.RangeDPS, NameEN = "Bard", NameJA = "吟遊詩人", NameFR = "Barde", NameDE = "Barde" },
            new Job() {ID = JobIDs.WHM, Role = Roles.Healer, NameEN = "White Mage", NameJA = "白魔道士", NameFR = "Mage Blanc", NameDE = "Wei\u00dfmagier" },
            new Job() {ID = JobIDs.BLM, Role = Roles.MagicDPS, NameEN = "Black Mage", NameJA = "黒魔道士", NameFR = "Mage Noir", NameDE = "Schwarzmagier" },
            new Job() {ID = JobIDs.ACN, Role = Roles.MagicDPS, NameEN = "Arcanist", NameJA = "巴術士", NameFR = "Arcaniste", NameDE = "Hermetiker" },
            new Job() {ID = JobIDs.SMN, Role = Roles.MagicDPS, NameEN = "Summoner", NameJA = "召喚士", NameFR = "Invocateur", NameDE = "Beschw\u00f6rer" },
            new Job() {ID = JobIDs.SCH, Role = Roles.Healer, NameEN = "Scholar", NameJA = "学者", NameFR = "\u00e9rudit", NameDE = "Gelehrter" },
            new Job() {ID = JobIDs.ROG, Role = Roles.MeleeDPS, NameEN = "Rogue", NameJA = "双剣士", NameFR = "Surineur", NameDE = "Schurke" },
            new Job() {ID = JobIDs.NIN, Role = Roles.MeleeDPS, NameEN = "Ninja", NameJA = "忍者", NameFR = "Ninja", NameDE = "Ninja" },
            new Job() {ID = JobIDs.MCH, Role = Roles.RangeDPS, NameEN = "Machinist", NameJA = "機工士", NameFR = "Machiniste", NameDE = "Maschinist" },
            new Job() {ID = JobIDs.DRK, Role = Roles.Tank, NameEN = "Dark Knight", NameJA = "暗黒騎士", NameFR = "Chevalier Noir", NameDE = "Dunkelritter" },
            new Job() {ID = JobIDs.AST, Role = Roles.Healer, NameEN = "Astrologian", NameJA = "占星術師", NameFR = "Astromancien", NameDE = "Astrologe" },
            new Job() {ID = JobIDs.SAM, Role = Roles.MeleeDPS, NameEN = "Samurai", NameJA = "侍", NameFR = "Samoura\u00ef", NameDE = "Samurai" },
            new Job() {ID = JobIDs.RDM, Role = Roles.MagicDPS, NameEN = "Red Mage", NameJA = "赤魔道士", NameFR = "Mage Rouge", NameDE = "Rotmagier" },
        };

        public static IReadOnlyList<Job> List => jobs;

        public static IReadOnlyList<Job> SortedList => (
            from x in jobs
            orderby
            x.Role,
            x.ID
            select
            x).ToList();

        public static Job Find(
            JobIDs id)
        {
            return jobs.Where(x => x.ID == id).FirstOrDefault() ?? new Job();
        }

        public static Job Find(
            int id)
        {
            return Jobs.Find(Jobs.IntToID(id));
        }

        public static Job Find(
            string id)
        {
            int.TryParse(id, out int idAsInt);
            return Jobs.Find(idAsInt);
        }

        public static JobIDs IntToID(
            int id)
        {
            var idAsEnum = JobIDs.Unknown;
            if (Enum.IsDefined(typeof(JobIDs), id))
            {
                idAsEnum = (JobIDs)id;
            }

            return idAsEnum;
        }
    }
}
