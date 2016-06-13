using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour {

    [Serializable]
    public class AudioCtrl
    {
        public float[] pitchRanges;
        public float[] playTimes;
        public float playTimer;
        public float interval;
        public float intervalTimer;
        public int cordCount;
        public int rangeCount;

        public AudioCtrl()
        {
            playTimer = 0.0f;
            interval = 0.0f;
            intervalTimer = 0.0f;
            cordCount = 0;
            rangeCount = 0;
        }

        public void CalculateAudio (float measure, int minFreq, int maxFreq, float low, float high)
        {
            float playTotal = 0.0f;

            cordCount = Random.Range(minFreq, maxFreq);
            playTimes = new float[cordCount];
            pitchRanges = new float[cordCount];
            for(int i = 0; i < cordCount; i++)
            {
                playTimes[i] = Random.Range(minFreq / cordCount, measure / cordCount);
                playTotal += playTimes[i];
                pitchRanges[i] = Random.Range(low, high);
            }
            playTimer = playTimes[0];

            interval = (measure - playTotal) / cordCount;
            intervalTimer = interval;
        }

        public void PlaySoundLine(AudioSource source)
        {
            if(rangeCount >= cordCount)
            {
                rangeCount = 0;
            }

            if(playTimer > 0)
            {
                playTimer -= Time.deltaTime;
                if (!source.isPlaying)
                {
                    source.pitch = pitchRanges[rangeCount];
                    source.Play();
                    rangeCount++;
                }
            }

            else if(playTimer <= 0)
            {
                source.Stop();

                if(intervalTimer > 0)
                {
                    intervalTimer -= Time.deltaTime;
                }
                else if(intervalTimer <= 0)
                {
                    playTimer = playTimes[rangeCount];
                    intervalTimer = interval;
                }
            }
        }
    }

    public static SoundManager instance = null;

    public AudioSource highSource;
    public AudioSource midSource;
    public AudioSource lowSource;

    public AudioSource efx2Source;
    public AudioSource efxSource;
    public AudioSource musicSource;
    public AudioSource ambientSource;

    public float lowPitchRange = 0.0f;
    public float highPitchRange = 0.0f;

    public float measure = 0.0f;

    public AudioCtrl baseAudio;
    public AudioCtrl midAudio;
    public AudioCtrl highAudio;

    //public float[] basePlayTime;
    //public float basePlayTimer = 0.0f;
    //public float baseInterval = 0.0f;
    //public float baseIntervalTimer = 0.0f;
    //public int baseCords;
    //private float[] basePitchRanges;
    //public int basePitchRangeCount = 0;

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        //this setting is for PCG music
        //lowPitchRange = 0.25f;
        //highPitchRange = 1.75f;

        lowPitchRange = 0.95f;
        highPitchRange = 1.05f;


        baseAudio = new AudioCtrl();
        midAudio = new AudioCtrl();
        highAudio = new AudioCtrl();

        FormAudio();

        //Init();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	private void Update ()
    {
        //if (playPCGMusic)
        //{
        //    // PlaySoundLine(lowSource, basePlayTime, ref basePlayTimer, baseInterval, ref baseIntervalTimer, baseCords, basePitchRanges, ref basePitchRangeCount);
        //    baseAudio.PlaySoundLine(lowSource);
        //    midAudio.PlaySoundLine(midSource);
        //    highAudio.PlaySoundLine(highSource);
        //}

        //if (playMusic)
        //{
        //    PlaySingle(musicSource);
        //}
        
    }

    public void FormAudio()
    {
        measure = Random.Range(1.0f, 20.0f);

        baseAudio.CalculateAudio(measure, 3, 7, lowPitchRange, highPitchRange);
        midAudio.CalculateAudio(measure, 2, 6, lowPitchRange, highPitchRange);
        highAudio.CalculateAudio(measure, 5, 10, lowPitchRange, highPitchRange);
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void PlaySingle2(AudioClip clip)
    {
        efx2Source.clip = clip;
        efx2Source.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
    public void RandomizeSfx2(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efx2Source.pitch = randomPitch;
        efx2Source.clip = clips[randomIndex];
        efx2Source.Play();
    }

    public static class AudioFade
    {
        public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime, float EndVolume)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > EndVolume)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }

        public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
        {
            audioSource.volume = 0;
            audioSource.Play();

            float endVolume = 1f;
            while (audioSource.volume < 1)
            {
                audioSource.volume += endVolume * Time.deltaTime / FadeTime;

                yield return null;
            }
        }

        public static IEnumerator TurnDown(AudioSource audioSource, float FadeTime)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0.5f)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }

        }

        public static IEnumerator TurnUp(AudioSource audioSource, float FadeTime)
        {
            audioSource.volume = 0.5f;
            audioSource.Play();

            float endVolume = 1f;
            while (audioSource.volume < 1)
            {
                audioSource.volume += endVolume * Time.deltaTime / FadeTime;

                yield return null;
            }
        }
    }

    //private void Init()
    //{
    //    measure = Random.Range(3.0f, 20.0f);
    //    float playTotal = 0.0f;

    //    baseCords = Random.Range(3, 7);
    //    basePlayTime = new float [baseCords];
    //    basePitchRanges = new float[baseCords];

    //    for(int i = 0; i < baseCords; i++)
    //    {
    //        basePlayTime[i] = Random.Range(3.0f / baseCords, measure / baseCords);
    //        playTotal += basePlayTime[i];
    //        basePitchRanges[i] = Random.Range(lowPitchRange, highPitchRange);
    //    }
    //    basePlayTimer = basePlayTime[0];

    //    baseInterval = (measure - playTotal) / baseCords;
    //    baseIntervalTimer = baseInterval;
    //}

    //private void PlaySoundLine(AudioSource audio, 
    //                                float[] playTime, ref float playTimer, 
    //                                float interval, ref float intervalTimer,    
    //                                int cords, float[] pitchRanges, ref int pitchRangeCount)
    //{
    //    if(pitchRangeCount >= cords)
    //    {
    //        pitchRangeCount = 0;
    //    }

    //    if(playTimer > 0)
    //    {
    //        playTimer -= Time.deltaTime;
    //        if (!audio.isPlaying)
    //        {
    //            audio.pitch = pitchRanges[pitchRangeCount];
    //            audio.Play();
    //            pitchRangeCount++;
    //        }
    //    }

    //    else if (playTimer <= 0)
    //    {
    //        audio.Stop();

    //        if (intervalTimer > 0)
    //        {
    //            intervalTimer -= Time.deltaTime;
    //        }
    //        else if (intervalTimer <= 0)
    //        {
    //            playTimer = playTime[pitchRangeCount];
    //            intervalTimer = interval;
    //        }
    //    }
    //}


}
