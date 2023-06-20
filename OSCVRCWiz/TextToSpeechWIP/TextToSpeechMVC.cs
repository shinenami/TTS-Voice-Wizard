using OSCVRCWiz.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.TextToSpeech
{
    public class TextToSpeechMVC
    {
        public class TTSModel
        {
            public string ServiceName { get; set; }
            // Additional properties for the TTS service

            public void Speak(TTSMessageQueue.TTSMessage message, CancellationToken ct)
            {
                // Implementation logic to call the respective TTS API service based on ServiceName
                // Perform the text-to-speech conversion
                Console.WriteLine($"Using {ServiceName} TTS service: {message.text}");
            }
        }

        public class TTSView
        {
            public TTSMessageQueue.TTSMessage GetInput(string InputMode, string text)
            {
                // message to convert to speech;

                TTSMessageQueue.TTSMessage message = new TTSMessageQueue.TTSMessage();
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    message.text = text;
                    message.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text;
                    message.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text; //comboBox2
                    message.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.Text; //comboBox5
                    message.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text;  //comboBox1
                    message.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                    message.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                    message.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                    message.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.Text;  //comboBox4
                    message.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text;  //comboBox3
                    message.STTMode = InputMode;
                    message.AzureTranslateText = "[ERROR]";
                });
                return message;
            }

            public void DisplayOutput(string output)
            {
                Console.WriteLine($"Speech output: {output}");
            }
        }

        public class TTSController
        {
            public List<TTSModel> ttsModels;
            public TTSView ttsView;

            public TTSController()
            {
                ttsModels = new List<TTSModel>();
                ttsView = new TTSView();
            }

            public void AddTTSModel(TTSModel model)
            {
                ttsModels.Add(model);
            }

            public void SpeakText(TTSMessageQueue.TTSMessage message, CancellationToken ct)
            {
                foreach (var model in ttsModels)
                {
                    model.Speak(message, ct);
                }
            }

            public void DisplayOutput(string output)
            {
                ttsView.DisplayOutput(output);
            }
        }

        public class ExampleUsage
        {
            static void Example()
            {
                TTSController ttsController = new TTSController();

                // Add TTS models for different services
                // ttsController.AddTTSModel(new TTSModel { ServiceName = "System Speech" }); //able to test multiple models at once if needed
                ttsController.AddTTSModel(new AmazonPollyTTSModel());


                CancellationTokenSource speechCt = new();
                // Get input from the user
                TTSMessageQueue.TTSMessage input = ttsController.ttsView.GetInput("Amazon Polly", "here are the words I am speaking");

                // Speak the input using all TTS models
                ttsController.SpeakText(input, speechCt.Token);


                // If I want to cancel the speak for what ever reason
                // speechCt.Cancel();


         
            }
        }


    }
}
