using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public TextAsset csvFile;  // Asignar el archivo CSV desde el editor
    public List<Note> notes = new List<Note>();
    public BeatManager beatManager;

    void Start()
    {
        LoadNotesFromCSV();
        beatManager.SetSong(notes);
        // StartCoroutine(PlaySong());
    }

    void LoadNotesFromCSV()
    {
        string[] data = csvFile.text.Split(new char[] { '\n' });
        CultureInfo culture = CultureInfo.CreateSpecificCulture("es-ES");

        // Leer cada línea del CSV, empezando desde la segunda línea (índice 1) para saltar el encabezado
        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ';' });
            
            if (row[0] == "") break; // Ignorar líneas vacías (última línea del archivo CSV

            Note note = new Note();
            float temp;

            // foreach (var item in row)
            // {
            //     Debug.Log(item);
            // }
            // Parseo de cada valor y manejo de posibles errores de formato
            if (float.TryParse(row[0].Trim(), NumberStyles.Float, culture, out temp))
                note.time = temp;
            else
                Debug.LogError("Formato incorrecto en 'Time' en la fila " + (i + 1));

            note.keys = ParseKeys(row[1].Trim());

            if (float.TryParse(row[2].Trim(), NumberStyles.Float, culture, out temp))
                note.duration = temp;
            else
                Debug.LogError("Formato incorrecto en 'Duration' en la fila " + (i + 1));

            if (float.TryParse(row[3].Trim(), NumberStyles.Float, culture, out temp))
                note.holdDuration = temp;
            else
                Debug.LogError("Formato incorrecto en 'HoldDuration' en la fila " + (i + 1));

            if (bool.TryParse(row[4].Trim(), out bool tempBool))
                note.isSpecial = tempBool;
            else
                Debug.LogError("Formato incorrecto en 'IsSpecial' en la fila " + (i + 1));

            if (int.TryParse(row[5].Trim(), out int tempInt))
                note.points = tempInt;
            else
                Debug.LogError("Formato incorrecto en 'Points' en la fila " + (i + 1));

            notes.Add(note);
        }

        // Ajustar la duración de cada nota basándose en el tiempo de la siguiente nota
        for (int i = 0; i < notes.Count - 1; i++)
        {
            notes[i].duration = notes[i + 1].time - notes[i].time;
        }

        foreach (var note in notes)
        {
            Debug.Log("Nota en: " + note.time + " segundos. Teclas: " + string.Join(", ", note.keys) + " Duración: " + note.duration + " segundos. Duración de mantenimiento: " + note.holdDuration + " segundos. Es especial: " + note.isSpecial + " Puntos: " + note.points.ToString());
        }

        Debug.Log("-------------> Notas cargadas: " + notes.Count);
    }

    List<Key> ParseKeys(string keysString)
    {
        List<Key> keys = new List<Key>();

        if (string.IsNullOrEmpty(keysString))
        {
            keys.Add(Key.NONE);
            return keys;
        }

        string[] keysArray = keysString.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string key in keysArray)
        {
            if (System.Enum.TryParse(key, true, out Key keyEnum))
            {
                keys.Add(keyEnum);
            }
            else
            {
                Debug.LogError("Invalid key: " + key);
                keys.Add(Key.NONE); // Default to NONE if parsing fails
            }
        }

        return keys;
    }

    IEnumerator PlaySong()
    {
        foreach (var note in notes)
        {
            yield return new WaitForSeconds(note.time);
            HandleNoteSpawn(note);
        }
    }

    void HandleNoteSpawn(Note note)
    {
        // Aquí puedes instanciar la nota y manejar su lógica de entrada
        Debug.Log("Nota en: " + note.time + " segundos. Teclas: " + string.Join(", ", note.keys));
        // Añadir lógica para mantener la nota
        if (note.holdDuration > 0)
        {
            StartCoroutine(HandleNoteHold(note));
        }
    }

    IEnumerator HandleNoteHold(Note note)
    {
        yield return new WaitForSeconds(note.holdDuration);
        // Aquí se maneja la liberación de la nota
        Debug.Log("Liberar nota: " + string.Join(", ", note.keys));
    }
}
