using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    List<AudioSource> audioSources;

    [SerializeField]
    float volumeChangeSpeed = 0.005f;

    AudioSource m_current;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetVolume(float value)
    {
        m_targetVolume = Mathf.Clamp( value, 0f, 1f);
    }

    float m_volume;
    float m_targetVolume = 0.1f;

    // Update is called once per frame
    void Update()
    {
        SetVolume(GameManager.GetInstance().kills / 50.0f * 0.7f);

        m_volume = m_volume + (m_targetVolume - m_volume) * Time.deltaTime * volumeChangeSpeed;

        if(m_current == null || !m_current.isPlaying)
        {
            m_current = audioSources[Random.Range(0, audioSources.Count)];
            m_current.Play();
        }
        m_current.volume = m_volume;
    }
}
