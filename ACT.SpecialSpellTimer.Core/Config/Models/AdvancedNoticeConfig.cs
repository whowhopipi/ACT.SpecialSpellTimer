using System;
using System.IO;
using ACT.SpecialSpellTimer.Sound;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Bridge;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.Models
{
    [Serializable]
    public class AdvancedNoticeConfig :
        BindableBase,
        ICloneable
    {
        private bool isEnabled;

        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetProperty(ref this.isEnabled, value);
        }

        private bool toMainDevice = true;

        public bool ToMainDevice
        {
            get => this.toMainDevice;
            set => this.SetProperty(ref this.toMainDevice, value);
        }

        private bool toSubDevice;

        public bool ToSubDevice
        {
            get => this.toSubDevice;
            set => this.SetProperty(ref this.toSubDevice, value);
        }

        private bool toDicordTextChat;

        public bool ToDicordTextChat
        {
            get => this.toDicordTextChat;
            set => this.SetProperty(ref this.toDicordTextChat, value);
        }

        public void PlayWave(
            string wave)
        {
            if (string.IsNullOrEmpty(wave))
            {
                return;
            }

            if (!File.Exists(wave))
            {
                return;
            }

            if (!this.IsEnabled)
            {
                ActGlobals.oFormActMain.PlaySound(wave);
                return;
            }

            if (this.ToMainDevice)
            {
                PlayBridge.Instance.PlayMainDeviceDelegate?.Invoke(wave);
            }

            if (this.ToSubDevice)
            {
                PlayBridge.Instance.PlaySubDeviceDelegate?.Invoke(wave);
            }
        }

        public void Speak(
            string tts)
        {
            if (string.IsNullOrEmpty(tts))
            {
                return;
            }

            tts = TTSDictionary.Instance.ReplaceWordsTTS(tts);

            if (!this.IsEnabled)
            {
                ActGlobals.oFormActMain.TTS(tts);
                return;
            }

            if (this.ToMainDevice)
            {
                PlayBridge.Instance.PlayMainDeviceDelegate?.Invoke(tts);
            }

            if (this.ToSubDevice)
            {
                PlayBridge.Instance.PlaySubDeviceDelegate?.Invoke(tts);
            }

            if (this.ToDicordTextChat)
            {
                DiscordBridge.Instance.SendMessageDelegate?.Invoke(tts);
            }
        }

        public object Clone() => this.MemberwiseClone();
    }
}
