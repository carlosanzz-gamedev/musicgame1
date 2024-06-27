using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }
    private Vector3[] _notePositions = new Vector3[4] {
        new Vector3(-1.6f, -0.65f, 0),
        new Vector3(-1.6f, 0.65f, 0),
        new Vector3(1.6f, -0.65f, 0),
        new Vector3(1.6f, 0.65f, 0)
    };

    private Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 endScale = new Vector3(0.5f, 0.5f, 0.5f);
    private float _beatsToHit = 2;
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
                    note.position = Vector3.Lerp(Vector3.zero, _notePositions[position], t);

                    // Interpolate the scale and debug the scale
                    Vector3 newScale = Vector3.Lerp(startScale, endScale, t);
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
        Transform note = Instantiate(_notePrefabs[(int)key - 1], Vector3.zero, Quaternion.identity);
        note.localScale = startScale;
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
