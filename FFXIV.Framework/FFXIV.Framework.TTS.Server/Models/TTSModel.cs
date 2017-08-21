using System;
using System.Runtime.CompilerServices;
using FFXIV.Framework.Common;
using FFXIV.Framework.TTS.Common;
using FFXIV.Framework.TTS.Common.Models;
using NLog;

namespace FFXIV.Framework.TTS.Server.Models
{
    public class TTSModel :
        MarshalByRefObject,
        ITTSModel
    {
        #region Logger

        private Logger logger = AppLog.DefaultLogger;

        #endregion Logger

        [MethodImpl(MethodImplOptions.NoInlining)]
        public CevioTalkerModel GetCevioTalker() =>
            CevioModel.Instance.GetCevioTalker();

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public bool IsReady() => true;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetCevioTalker(CevioTalkerModel talker) =>
            CevioModel.Instance.SetCevioTalker(talker);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void TextToWave(
            TTSTypes ttsType,
            string textToSpeak,
            string waveFileName,
            int speed)
        {
            switch (ttsType)
            {
                case TTSTypes.Yukkuri:
                    YukkuriModel.Instance.TextToWave(textToSpeak, waveFileName, speed);
                    break;

                case TTSTypes.CeVIO:
                    CevioModel.Instance.TextToWave(textToSpeak, waveFileName);
                    break;
            }

            this.logger.Info($"[{ttsType.ToString()}] Speak {textToSpeak}, wave={waveFileName}");
        }
    }
}
