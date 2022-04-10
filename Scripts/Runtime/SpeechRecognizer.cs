using System;
using Facebook.WitAi.Data;
using UnityEngine;
using UnityEngine.Events;


namespace DoubTech.SpeechRecognizerVoiceSDK
{
    public class SpeechRecognizer : MonoBehaviour
    {
        [SerializeField] private SpeechRecognitionSystem.SpeechRecognizer _speechRecognizer;

        [SerializeField]
        private UnityEvent<string> _onPartialTranscription = new UnityEvent<string>();
        [SerializeField]
        private UnityEvent<string> _onFullTranscription = new UnityEvent<string>();

        private void Awake()
        {
            _speechRecognizer = new SpeechRecognitionSystem.SpeechRecognizer();
        }

        private void OnEnable()
        {
            AudioBuffer.Instance.Events.OnFloatSampleReady += OnSampleReady;
            AudioBuffer.Instance.StartRecording(this);
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
                _onPartialTranscription.Invoke(_speechRecognizer.GetPartialResult()?.partial);
            }
            else
            {
                _onFullTranscription.Invoke(_speechRecognizer.GetResult()?.text);
            }
        }
    }
}
