using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;

    [SerializeField] private AudioSource uiSource;

    [SerializeField] private AudioClip bgm;

    [SerializeField] private AudioClip hoverSound;

    [SerializeField] private AudioClip clickSound;

    
    void Start()
    {   
        if(bgmSource == null){
            return;
        }
        bgmSource.loop = true;
        if(bgm == null){
            return;
        }
        bgmSource.clip = bgm;

        bgmSource.Play();
    }

    public void PlayHoverSound(){
        uiSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound(){
        uiSource.PlayOneShot(clickSound);
    }

    
}
