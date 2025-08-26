using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
public class CutSceneManager : MonoBehaviour
{
    /*
        use of this script to control the cutscene.

        On awake, show the first image, with chars typing gradually. press anywhere to show instantly.
        
        After all chars get shown, press any key to jump to the next image.

        repeat the same process.

        * for the last of the image, start the game.
    */
    [Header("Slides")]
    [SerializeField] private List<GameObject> bgImages;

    [Header("UI")] // will be assigned per slide
    [SerializeField] private TMP_Text curText;

    [SerializeField] private GameObject ContinueObj;


    private Coroutine showTextCoroutine;

    private float textShowingInterval = 0.03f;

    private int curDialogueLength;

    [SerializeField] private CutSceneAmbienceManager audioManager;

    private bool textCanContinue = false;

    private bool imageCanContinue;

    private bool isTyping;

    private int curSlideIdx;

    [Header("Script used for Opening: 0, Winning: 1, Defeat: 2")]
    [SerializeField] private int usage;

    void Start(){
        curSlideIdx = 0;
        StartSlide(curSlideIdx);
    }

    void Update(){
        EnableContinueHint();

        ContinueSlide();

        ShowAllText();
    }

    private void ContinueSlide(){
        // any click / tap / key pressed
        bool pressed = Input.GetMouseButtonDown(0) || Input.anyKeyDown;
        if(isTyping){
            return;
        }
        if(!imageCanContinue){
            return;
        }
    
        if(pressed){
            if(curSlideIdx < bgImages.Count - 1){
                bgImages[curSlideIdx].SetActive(false);
            }
            
            StartSlide(curSlideIdx + 1);
        }

    }
    private void StartSlide(int index){
        if(index < 0){
            return;
        }
        curSlideIdx = index;
        if(index >= bgImages.Count){
            // opening
            if(usage == 0){
                SceneManager.LoadScene("level1");
                Time.timeScale = 1;
                return;
            }
            // defeat
            if(usage == 2){
                // load main menu.
                SceneManager.LoadScene("MainMenu");
                
                Time.timeScale = 1;
                return;
            }
            // winning
            if(usage == 1){
                // load main menu.
                SceneManager.LoadScene("MainMenu");
                Time.timeScale = 1;
                
                return;
            }
        }
        textCanContinue = false;
        ContinueObj.SetActive(false);

        if(imageCanContinue){
            imageCanContinue = false;
        }
        
        // enable bg image
        bgImages[index].SetActive(true);

        // call ambience.
        PlayAmbience(index);

        // get TMP object
        Transform bgImageTransform = bgImages[curSlideIdx].transform;

        if(bgImageTransform.Find("text") == null){
            imageCanContinue = true;
            isTyping = false;
            textCanContinue = false;
            return;
        }  

        curText = bgImageTransform.Find("text").GetComponent<TMP_Text>();
        // get the length of the text
        curText.ForceMeshUpdate();

        curDialogueLength = curText.textInfo.characterCount;

        curText.maxVisibleCharacters = 0;
        // start typing
        showTextCoroutine = StartCoroutine(StartTyping());
    }


    private IEnumerator StartTyping(){
        isTyping = true;
        for(int i = 0; i < curDialogueLength; i ++){
            curText.maxVisibleCharacters = i + 1;
            yield return new WaitForSecondsRealtime(textShowingInterval);
        }
        isTyping = false;
        imageCanContinue = true;
    }

    private void EnableContinueHint(){
        if(curText == null){
            ContinueObj.SetActive(true);
            imageCanContinue = true;
            return;
        }
        // wait for typewriter to proceed
        if(curText.maxVisibleCharacters < 0.6f * curDialogueLength){
            textCanContinue = false;
            imageCanContinue = false;
            isTyping = true;
            return;
        }
        ContinueObj.SetActive(true);
        textCanContinue = true;
    }

    private void ShowAllText(){
        bool pressed = Input.GetMouseButtonDown(0) || Input.anyKeyDown;
        
        if(isTyping){
            if(textCanContinue){
                if(pressed){
                    StopCoroutine(showTextCoroutine);
                    curText.maxVisibleCharacters = curDialogueLength;
                    isTyping = false;
                    textCanContinue = false;
                    imageCanContinue = true;
                }
                
            }
        }
    }

    private void PlayAmbience(int index){
        // opening
        if(usage == 0){
            // enable ambience
            
            if(index == 1){
                audioManager.PlayLoop(audioManager.screamingAudio, 0.1f);
            }
            if(index == 2){
                audioManager.PlayLoop(audioManager.breathingAudio, 0.1f);
            }
            if(index == 3){
                audioManager.ambienceSource.Stop();
            }
            if(index == 4){
                audioManager.PlayLoop(audioManager.equiptSound, 0.1f);
            }
            if(index == 5){
                audioManager.PlayLoop(audioManager.swordSound, 0.4f);
            }
            return;
        }
        // winning
        if(usage == 1){
            if(index == 0){
                audioManager.PlayLoop(audioManager.swordSound2, 0.1f);
            }
            if(index == 1){
                audioManager.PlayLoop(audioManager.witchBreath, 0.1f);
            }   
            if(index == 2){
                audioManager.PlayLoop(audioManager.magicSound, 0.1f);
            }
            if(index == 3){
                audioManager.PlayLoop(audioManager.peaceful, 0.1f);
            }
            if(index == 4){
                audioManager.PlayLoop(audioManager.fireSound, 0.1f);
            }
            if(index == 5){
                audioManager.PlayLoop(audioManager.glassSound, 0.1f);
            }
            if(index == 6){
                audioManager.PlayLoop(audioManager.knightAnger, 0.1f);
            }
            if(index == 7){
                audioManager.PlayLoop(audioManager.sliceSound, 0.1f);
            }
            return;
        }
        if(usage == 2){
            audioManager.PlayLoop(audioManager.defeatSound, 0.1f);
            return;
        }
    }
}
