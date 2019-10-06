using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    //todo: camera management, creating randomloops, fading for loops(generic)

    public static AudioManager instance;
    private List<AudioClip> Clips = new List<AudioClip>();
    private List<AudioSource> Sources = new List<AudioSource>();
    private List<AudioLoop> Loops = new List<AudioLoop>();

    //Singleton
    void Start()
    {
        if(instance != null)
            Destroy(this);
        else
        {
            //Singleton functionality
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Load all audio clips
            Object[] loadedClips = { };
            try
            {
                loadedClips = Resources.LoadAll("Audio", typeof(AudioClip));
            }
            catch(System.Exception e)
            {
                print(e.ToString());
            }
            if (loadedClips.Length > 0)
            {
                foreach (Object o in loadedClips)
                {
                    Clips.Add((AudioClip)o);
                    Clips[Clips.Count - 1].name = o.name;
                }
            }
            else
            {
                print("No clips detected in directory: 'Resources>Audio'");
            }

            //prepare future scene loads
            SceneManager.sceneLoaded += OnSceneLoad;
            //mention first scene load
            OnSceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    //Handle pitch and volume adjustment on caller end?
    public AudioSource CreateOneShot(string s, float volume)
    {
        AudioSource oneShot = Camera.main.gameObject.AddComponent<AudioSource>();
        oneShot.clip = FindClip(s);
        oneShot.name = oneShot.clip.name;
        oneShot.Play();
        oneShot.volume = volume;
        Sources.Add(oneShot);
        StartCoroutine(DelayedClipDestruction(oneShot));
        return oneShot;
    }
    public AudioLoop CreateLoop(string s, float volume, bool startLoud)
    {
        AudioSource source = Camera.main.gameObject.AddComponent<AudioSource>();
        source.clip = FindClip(s);
        source.loop = true;
        source.name = s;
        if (startLoud)
            source.Play();
        AudioLoop loop = Camera.main.gameObject.AddComponent<AudioLoop>().Initialize(source,volume,startLoud);
        Loops.Add(loop);
        return Loops[Loops.Count-1];
    }
    /*
    public AudioLoop CreateRandomLoop(string[] s, Vector2 volumeRange, Vector2 pitchRange, Vector2 rateRange)
    {
        AudioClip[] clips = new AudioClip[s.Length];
        for (int i = 0; i < s.Length; i++)
            clips[i] = FindClip(s[i]);
        return Camera.main.gameObject.AddComponent<RandomAudioLoop>().Initialize(clips, volumeRange, pitchRange, rateRange);
        
    }*/

    public void TerminateSource(AudioSource a)
    {
        try
        {
            if (a != null)
            {
                a.Stop();
                Sources.Remove(a);
                Destroy(a);
            }
        }
        catch(System.Exception e)
        {
            print(e.ToString());
        }
    } 
    public void TerminateLoop(AudioLoop l)
    {
        try
        {
            l.Cleanup();
        }
        catch (System.Exception e)
        {
            print(e.ToString());
        }
    }

    private IEnumerator DelayedClipDestruction(AudioSource c)
    {
        yield return new WaitForSeconds(c.clip.length);
        TerminateSource(c);
    }

    private AudioClip FindClip(string s)
    {
        foreach(AudioClip c in Clips)
        {
            if (c.name == s)
                return c;
        }
        print("Clip: " + s + " could not be found");
        return null;
    }


    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Menu":
                StartCoroutine(DelayMenuMusic());
                break;
            case "MatteColorDrivingTest":
                CreateLoop("Music_Gameplay",1,true);
                break;

        }
    }

    IEnumerator DelayMenuMusic()
    {
        AudioSource primary = CreateOneShot("Music_Menu_Intro", 1);
        AudioLoop secondary = CreateLoop("Music_Menu_Loop", 1, false);
        secondary.source.volume = .95f;
        secondary.volume = .95f;
        secondary.source.PlayScheduled(AudioSettings.dspTime + primary.clip.length);
        yield return null;
    }
}


public class AudioLoop : MonoBehaviour
{
    public float volume;
    public AudioSource source;
    float capVolume = 1f;
    Coroutine fadeCor;
    
    public AudioLoop Initialize(AudioSource s, float capVolume, bool startLoud)
    {
        volume = startLoud ? 1 : 0;
        source = s;
        this.capVolume = capVolume;

        return this;

    }

    /// <summary>
    /// Fades fully in or out from current volume over duration.
    /// </summary>
    /// <param name="toCapVolume"></param>
    /// <param name="duration"></param>
    public void FadeAbsolute(bool toCapVolume, float duration)
    {
        if(fadeCor != null)
            StopCoroutine(fadeCor);

        if (toCapVolume)
            fadeCor = StartCoroutine(FadeProcess(capVolume,duration));
        else
            fadeCor = StartCoroutine(FadeProcess(0,duration));
    }
    public void Fade(float destination, float duration)
    {
        if (fadeCor != null)
            StopCoroutine(fadeCor);

        fadeCor = StartCoroutine(FadeProcess(destination, duration));
    }

    IEnumerator FadeProcess(float destinationVolume, float duration)
    {
            float startVolume = source.volume;
            float t = 0;
            while (t < duration)
            {
                volume = Mathf.Lerp(startVolume, destinationVolume, t / duration);
                source.volume = volume;
                t += Time.deltaTime;
                yield return null;
            }
        volume = destinationVolume;
        source.volume = volume;
    }


    /// <summary>
    /// destroy externally to allow sanitary removal from arrays
    /// </summary>
    public void Cleanup()
    {
        source.Stop();
        if(source!=null)
            Destroy(source);
    }
}
/*
public class RandomAudioLoop : AudioLoop
{
    Coroutine loopCor;
    float decayTime = 0f;
    public AudioLoop Initialize(AudioClip[] sourceClips, Vector2 volumeRange, Vector2 pitchRange, Vector3 rateRange)
    {
        loopCor = StartCoroutine(Loop(sourceClips, volumeRange,pitchRange,rateRange));
        return this;
    }
   
    IEnumerator Loop(AudioClip[] sourceClips, Vector2 volumeRange, Vector2 pitchRange, Vector2 rateRange)
    {
        while (true)
        {
            float t = 0;
            float duration = Random.Range(rateRange.x, rateRange.y);
            while (t < duration)
            {
                decayTime -= Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
            AudioClip clip = sourceClips[Random.Range(0, sourceClips.Length)];
            if (clip.length > decayTime)
                decayTime = clip.length + .01f;
            CreateOneShot(clip, Random.Range(volumeRange.x,volumeRange.y), Random.Range(pitchRange.x,pitchRange.y));
        }
    }

    public AudioSource CreateOneShot(AudioClip clip, float volume, float pitch)
    {
        AudioSource oneShot = Camera.main.gameObject.AddComponent<AudioSource>();
        oneShot.clip = clip;
        oneShot.name = oneShot.clip.name;
        oneShot.Play();
        oneShot.volume = volume;
        oneShot.pitch = pitch;
        Destroy(oneShot,oneShot.clip.length);
        return oneShot;
    }

    public override void Cleanup()
    {
        StopCoroutine(loopCor);
        Destroy(this, decayTime);
    }
}
*/