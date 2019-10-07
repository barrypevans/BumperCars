using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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


            CreateLoop("Music_Menu", .6f, false);
            CreateLoop("Music_Credits", .8f, false);
            CreateLoop("Music_Gameplay", .6f, false);

            //prepare future scene loads
            SceneManager.sceneLoaded += OnSceneLoad;
            //mention first scene load
            TransitionMusic("Music_Menu");
        }
    }

    //Handle pitch and volume adjustment on caller end?
    public AudioSource CreateOneShot(string s, float volume)
    {
        AudioSource oneShot = gameObject.AddComponent<AudioSource>();
        oneShot.clip = FindClip(s);
        oneShot.name = oneShot.clip.name;
        oneShot.Play();
        oneShot.volume = volume;
        Sources.Add(oneShot);
        StartCoroutine(DelayedClipDestruction(oneShot));
        return oneShot;
    }
    public AudioLoop CreateLoop(string s, float capVolume, bool startLoud)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = FindClip(s);
        source.loop = true;
        source.name = s;
        if (!startLoud) source.volume = 0;
        source.Play();
        AudioLoop loop = gameObject.AddComponent<AudioLoop>().Initialize(source,capVolume,startLoud);
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

        TransitionMusic("Music_"+scene.name);

    }

    private string previousTheme = "";
    Coroutine menuIntroFade;
    bool firstCreditsEntry = true;
    public void TransitionMusic(string s)
    {
        AudioLoop activeMusic = FindAudioLoop(s);
        bool MenuOpen = (previousTheme == "Music_Credits" || previousTheme == "Music_Menu" || previousTheme == "Music_CarSelect");

        AudioSource intro;
        switch(previousTheme + ">" + s)
        {
            case ">Music_Menu":
                intro = CreateOneShot("Music_MenuIntro", 1);
                FindAudioLoop("Music_Menu").FadeAbsolute(true, 0f);
                FindAudioLoop("Music_Menu").source.Stop();
                FindAudioLoop("Music_Menu").source.PlayScheduled(AudioSettings.dspTime + intro.clip.length);
                break;
            case "Music_Gameplay>Music_CarSelect":
                if (FindAudioSource("Music_MenuIntro") != null)
                {
                    TerminateSource(FindAudioSource("Music_MenuIntro"));
                }
                FindAudioLoop("Music_Gameplay").FadeAbsolute(false, 2f);

                firstCreditsEntry = true;
                intro = CreateOneShot("Music_MenuIntro", 1);
                FindAudioLoop("Music_Menu").FadeAbsolute(true, 0f);
                FindAudioLoop("Music_Menu").source.PlayScheduled(AudioSettings.dspTime + intro.clip.length);
                break;
            case "Music_Credits>Music_CarSelect":
            case "Music_Credits>Music_Menu":
                //fade in menu intro
                //use menuintro if available
                if (FindAudioSource("Music_MenuIntro"))
                {
                    //If fading menu intro out, stop
                    if (menuIntroFade != null)
                        StopCoroutine(menuIntroFade);
                    //fade in menuIntro
                    menuIntroFade = StartCoroutine(MenuThemeFade(true, 1f));
                }
                FindAudioLoop("Music_Menu").FadeAbsolute(true, 1f);
                //fade out credits
                FindAudioLoop("Music_Credits").FadeAbsolute(false,1f);
                break;

            case "Music_Menu>Music_Credits":
                if (firstCreditsEntry)
                {
                    FindAudioLoop("Music_Credits").source.Play();
                    FindAudioLoop("Music_Credits").FadeAbsolute(true, 0);
                    firstCreditsEntry = false;
                }
                else
                {
                    FindAudioLoop("Music_Credits").FadeAbsolute(true, 1);
                }
                //Edge case: fade out music intro instead of loop, instead set loop to 0 so delayed doesn't sneak in
                if (FindAudioSource("Music_MenuIntro"))
                {
                    FindAudioLoop("Music_Menu").FadeAbsolute(false, 0f);
                    if (menuIntroFade != null)
                        StopCoroutine(menuIntroFade);

                    menuIntroFade = StartCoroutine(MenuThemeFade(false, 1f));
                }
                else
                    FindAudioLoop("Music_Menu").FadeAbsolute(false, 1f);
                break;
            case "Music_CarSelect>Music_Menu":
                break;
            case "Music_Menu>Music_Gameplay":
            case "Music_CarSelect>Music_Gameplay":
                //cancel play scheduled
                FindAudioLoop("Music_Menu").FadeAbsolute(false, 0f);
                //Edge case: fade out music intro instead of loop
                if (FindAudioSource("Music_MenuIntro"))
                {
                    if (menuIntroFade != null)
                        StopCoroutine(menuIntroFade);

                    menuIntroFade = StartCoroutine(MenuThemeFade(false, 1f));
                }
                else
                    FindAudioLoop("Music_Menu").FadeAbsolute(false, 1f);
                //fade in gameplay
                AudioLoop gameplay = FindAudioLoop("Music_Gameplay");
                gameplay.FadeAbsolute(true, 1f);
                gameplay.source.Stop();
                gameplay.source.Play();
                break;
        }

        previousTheme = s;
    }
    AudioLoop FindAudioLoop(string s)
    {
        return Loops.Find(loop => loop.source.clip.name == s);
    }
    AudioSource FindAudioSource(string s)
    {
        return Sources.Find(source => source.clip.name == s);
    }

    IEnumerator MenuThemeFade(bool fadeIn, float duration)
    {
        AudioSource menuIntro = FindAudioSource("Music_MenuIntro");

        if (menuIntro != null)
        {
            int start = fadeIn ? 0 : 1;
            int end = fadeIn ? 1 : 0;
            float t = 0;
            while (t < duration)
            {
                menuIntro.volume = Mathf.Lerp(start, end, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
            menuIntro.volume = end;
        }
        else
            Debug.Log("No menuIntro playing");

        menuIntroFade = null;

    }



    public void Honk(string carName)
    {
        string honkName = "RegularCarShortHonk";
        float volume = 1f;
        switch (carName)
        {
            case "Best Byon Buggy":
                honkName = "Double_Honk";
                volume = 1.2f;
                break;
            case "Car-Avaggio":
                honkName = "RegularCarLongHonk";
                volume = 1.4f;
                break;
            case "CritterMobile":
                honkName = "Horn 2";
                volume = .5f;
                break;
            case "Grey":
                honkName = "RegularCarShortHonk";
                volume = 1f;
                break;
            case "Midnight Strider":
                honkName = "Honk_Irl";
                volume = 1f;
                break;
            case "Skyblue Stallion":
                honkName = "UICarHorn";
                volume = 1f;
                break;
            case "Vapor Wagon":
                honkName = "ToyHonk";
                volume = 1.2f;
                break;
            default:
                honkName = "RegularCarShortHonk";
                volume = 1f;
                break;
        }


        CreateOneShot(honkName, volume);
    }
    public void Rev(string carName)
    {
        string revName = "Engine Rev";
        float volume = 1f;
        switch (carName)
        {
            case "Best Byon Buggy":
                revName = "Engine Rev 6";
                volume = 1.5f;
                break;
            case "Car-Avaggio":
                revName = "Engine Rev 5";
                volume = 1.3f;
                break;
            case "CritterMobile":
                revName = "Engine Rev 7";
                volume = 1f;
                break;
            case "Grey":
                revName = "Engine Rev 3";
                volume = 1f;
                break;
            case "Midnight Strider":
                revName = "Engine Rev 4";
                volume = 1f;
                break;
            case "Skyblue Stallion":
                revName = "Engine Rev 2";
                volume = 1f;
                break;
            case "Vapor Wagon":
                revName = "Engine Rev";
                volume = 1.6f;
                break;
            default:
                revName = "Engine Rev 2";
                volume = 1f;
                break;
        }


        CreateOneShot(revName, volume);
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