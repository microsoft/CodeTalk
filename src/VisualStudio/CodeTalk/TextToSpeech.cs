//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk
{
    class TextToSpeech
    {
        static SpeechSynthesizer speaker = new SpeechSynthesizer();
        public static bool IsTextToSpeechEnabled = false;

        private static int speedMultiplier = 5;
        private static int selectedSpeed = 0;

        private const int speakDelayMillis = 500;
        private const int speakExtendedDelayMillis = 900;

        static TextToSpeech()
        {
            speaker.Volume = 100;
        }
        private TextToSpeech()
        {
            //Initiate the Text to Speech Engine
            speaker = new SpeechSynthesizer();
        }

        public enum SpeakDelay
        {
            None,
            Delay,
            ExtendedDelay
        }

        public static void SpeakText(string str, SpeakDelay delay = SpeakDelay.None)
        {
            if (!IsTextToSpeechEnabled) { return; }
            speaker.SpeakAsyncCancelAll();
            if (string.IsNullOrEmpty(str)) { return; }
			switch (delay)
			{
				case SpeakDelay.Delay:
					Thread.Sleep(speakDelayMillis);
					break;
				case SpeakDelay.ExtendedDelay:
					Thread.Sleep(speakExtendedDelayMillis);
					break;
				case SpeakDelay.None:
				default:
					break;
			}
			speaker.Speak(str);
        }

        public static void StopSpeak()
        {
            if (!IsTextToSpeechEnabled) { return; }
            speaker.SpeakAsyncCancelAll();
        }

        public static void ForceSpeakText(string str, SpeakDelay delay = SpeakDelay.None)
        {
            if (string.IsNullOrEmpty(str)) { return; }
            Task.Run(() =>
            {
                switch (delay)
                {
                    case SpeakDelay.Delay:
                        Thread.Sleep(speakDelayMillis);
                        break;
                    case SpeakDelay.ExtendedDelay:
                        Thread.Sleep(speakExtendedDelayMillis);
                        break;
                    case SpeakDelay.None:
                    default:
                        break;
                }
                speaker.SpeakAsyncCancelAll();
                speaker.Speak(str);
            });
        }

        public static void SetSpeechSpeedLow()
        {
            speaker.Rate = -5;
        }

        public static void SetSpeechSpeedMed()
        {
            speaker.Rate = 0;
        }
        public static void SetSpeechSpeedHigh()
        {
            speaker.Rate = 5;
        }

        private static void SetSpeechSpeed()
        {
            int speed = selectedSpeed * speedMultiplier;
            if (speed < 10 && speed > -10)
            {
                speaker.Rate = speed;
                return;
            }
            speaker.Rate = 0;
        }

        public static void IncreaseSpeechSpeed()
        {
            if (selectedSpeed < 1)
            {
                selectedSpeed++;
                SetSpeechSpeed();
            }
        }

        public static void DecreaseSpeechSpeed()
        {
            if (selectedSpeed > -1)
            {
                selectedSpeed--;
                SetSpeechSpeed();
            }
        }
    }
}
