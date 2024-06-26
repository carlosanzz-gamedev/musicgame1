using System;
using System.Collections;
using UnityEngine;

public class PulseToTheBeat : MonoBehaviour
{
    [SerializeField] private bool _useTestBeat;
    [SerializeField] private float _pulseSize = 1.15f;
    [SerializeField] private float _returnSpeed = 15f;
    private Vector3 _startSize;

    private void Start()
    {
        _startSize = transform.localScale;
        if (_useTestBeat)
        {
            StartCoroutine(TestBeat());
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * _returnSpeed);
    }

    IEnumerator TestBeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Pulse();
        }
    }

    public void Pulse()
    {
        transform.localScale = _startSize * _pulseSize;
    }
}
