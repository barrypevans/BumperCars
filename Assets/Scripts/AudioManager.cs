using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    private List<AudioClip> Clips = new List<AudioClip>();
    private List<AudioSource> Sources = new List<AudioSource>();

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

        }
    }

    //Handle pitch and volume adjustment on caller end
    public AudioSource CreateOneShot(string s)
    {
        AudioSource oneShot = Camera.main.gameObject.AddComponent<AudioSource>();
        oneShot.clip = FindClip(s);
        oneShot.name = oneShot.clip.name;
        oneShot.Play();
        Sources.Add(oneShot);
        StartCoroutine(DelayedClipDestruction(oneShot));
        return oneShot;
    }
    public AudioSource CreateLoop(string s)
    {
        AudioSource loop = Camera.main.gameObject.AddComponent<AudioSource>();
        loop.clip = FindClip(s);
        loop.loop = true;
        loop.name = loop.clip.name;
        loop.Play();
        Sources.Add(loop);
        return loop;
    }

    public void TerminateSource(AudioSource a)
    {
        try
        {
            a.Stop();
            Sources.Remove(a);
            Destroy(a);
        }
        catch(System.Exception e)
        {
            print(e.ToString());
        }
    }
    //to be implemented (may handle loops on their own system
    public void TerminateLoop()
    {
    }

    private IEnumerator DelayedClipDestruction(AudioSource c)
    {
        yield return new WaitForSeconds(c.clip.length);
        Sources.Remove(c);
        Destroy(c);
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

//to be implemented
public class RandomAudioLoop
{

}
