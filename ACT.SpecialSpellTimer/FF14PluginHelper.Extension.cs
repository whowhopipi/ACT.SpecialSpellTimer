#if false
namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using ACT.SpecialSpellTimer.Config;

    /// <summary>
    /// FF14Pluginヘルパーの拡張部分
    /// </summary>
    public static partial class FF14PluginHelper
    {
        /// <summary>
        /// 空のCombatantリスト
        /// </summary>
        private static readonly List<Combatant> emptyCombatantList = new List<Combatant>();

        /// <summary>
        /// プレイヤー情報を最後に取得した日時
        /// </summary>
        private static DateTime lastPlayerDateTime = DateTime.MinValue;

        /// <summary>
        /// プレイヤー情報
        /// </summary>
        private static Combatant player;

        /// <summary>
        /// プレイヤ情報の更新間隔
        /// </summary>
        private static double playerInfoRefreshInterval = Settings.Default.PlayerInfoRefreshInterval;

        /// <summary>
        /// パーティの戦闘メンバリストを取得する
        /// </summary>
        /// <returns>パーティの戦闘メンバリスト</returns>
        public static List<Combatant> GetCombatantListParty()
        {
            // FFXIV以外の使用？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                return emptyCombatantList;
            }

            // 総戦闘メンバリストを取得する（周囲のPC, NPC, MOB等すべて）
            var combatListAll = FF14PluginHelper.GetCombatantList();

            // パーティメンバのIDリストを取得する
            int partyCount;
            var partyListById = FF14PluginHelper.GetCurrentPartyList(out partyCount).Take(8);

            var combatListParty = new List<Combatant>();

            foreach (var partyMemberId in partyListById)
            {
                if (partyMemberId == 0)
                {
                    continue;
                }

                var partyMember = (
                    from x in combatListAll
                    where
                    x.ID == partyMemberId
                    select
                    x).FirstOrDefault();

                if (partyMember != null)
                {
                    combatListParty.Add(partyMember);
#if DEBUG
                    Debug.WriteLine("<" + combatListParty.Count().ToString() + "> " + partyMember.Name);
#endif
                }
            }

            return combatListParty;
        }

        /// <summary>
        /// プレイヤー情報を取得する
        /// </summary>
        /// <returns>プレイヤー情報</returns>
        public static Combatant GetPlayer()
        {
            // FFXIV以外の使用？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                return null;
            }

            // 3分以上経過した？
            if (player == null ||
                lastPlayerDateTime <= DateTime.MinValue ||
                (DateTime.Now - lastPlayerDateTime).TotalMinutes >= playerInfoRefreshInterval)
            {
                RefreshPlayer();
            }

            return player;
        }

        /// <summary>
        /// プレイヤ情報をリフレッシュする
        /// </summary>
        public static void RefreshPlayer()
        {
            var list = FF14PluginHelper.GetCombatantList();
            if (list.Count > 0)
            {
                player = list[0];
                lastPlayerDateTime = DateTime.Now;
            }
        }
    }
}
#endif