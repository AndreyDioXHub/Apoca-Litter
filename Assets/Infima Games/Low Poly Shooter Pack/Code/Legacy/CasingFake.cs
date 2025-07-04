using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingFake : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _casingSounds;

    [SerializeField] 
    private AudioSource _audioSource;

    [SerializeField]
    private float _rotationSpeed = 2500.0f;
    [SerializeField]
    private Transform _body;

    [SerializeField]
    private float _lifeTime = 5f;
    [SerializeField]
    private float _startSpeed = 2;

    private float _time = 0;
    private float _nexttime = 0;
    private Vector3 _direction;
    private Vector3 _origin;

    private Vector3 _position;

    void Start()
    {

        _direction = transform.right;
        _origin = transform.position;

        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        //Wait for random time before playing sound clip
        yield return new WaitForSeconds(Random.Range(0.25f, 0.85f));
        //Get a random casing sound from the array 
        _audioSource.clip = _casingSounds
            [Random.Range(0, _casingSounds.Length)];
        //Play the random casing sound
        _audioSource.Play();
    }

    void Update()
    {
        if(_body == null)
        {
            return;
        }

        _body.Rotate(Vector3.right, _rotationSpeed * Time.deltaTime);
        _body.Rotate(Vector3.down, _rotationSpeed * Time.deltaTime);

        _time += Time.deltaTime;
        _nexttime = _time + Time.deltaTime;

        _position = _origin + _startSpeed * _direction * _time + new Vector3(0, -9.8f, 0) * _time * _time / 2f;

        transform.position = _position;

        if(_time> _lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
