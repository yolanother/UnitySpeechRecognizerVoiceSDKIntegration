using System;
using Facebook.WitAi.Data;
using UnityEngine;
using UnityEngine.Events;


namespace DoubTech.SpeechRecognizerVoiceSDK
{
    public class SpeechRecognizer : MonoBehaviour
    {
        public string _languageModelDirPath = "SpeechRecognitionSystem/model/english_small";

        [SerializeField] private SpeechRecognitionSystem.SpeechRecognizer _speechRecognizer;

        [SerializeField]
        private UnityEvent<string> _onPartialTranscription = new UnityEvent<string>();
        [SerializeField]
        private UnityEvent<string> _onFullTranscription = new UnityEvent<string>();

        private string ModelPath => Application.streamingAssetsPath + "/" + _languageModelDirPath;

        private void Awake()
        {
            _speechRecognizer = new SpeechRecognitionSystem.SpeechRecognizer();
        }

        private void OnEnable()
        {
            if (_speechRecognizer.Init(ModelPath))
            {
                AudioBuffer.Instance.Events.OnFloatSampleReady += OnSampleReady;
                AudioBuffer.Instance.StartRecording(this);
            }
        }

        private void OnDisable()
        {
            AudioBuffer.Instance.StopRecording(this);
        }

        private void OnSampleReady(float[] sample)
        {
            int resultReady = _speechRecognizer.AppendAudioData(sample);
            if (resultReady == 0)
            {
                var partial = _speechRecognizer.GetPartialResult()?.partial;
                if (!string.IsNullOrEmpty(partial))
                {
                    _onPartialTranscription.Invoke(partial);
                }
            }
            else
            {
                _onFullTranscription.Invoke(_speechRecognizer.GetResult()?.text);
            }
        }
    }
}
