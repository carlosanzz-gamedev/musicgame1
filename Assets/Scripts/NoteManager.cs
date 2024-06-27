using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }
    private Vector3[] _notePositions;
    [SerializeField] private Transform[] _goals;

    [SerializeField] private Vector3 _startScale = Vector3.zero;
    [SerializeField] private Vector3 _endScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 _startPosition = Vector3.zero;
    private float _beatsToHit = 4;
    public List<Transform> _notesLB = new List<Transform>();
    public List<Transform> _notesLT = new List<Transform>();
    public List<Transform> _notesRB = new List<Transform>();
    public List<Transform> _notesRT = new List<Transform>();
    List<int> toRemove = new List<int>();

    [SerializeField] private Transform[] _notePrefabs;

    float _time = 0;


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
        _notePositions = new Vector3[_goals.Length];
        for (int i = 0; i < _goals.Length; i++)
        {
            _notePositions[i] = _goals[i].position;
        }
    }
    private void UpdateListNotes(List<Transform> notes, int position)
    {
        if (notes.Count > 0)
        {
            foreach (Transform note in notes)
            {
                float _spawnTime = note.GetComponent<NoteTest>().spawnTime;
                if (_time - _spawnTime < _beatsToHit)
                {
                    float t = (_time - _spawnTime) / _beatsToHit;
                    t = Mathf.Clamp01(t);

                    // Debugging the value of t
                    Debug.Log($"t: {t}");

                    // Interpolate from the initial position to the target position
                    note.position = Vector3.Lerp(_startPosition, _notePositions[position], t);

                    // Interpolate the scale and debug the scale
                    Vector3 newScale = Vector3.Lerp(_startScale, _endScale, t);
                    Debug.Log($"newScale: {newScale}");
                    note.localScale = newScale;
                }
                else
                {
                    Destroy(note.gameObject);
                    toRemove.Add(notes.IndexOf(note));
                }
            }

            foreach (int i in toRemove)
            {
                notes.RemoveAt(i);
            }
            toRemove.Clear();
        }
    }

    private void Update()
    {
        _time = BeatManager.Instance.GetAudioTime();

        UpdateListNotes(_notesLB, 0);
        UpdateListNotes(_notesLT, 1);
        UpdateListNotes(_notesRB, 2);
        UpdateListNotes(_notesRT, 3);
    }

    public void AddNoteToList(Key key, float time, float holdtime, bool isSpecial, float points)
    {
        Transform note = Instantiate(_notePrefabs[(int)key - 1], _startPosition, Quaternion.identity);
        note.localScale = _startScale;
        note.GetComponent<NoteTest>().spawnTime = time;
        switch (key)
        {
            case Key.LB:
                _notesLB.Add(note);
                break;
            case Key.LT:
                _notesLT.Add(note);
                break;
            case Key.RB:
                _notesRB.Add(note);
                break;
            case Key.RT:
                _notesRT.Add(note);
                break;
        }
    }
}
