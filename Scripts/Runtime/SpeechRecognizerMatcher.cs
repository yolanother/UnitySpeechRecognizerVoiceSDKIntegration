using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace DoubTech.SpeechRecognizerVoiceSDK
{
    public class SpeechRecognizerMatcher : MonoBehaviour
    {
        [SerializeField] private string _searchString;
        [SerializeField] private bool _useRegularExpressions;
        [SerializeField] private bool _requireStartsWith;

        [SerializeField] private SpeechRecognizer _speechRecognizer;

        [SerializeField] private UnityEvent onMatched = new UnityEvent();

        private Regex _regex;

        private bool _matched = false;

        private void OnValidate()
        {
            if (!_speechRecognizer) _speechRecognizer = GetComponent<SpeechRecognizer>();
        }

        private void Awake()
        {
            if (_useRegularExpressions)
            {
                _regex = new Regex(_searchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            if (!_speechRecognizer) _speechRecognizer = GetComponent<SpeechRecognizer>();
        }

        private void OnEnable()
        {
            _speechRecognizer.OnPartialTranscription.AddListener(OnPartialTranscription);
            _speechRecognizer.OnFullTranscription.AddListener(OnFullTranscription);
        }

        private void OnFullTranscription(string transcription)
        {
            ProcessTranscription(transcription);
            _matched = false;
        }

        private void OnPartialTranscription(string transcription)
        {
            ProcessTranscription(transcription);
        }

        public void ProcessTranscription(string transcription)
        {
            if (_matched) return;

            if (_requireStartsWith)
            {
                if (transcription.StartsWith(_searchString))
                {
                    _matched = true;
                    onMatched.Invoke();
                }
                else if (transcription.Length > _searchString.Length)
                {
                    _matched = true;
                }
            }
            else if (_useRegularExpressions && _regex.Match(transcription).Success)
            {
                _matched = true;
                onMatched.Invoke();
            }
            else if (transcription.Contains(_searchString))
            {
                _matched = true;
                onMatched.Invoke();
            }
        }
    }
}
