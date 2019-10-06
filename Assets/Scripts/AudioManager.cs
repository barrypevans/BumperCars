using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


            //CreateRandomLoop( new string[]{"ToyHonk","RegularCarShortHonk"}, new Vector2(.5f,1), new Vector2(.9f,1.1f), new Vector2(.3f,1f));

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
        AudioSource loop = Camera.main.gameObject.AddComponent<AudioSource>();
        loop.clip = FindClip(s);
        loop.loop = true;
        loop.name = loop.clip.name;
        loop.Play();
        Loops.Add(new AudioLoop(loop, volume, startLoud));
        return Loops[Loops.Count-1];
    }
    public AudioLoop CreateRandomLoop(string[] s, Vector2 volumeRange, Vector2 pitchRange, Vector2 rateRange)
    {
        AudioClip[] clips = new AudioClip[s.Length];
        for (int i = 0; i < s.Length; i++)
            clips[i] = FindClip(s[i]);
        return Camera.main.gameObject.AddComponent<RandomAudioLoop>().Initialize(clips, volumeRange, pitchRange, rateRange);
        
    }

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
            l.sources[0].Stop();
            Destroy(l.sources[0]);
            Loops.Remove(l);
            Destroy(l);
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


}


public class AudioLoop :MonoBehaviour
{
    public float volume;
    public AudioSource[] sources;
    float capVolume = 1f;
    Coroutine fadeCor;

    public AudioLoop() { }
    public AudioLoop(AudioSource source, float capVolume, bool startLoud)
    {
        sources = new AudioSource[1]{ source};
        this.capVolume = capVolume;
        if (startLoud)
        {
            source.volume = capVolume;
            volume = capVolume;
        }
        else
        {
            source.volume = 0;
            volume = 0;
        }
               
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
            fadeCor = StartCoroutine(fadeProcess(capVolume,duration));
        else
            fadeCor = StartCoroutine(fadeProcess(0,duration));
    }
    public void Fade(float destination, float duration)
    {
        if (fadeCor != null)
            StopCoroutine(fadeCor);

        fadeCor = StartCoroutine(fadeProcess(destination, duration));
    }

    IEnumerator fadeProcess(float destinationVolume, float duration)
    {
        foreach (AudioSource a in sources) {
            float startVolume = a.volume;
            float t = 0;
            while (t < duration)
            {
                volume = Mathf.Lerp(startVolume, destinationVolume, t / duration);
                a.volume = volume;
                t += Time.deltaTime;
                yield return null;
            }
        }

    }

    /// <summary>
    /// destroy externally to allow sanitary removal from arrays
    /// </summary>
    public virtual void Cleanup()
    {
        foreach (AudioSource s in sources)
        {
            s.Stop();
            if(s!=null)
                Destroy(s);
        }
    }
}

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
