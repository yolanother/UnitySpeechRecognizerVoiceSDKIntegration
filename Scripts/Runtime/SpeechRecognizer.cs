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

        private bool _initialized;

        private string ModelPath => Application.streamingAssetsPath + "/" + _languageModelDirPath;

        public UnityEvent<string> OnPartialTranscription => _onPartialTranscription;
        public UnityEvent<string> OnFullTranscription => _onFullTranscription;

        private void Awake()
        {
            _speechRecognizer = new SpeechRecognitionSystem.SpeechRecognizer();
        }

        private void OnEnable()
        {
            _initialized = _speechRecognizer.Init(ModelPath);
            if (_initialized)
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
            if (!_initialized) return;

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
