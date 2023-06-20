using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Text;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using static OSCVRCWiz.TextToSpeech.TextToSpeechMVC;

namespace OSCVRCWiz.TextToSpeech
{
    public class AmazonPollyTTSModel : TTSModel
    {
        // Additional properties specific to Amazon Polly

        public virtual void Speak(TTSMessageQueue.TTSMessage message, CancellationToken ct)
        {
            // Implementation logic specific to Amazon Polly TTS service
            // Perform the text-to-speech conversion using Amazon Polly API
            Console.WriteLine($"Using Amazon Polly TTS service: {message.text}");
            // Add the code to call the Amazon Polly API and convert the text to speech

            var pollyService = new AmazonPollyService();
            pollyService.SpeakAsync(message, ct);
        }
    }

    public class AmazonPollyService
    {

        private readonly AmazonPollyClient _client;

        public AmazonPollyService()
        {
            var AWSaccessKeyId = Settings1.Default.yourAWSKey;
            var AWSsecretKey = Settings1.Default.yourAWSSecret;
            var AWSRegion = RegionEndpoint.GetBySystemName(Settings1.Default.yourAWSRegion);

            _client = new AmazonPollyClient(AWSaccessKeyId, AWSsecretKey, AWSRegion);

        }
        public async Task SpeakAsync(TTSMessageQueue.TTSMessage message, CancellationToken ct)
        {
            try
            {
                var response = await SynthesizeSpeechAsync(message);
                await PlaySpeechAsync(response.AudioStream, message,ct);
                response.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Amazon Polly TTS Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();

            }
            
        }
        private async Task<SynthesizeSpeechResponse> SynthesizeSpeechAsync(TTSMessageQueue.TTSMessage message)
        {
            var request = new SynthesizeSpeechRequest
            {
                Text = message.text,
                // Other request parameters
            };

            return await _client.SynthesizeSpeechAsync(request);
        }

        private async Task PlaySpeechAsync(Stream audioStream,TTSMessageQueue.TTSMessage message, CancellationToken ct)
        {
            using (audioStream)
            {
                MemoryStream memoryStream = new MemoryStream();
                await WriteSpeechToStream(audioStream, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                AudioDevices.playSSMLMP3Stream(memoryStream, message, ct);//Next refactor how audio is played
                memoryStream.Dispose();
            }
        }
        public async Task WriteSpeechToStream(Stream audioStream, MemoryStream output)
        {

            byte[] buffer = new byte[2 * 1024];
            int readBytes;

            while ((readBytes = audioStream.Read(buffer, 0, 2 * 1024)) > 0)
            {
                output.Write(buffer, 0, readBytes);
            }

            // Flushes the buffer to avoid losing the last second or so of
            // the synthesized text.
            output.Flush();

        }




    }
}
