using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }
    [SerializeField] private float _bpm;
    [SerializeField] private AudioClip _song;
    [SerializeField] private float _intervalOffset;
    [SerializeField] private AudioSource _muteAudioSource, _songAudioSource;
    [SerializeField] private Intervals[] _intervals;
    [SerializeField] private PulseToTheBeat[] _pulseObjects;

    private float _intervalSamplesLength;
    private float _audioTime = 0;
    private float _lastAudioTime = 0;
    // private bool _songLooped = false;
    private Vector3[] _notePositions = new Vector3[4] {
        new Vector3(-0.5f, -0.5f, 0),
        new Vector3(-0.5f, 0.5f, 0),
        new Vector3(0.5f, -0.5f, 0),
        new Vector3(0.5f, 0.5f, 0)
    };
    private float _beatsToHit = 4;
    
    
    public List<Note> notes = new List<Note>();
    public bool songReady = false;
    private int _noteIndex = 0;
    struct HoldNote {
        public Key[] notes;
        public float endTime;
    }
    private List<HoldNote> _holdNotes = new List<HoldNote>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _songAudioSource.clip = _song;
        _muteAudioSource.clip = _song;
    }

    private void Start()
    {
        _intervalSamplesLength = _muteAudioSource.clip.frequency * 60f / _bpm;

        Debug.Log("Interval length: " + _intervalSamplesLength + " / " + _muteAudioSource.clip.frequency + " / " + _bpm);
    }

    private void FixedUpdate()
    {
        if(!songReady) return;
        if (!_muteAudioSource.isPlaying)
        {
            _muteAudioSource.Play();
        }
        _audioTime = _muteAudioSource.timeSamples / _intervalSamplesLength + _intervalOffset;
        if(!_songAudioSource.isPlaying && _audioTime >= _beatsToHit)
        {
            _songAudioSource.Play();
        }
        
        if (_audioTime < _lastAudioTime)
        {
            // _songLooped = true;
            _noteIndex = 0;
        }

        CheckSongNote(_audioTime);
        CheckHoldNote(_audioTime);

        // foreach (var interval in _intervals)
        // {
            
        //     float sampledTime = _audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm));
        //     // Debug.Log(_audioSource.timeSamples + " / " + sampledTime);
        //     interval.CheckForNewInterval(sampledTime);
        // }
        _lastAudioTime = _audioTime;
    }

    public void CheckSongNote(float audioTime)
    {
        
        // Debug.Log(audioTime + " / " + notes[_noteIndex].time + " / " + notes.Count + " / " + _noteIndex);
        // Debug.Log(_noteIndex);
        if (_noteIndex < notes.Count && audioTime >= notes[_noteIndex].time)
        {
            // string keys = "";
            foreach (var key in notes[_noteIndex].keys)
            {
                // keys += key.ToString() + " ";
                if (key != Key.NONE){
                    NoteManager.Instance.AddNoteToList(key, audioTime, notes[_noteIndex].holdDuration, notes[_noteIndex].isSpecial, notes[_noteIndex].points);
                }
                    // _pulseObjects[(int)key - 1].Pulse();
            }

            // Debug.Log("Time: " + notes[_noteIndex].time + " / Keys: " + keys + " / Duration: " + notes[_noteIndex].duration + " / HoldDuration: " + notes[_noteIndex].holdDuration + " / Special: " + notes[_noteIndex].isSpecial + " / Points: " + notes[_noteIndex].points);

            if (notes[_noteIndex].holdDuration > 0)
            {
                HoldNote holdNote = new HoldNote
                {
                    notes = notes[_noteIndex].keys.ToArray(),
                    endTime = notes[_noteIndex].time + notes[_noteIndex].holdDuration
                };
                _holdNotes.Add(holdNote);
            }

            _noteIndex++;

        }
        // else if (_noteIndex >= notes.Count)
        // {
        //     Debug.Log("End of song");

        // }
        
    }
    public void CheckHoldNote(float audioTime) {
        if (_holdNotes.Count == 0) return;
        for (int i = 0; i < _holdNotes.Count; i++)
        {
            if (audioTime >= _holdNotes[i].endTime)
            {
                // Debug.Log("End of hold note " + _holdNotes[i].endTime);
                _holdNotes.RemoveAt(i);
            }
        }
    }

    public void SetSong(List<Note> song)
    {
        notes = song;
        songReady = true;
    }

    public float GetAudioTime()
    {
        return _audioTime;
    }
    
}

[System.Serializable]
public class Intervals {
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    private int _lastInterval;
    private bool _firstInterval = true;
    
    public float GetIntervalLength(float bpm) {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewInterval(float interval) {
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
            return;
        }
        else if (_firstInterval)
        {
            _trigger.Invoke();
            _firstInterval = false;
        }
    }
}