using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// SpecialSpellTimer Plugin
    /// </summary>
    public class Plugin :
        IActPluginV1
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Plugin()
        {
            AssemblyResolver.Instance.Initialize();
        }

        /// <summary>
        /// 後片付けをする
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        void IActPluginV1.DeInitPlugin()
        {
            PluginCore.Instance.DeInitPluginCore();
            Logger.End();
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="pluginScreenSpace">Pluginタブ</param>
        /// <param name="pluginStatusText">Pluginステータスラベル</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        void IActPluginV1.InitPlugin(
            TabPage pluginScreenSpace,
            Label pluginStatusText)
        {
            Logger.Begin();
            PluginCore.Initialize(this);
            PluginCore.Instance.InitPluginCore(
                pluginScreenSpace,
                pluginStatusText);
        }
    }
}
