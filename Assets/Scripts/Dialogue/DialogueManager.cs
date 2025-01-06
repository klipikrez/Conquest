using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yarn.Markup;
using TMPro;
using System;
using UnityEngine.Audio;
using System.IO;


namespace Yarn.Unity.Example
{
    public class DialogueManager : DialogueViewBase
    {
        [SerializeField] DialogueRunner runner;

        [Header("Assets"), Tooltip("you can manually assign various assets here if you don't want to use /Resources/ folder")]
        public List<Sprite> loadSprites = new List<Sprite>();
        public List<AudioClip> loadAudio = new List<AudioClip>();

        [Tooltip("if enabled: will automatically load all Sprites and AudioClips in any /Resources/ folder including any subfolders")]
        public bool useResourcesFolders = false;


        [Header("Sprite UI settings")] // UI tuning variables and references
        [Tooltip("all sprites will be tinted with this color")]
        public Color defaultTint;
        [Tooltip("when speaking, a sprite will be highlighted by tinting it with this color")]
        public Color highlightTint;


        [Header("Object references"), Tooltip("don't change these unless you know what you're doing")]
        public RectTransform spriteGroup; // used for screenshake
        public Image bgImage, fadeBG, nameplateBG;
        public Image genericSprite; // local prefab, used for instantiating sprites
        public AudioSource genericAudioSource; // local prefab, used for instantiating sounds

        // big lists to keep track of all instantiated objects
        List<AudioSource> sounds = new List<AudioSource>(); // big list of all instantiated sounds
        public AudioMixerGroup audioGroup;
        List<Image> sprites = new List<Image>(); // big list of all instantianted sprites

        // store sprite references for "actors" (characters, etc.)
        [HideInInspector] public Dictionary<string, VNActor> actors = new Dictionary<string, VNActor>(); // tracks names to sprites

        static Vector2 screenSize = new Vector2(1280f, 720f); // needed for position calcuations, e.g. what does "left" mean?

        public static DialogueManager Instance;
        public bool onStartDialogue = false;
        public string scriptToLoadOnStart;
        [System.NonSerialized]
        public bool inDialogue = false;
        private LevelMenu levelMenu = null;
        private Coroutine moveCameraCoroutine = null;

        public void Initiate()
        {
            Instance = this;

            // adds all Resources to internal lists / one big pile... it
            // will scan inside all subfolders too! note: but when
            // referencing sprites in the Yarn script, just use the file
            // name and omit folder names

            if (useResourcesFolders)
            {

                List<Sprite> resourceSprites = new List<Sprite>();
                List<AudioClip> resourceAudio = new List<AudioClip>();

                if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/DialogueResources"))
                {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/DialogueResources");
                }

                string folderPath = Application.dataPath + "/StreamingAssets/DialogueResources";

                string[] Files = Directory.GetFiles(folderPath); //Getting Text files
                                                                 //List<TerrainLayer> layers = new List<TerrainLayer>();

                int i = 0;
                foreach (string file in Files)
                {
                    if (IsImage(file))
                    {
                        Byte[] pngBytes = System.IO.File.ReadAllBytes(file);
                        Texture2D tt = new Texture2D(52, 52);
                        tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                                               //tt.alphaIsTransparency = true;
                        tt.name = Path.GetFileName(file);

                        Rect rect = new Rect(0, 0, tt.width, tt.height);
                        Sprite sprite = Sprite.Create(tt, rect, new Vector2(0.5f, 0.5f));
                        sprite.name = Path.GetFileNameWithoutExtension(tt.name);
                        resourceSprites.Add(sprite);
                    }
                    else
                    if (IsAudio(file))
                    {
                        /*Byte[] pngBytes = System.IO.File.ReadAllBytes(file);
                        Texture2D tt = new Texture2D(52, 52);
                        tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                                               //tt.alphaIsTransparency = true;
                        tt.name = Path.GetFileName(file);*/

                        Debug.Log(file);
                    }
                    i++;
                }
                //EditorManager.Instance.SetTerrainTextures(layers.ToArray());
                //DeselectAll();


                //DialogueResources
                Sprite[] allSpritesInResources = resourceSprites.ToArray();
                loadSprites.AddRange(allSpritesInResources);
                AudioClip[] allAudioInResources = resourceAudio.ToArray();
                loadAudio.AddRange(allAudioInResources);

            }

            // manually add all Yarn command handlers, so that we don't
            // have to type out game object names in Yarn scripts (also
            // gives us a performance increase by avoiding GameObject.Find)
            runner.AddCommandHandler<string, float>("Scene", DoSceneChange);
            runner.AddCommandHandler<string, string, string, string, string>("Act", SetActor);
            runner.AddCommandHandler<string, string, string>("Draw", SetSpriteYarn);

            runner.AddCommandHandler<string>("Hide", HideSprite);
            runner.AddCommandHandler("HideAll", HideAllSprites);
            runner.AddCommandHandler("Reset", ResetScene);

            runner.AddCommandHandler<string, string, string, float>("Move", MoveSprite);
            runner.AddCommandHandler<string, string>("Flip", FlipSprite);
            runner.AddCommandHandler<string, float>("Shake", ShakeSprite);

            runner.AddCommandHandler<string, float, string>("PlayAudio", PlayAudio);
            runner.AddCommandHandler<string>("PlayMusic", PlayMusic);
            runner.AddCommandHandler<string>("StopAudio", StopAudio);
            runner.AddCommandHandler("StopAudioAll", StopAudioAll);

            runner.AddCommandHandler<string, float, float, float>("Fade", SetFade);
            runner.AddCommandHandler<float>("FadeIn", SetFadeIn);
            runner.AddCommandHandler<string, string, float>("CamOffset", SetCameraOffset);

            runner.AddCommandHandler<int>("LookAt", SetCameraLook);

            // runner.onDialogueComplete.AddListener(DialogueComplete);

        }


        private bool IsImage(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
        }

        private bool IsAudio(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension == ".ogg" || extension == ".mp3" || extension == ".wav" || extension == ".flac";
        }

        private void Start()
        {
            if (onStartDialogue)
            {
                StartDialogue(scriptToLoadOnStart);
            }
        }
        public void StartDialogue(string startNode)
        {
            // Load the program, along with all of its nodes. 
            // The string table will be selected based on the 
            // Dialogue Runner's text language variable.
            /*AudioManager.Instance.PauseMusic();*/
            if (levelMenu == null)
                levelMenu = GameObject.FindGameObjectWithTag("levelMenu").GetComponent<LevelMenu>();
            if (levelMenu != null)
                levelMenu.Block();

            inDialogue = true;
            spriteGroup.gameObject.SetActive(true);
            bgImage.gameObject.SetActive(true);
            fadeBG.gameObject.SetActive(true);
            nameplateBG.gameObject.SetActive(true);
            genericSprite.gameObject.SetActive(true);
            runner.StartDialogue(startNode);
        }

        public override void DialogueComplete()
        {
            //runner.Stop();
            /*if (AudioManager.Instance.musicSet != null)
                AudioManager.Instance.SetMainMusicSet(AudioManager.Instance.musicSet);
*/
            if (levelMenu == null)
                levelMenu = GameObject.FindGameObjectWithTag("levelMenu").GetComponent<LevelMenu>();
            if (levelMenu != null)
                levelMenu.Resume();

            inDialogue = false;
            spriteGroup.gameObject.SetActive(false);
            bgImage.gameObject.SetActive(false);
            fadeBG.gameObject.SetActive(false);
            nameplateBG.gameObject.SetActive(false);
            genericSprite.gameObject.SetActive(false);

            if (moveCameraCoroutine != null) StopCoroutine(moveCameraCoroutine);
        }

        #region YarnCommands

        /// <summary>changes background image</summary>
        public void DoSceneChange(string spriteName, float alpha = 0.3f)
        {
            Color col = Color.yellow;
            if (spriteName != string.Empty && ColorUtility.TryParseHtmlString(spriteName, out col) == false)
            {
                bgImage.sprite = FetchAsset<Sprite>(spriteName);
                bgImage.color = new Color(1, 1, 1, alpha);
                return;
            }
            col.a = alpha;
            bgImage.sprite = FetchAsset<Sprite>("white");
            bgImage.color = col;
        }

        /// <summary>
        /// SetActor(actorName,spriteName,positionX,positionY,color) main
        /// function for moving / adjusting characters</summary>
        public void SetActor(string actorName, string spriteName, string positionX = "", string positionY = "", string colorHex = "")
        {
            // have to use SetSprite() because par[2] and par[3] might be
            // keywords (e.g. "left", "right")
            var newActor = SetSpriteUnity(spriteName, positionX, positionY);

            // define text label BG color
            var actorColor = Color.black;
            if (colorHex != string.Empty && ColorUtility.TryParseHtmlString(colorHex, out actorColor) == false)
            {
                Debug.LogErrorFormat(this, "VN Manager can't parse [{0}] as an HTML color (e.g. [#FFFFFF] or certain keywords like [white])", colorHex);
            }

            // if the actor is using a sprite already, then clone any
            // persisting data, and destroy it (just to be safe)
            if (actors.ContainsKey(actorName))
            {
                // if any missing position params, assume the actor
                // position should stay the same
                var newPos = newActor.rectTransform.anchoredPosition;
                if (positionX == string.Empty && positionY == string.Empty)
                { // missing 2 params, override both x and y
                    newPos = actors[actorName].rectTransform.anchoredPosition;
                }
                else if (positionY == string.Empty)
                { // missing 1 param, override y
                    newPos.y = actors[actorName].rectTransform.anchoredPosition.y;
                }
                // if any missing color params, then assume actor color
                // should stay the same
                if (colorHex == string.Empty)
                {
                    actorColor = actors[actorName].actorColor;
                }
                newActor.rectTransform.anchoredPosition = newPos;
                // clean-up
                Destroy(actors[actorName].gameObject);
                actors.Remove(actorName);
                actors.Remove(actorName);
            }

            // save actor data
            actors.Add(actorName, new VNActor(newActor, actorColor));
        }

        ///<summary> Draw(spriteName,positionX,positionY) generic function
        ///for sprite drawing</summary>
        public void SetSpriteYarn(string spriteName, string positionX = "", string positionY = "")
        {
            SetSpriteUnity(spriteName, positionX, positionY);
        }

        public Image SetSpriteUnity(string spriteName, string positionX = "", string positionY = "")
        {

            // position sprite
            var pos = new Vector2(0.5f, 0.5f);

            if (positionX != string.Empty)
            {
                pos.x = ConvertCoordinates(positionX);
            }

            if (positionY != string.Empty)
            {
                pos.y = ConvertCoordinates(positionY);
            }

            // actually instantiate and draw sprite now
            return SetSpriteActual(spriteName, pos);
        }

        ///<summary>Hide(spriteName). "spriteName" can use wildcards, e.g.
        ///HideSprite(Sally*) will hide both SallyIdle and
        ///Sally_Happy</summary>
        public void HideSprite(string spriteName)
        {

            var wildcard = new Wildcard(spriteName);

            // generate lists of things to remove

            var imagesToDestroy = new List<Image>();
            var actorKeysToRemove = new List<string>();

            foreach (var actor in actors)
            {
                if (wildcard.IsMatch(actor.Key) || wildcard.IsMatch(actor.Value.actorImage.name))
                {
                    actorKeysToRemove.Add(actor.Key);
                    imagesToDestroy.Add(actor.Value.actorImage);
                }
            }

            foreach (var sprite in sprites)
            {
                if (wildcard.IsMatch(sprite.name))
                {
                    imagesToDestroy.Add(sprite);
                }
            }

            // actually remove all the things now, if any

            for (int i = 0; i < actorKeysToRemove.Count; i++)
            {
                if (actors.ContainsKey(actorKeysToRemove[i]))
                { // this should never be false, but let's be safe
                    actors.Remove(actorKeysToRemove[i]);
                }
            }

            for (int i = 0; i < imagesToDestroy.Count; i++)
            {
                if (imagesToDestroy[i] != null)
                { // this should never be false, but let's be safe
                    CleanDestroy<Image>(imagesToDestroy[i].gameObject);
                }
            }

        }

        /// <summary>HideAll doesn't actually use any parameters</summary>
        public void HideAllSprites()
        {
            HideSprite("*");
            actors.Clear();
            sprites.Clear();
        }

        /// <summary>Reset doesn't actually use any parameters</summary>
        public void ResetScene()
        {
            bgImage.sprite = null;
            HideAllSprites();
            SetFadeIn(0);
        }

        // move a sprite usage: <<Move actorOrspriteName, screenPosX=0.5,
        // screenPosY=0.5, moveTime=1.0>> screenPosX and screenPosY are
        // normalized screen coordinates (0.0 - 1.0) moveTime is the time
        // in seconds it will take to reach that position
        public void MoveSprite(string actorOrSpriteName, string screenPosX = "0.5", string screenPosY = "0.5", float moveTime = 1)
        {

            var image = FindActorOrSprite(actorOrSpriteName);

            // get new screen position
            Vector2 newPos = new Vector2(0.5f, 0.5f);
            if (screenPosX != string.Empty && screenPosY != string.Empty)
            {
                newPos = new Vector2(ConvertCoordinates(screenPosX), ConvertCoordinates(screenPosY));
            }
            else if (screenPosX != string.Empty)
            {
                newPos.x = ConvertCoordinates(screenPosX);
            }

            // actually do the moving now
            StartCoroutine(MoveCoroutine(image.GetComponent<RectTransform>(), Vector2.Scale(newPos, screenSize), moveTime));
        }

        /// <summary>flip a sprite, or force the sprite to face a
        /// direction< Move(actorOrSpriteName, xDirection=toggle)</sprite>
        public void FlipSprite(string actorOrSpriteName, string xDirection = "")
        {

            var image = FindActorOrSprite(actorOrSpriteName);


            float direction;

            if (xDirection != string.Empty)
            {
                direction = Mathf.Sign(ConvertCoordinates(xDirection) - 0.5f);
            }
            else
            {
                direction = Mathf.Sign(image.rectTransform.localScale.x) * -1f;
            }

            image.rectTransform.localScale = new Vector3(
                direction * Mathf.Abs(image.rectTransform.localScale.x),
                image.rectTransform.localScale.y,
                image.rectTransform.localScale.z
            );
        }

        /// <summary>Shake(actorName or spriteName, strength=0.5)</summary>
        public void ShakeSprite(string actorOrSpriteName, float shakeStrength = 0.5f)
        {

            var findShakeTarget = FindActorOrSprite(actorOrSpriteName);
            if (findShakeTarget != null)
            {
                StartCoroutine(SetShake(findShakeTarget.rectTransform, shakeStrength));
            }
        }

        /// <summary>PlayAudio( soundName,volume,"loop" )...
        /// PlayAudio(soundName,1.0) plays soundName once at 100% volume...
        /// if third parameter was word "loop" it would loop "volume" is a
        /// number from 0.0 to 1.0 "loop" is the word "loop" (or "true"),
        /// which tells the sound to loop over and over</summary>
        public void PlayMusic(string soundName)
        {
            /*AudioManager.Instance.StopCycle();
            AudioManager.Instance.SetMainMusic(soundName);*/
        }
        public void PlayAudio(string soundName, float volume = 1, string loop = "")
        {

            var audioClip = FetchAsset<AudioClip>(soundName);
            // detect volume setting

            if (volume <= 0.01f)
            {
                Debug.LogWarningFormat(this, "VN Manager is playing sound {0} at very low volume ({1}), just so you know", soundName, volume);
            }

            // detect loop setting
            bool shouldLoop = loop.Contains("loop") || loop.Contains("true");

            // instantiate AudioSource and configure it (don't use
            // AudioSource.PlayOneShot because we also want the option to
            // use <<StopAudio>> and interrupt it)
            var newAudioSource = Instantiate<AudioSource>(genericAudioSource, genericAudioSource.transform.parent);
            newAudioSource.name = audioClip.name;
            newAudioSource.clip = audioClip;
            newAudioSource.volume *= volume;
            newAudioSource.loop = shouldLoop;
            newAudioSource.outputAudioMixerGroup = audioGroup;
            newAudioSource.Play();
            sounds.Add(newAudioSource);

            // if it doesn't loop, let's set a max lifetime for this sound
            if (shouldLoop == false)
            {
                StartCoroutine(SetDestroyTime(newAudioSource, audioClip.length));
            }
        }

        /// <summary>stops sound playback based on sound name, whether it's
        /// looping or not</summary>
        public void StopAudio(string soundName)
        {

            // let's just do this in a sloppy way for now, and also assume
            // there's only one object like it
            AudioSource toDestroy = null;
            foreach (var audioObject in sounds)
            {
                if (audioObject.name == soundName)
                {
                    toDestroy = audioObject;
                    break;
                }
            }

            // double-check there's any audioSource to destroy tho, because
            // it might have been destroyed already
            if (toDestroy != null)
            {
                CleanDestroy<AudioSource>(toDestroy.gameObject);
            }
            else
            {
                Debug.LogWarningFormat(this, "VN Manager tried to <<StopAudio {0}>> but couldn't find any sound \"{0}\" currently playing. Double-check the name, or maybe it already stopped.", soundName);
            }
        }

        /// <summary>stops all currently playing sounds, doesn't actually
        /// take any parameters</summary>
        public void StopAudioAll()
        {
            var toStop = new List<AudioSource>();
            foreach (var audioSrc in sounds)
            {
                toStop.Add(audioSrc);
            }
            foreach (var stopThis in toStop)
            {
                StopAudio(stopThis.name);
            }
        }

        /// <summary>typical screen fade effect, good for transitions?
        /// usage: Fade( #hexcolor, startAlpha, endAlpha, fadeTime
        /// )</summary>
        Coroutine fade;
        public void SetFade(string fadeColorHex, float startAlpha = 0, float endAlpha = 1, float fadeTime = 1)
        {
            // grab the color
            if (fade != null) StopCoroutine(fade);
            if (ColorUtility.TryParseHtmlString(fadeColorHex, out var fadeColor) == false)
            {
                Debug.LogErrorFormat(this, "VN Manager <<Fade>> couldn't parse [{0}] as an HTML hex color... it should look like [#FFFFFF] or [##FFCC00FF], or a small number of keywords work too, like [black] or [red]", fadeColorHex);
                fadeColor = Color.magenta;
            }

            // do the fade
            fade = StartCoroutine(FadeCoroutine(fadeColor, startAlpha, endAlpha, fadeTime));
        }

        /// <summary>convenient for an easy fade in, no matter what the
        /// previous fade color or alpha was</summary>
        public void SetFadeIn(float fadeTime = 1)
        {
            if (fade != null) StopCoroutine(fade);
            // do the fade in
            fade = StartCoroutine(FadeCoroutine(fadeBG.color, -1f, 0f, fadeTime));
        }

        /// <summary>pan the camera. Usage: CameraOffset(xPos, yPos,
        /// moveTime)</summary>
        /// 0, 0 is center default
        public void SetCameraOffset(string xPos = "", string yPos = "", float moveTime = 0.25f)
        {

            Vector2 newOffset = Vector2.zero;
            if (xPos != string.Empty && yPos != string.Empty)
            {
                newOffset = new Vector2(ConvertCoordinates(xPos) - 0.5f, ConvertCoordinates(xPos) - 0.5f);
            }
            else if (xPos != string.Empty)
            {
                newOffset.x = ConvertCoordinates(xPos) - 0.5f;
            }

            // because we're using UI overlays, there's no actual "camera"
            // exactly so we do a fake camera scroll by moving the
            // "Sprites" game object container
            var parent = genericSprite.transform.parent.GetComponent<RectTransform>();
            var newPos = Vector2.Scale(new Vector2(0.5f, 0.5f) - newOffset, screenSize);
            StartCoroutine(MoveCoroutine(parent, newPos, moveTime));
        }

        public void SetCameraLook(int towerID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            GameObject[] towers = GameObject.FindGameObjectsWithTag("building");


            foreach (GameObject tower in towers)
            {
                if (tower.GetComponent<BuildingMain>().id == towerID)
                {
                    Debug.Log(tower.transform.position);
                    if (moveCameraCoroutine != null) StopCoroutine(moveCameraCoroutine);
                    moveCameraCoroutine = StartCoroutine(MoveCamera(tower.transform.position, player.transform));
                    return;
                }
            }
            Debug.Log("TOWER WITH ID:: " + towerID + " NOT FOUND");
        }

        IEnumerator MoveCamera(Vector3 pos, Transform player)
        {
            pos = new Vector3(pos.x, player.position.y, pos.z - 5);
            while (true)
            {
                pos = new Vector3(pos.x, player.position.y, pos.z);
                player.position = Vector3.Lerp(player.position, pos, Time.deltaTime * 9);
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion



        #region Utility

        public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
        {
            var actorName = dialogueLine.CharacterName;
            if (string.IsNullOrEmpty(actorName) == false && actors.ContainsKey(actorName))
            {
                HighlightSprite(actors[actorName].actorImage);
                if (nameplateBG.color != actors[actorName].actorColor)
                {
                    nameplateBG.gameObject.SetActive(false);
                }
                nameplateBG.color = actors[actorName].actorColor;

                nameplateBG.gameObject.SetActive(true);
                /*Debug.Log(nameplateText);
                nameplateText.text = actorName;*/
            }
            else
            {
                nameplateBG.gameObject.SetActive(false);
            }

            onDialogueLineFinished();
        }

        public void HighlightSprite(Image sprite)
        {
            StopCoroutine("HighlightSpriteCoroutine"); // use StartCoroutine(string) overload so that we can Stop and Start the coroutine (it doesn't work otherwise?)
            StartCoroutine("HighlightSpriteCoroutine", sprite);
        }

        // called by HighlightSprite
        IEnumerator HighlightSpriteCoroutine(Image highlightedSprite)
        {
            float t = 0f;
            // over time, gradually change sprites to be "normal" or
            // "highlighted"
            while (t < 1f)
            {
                t += Time.deltaTime / 2f;
                foreach (var spr in sprites)
                {
                    Vector3 regularScalePreserveXFlip = new Vector3(Mathf.Sign(spr.transform.localScale.x), 1f, 1f);
                    if (spr != highlightedSprite)
                    { // set back to normal
                        spr.transform.localScale = Vector3.MoveTowards(spr.transform.localScale, regularScalePreserveXFlip, Time.deltaTime);
                        spr.color = Color.Lerp(spr.color, defaultTint, Time.deltaTime * 5f);
                    }
                    else
                    { // a little bit bigger / brighter
                        spr.transform.localScale = Vector3.MoveTowards(spr.transform.localScale, regularScalePreserveXFlip * 1.05f, Time.deltaTime);
                        spr.color = Color.Lerp(spr.color, highlightTint, Time.deltaTime * 5f);
                        spr.transform.SetAsLastSibling();
                    }
                }
                yield return 0;
            }
        }

        IEnumerator MoveCoroutine(RectTransform transform, Vector2 newAnchorPos, float moveTime)
        {
            Vector2 startPos = transform.anchoredPosition;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.001f, moveTime); // Math.Max to prevent divide by zero error
                transform.anchoredPosition = Vector2.Lerp(startPos, newAnchorPos, t);
                yield return 0;
            }
        }

        IEnumerator FadeCoroutine(Color fadeColor, float startAlpha, float endAlpha, float fadeTime)
        {
            Color startColor = fadeColor;
            if (startAlpha >= 0f)
            { // if startAlpha is -1f, that means just use whatever's there
                startColor.a = startAlpha;
            }
            else
            {
                startColor = fadeBG.color;
            }
            fadeColor.a = endAlpha;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.001f, fadeTime); // Math.Max to prevent divide by zero error
                fadeBG.color = Color.Lerp(startColor, fadeColor, t);
                yield return 0;
            }
        }

        Image SetSpriteActual(string spriteName, Vector2 position)
        {
            var newSpriteObject = Instantiate<Image>(genericSprite, genericSprite.transform.parent);
            sprites.Add(newSpriteObject);
            newSpriteObject.name = spriteName;
            newSpriteObject.sprite = FetchAsset<Sprite>(spriteName);
            newSpriteObject.SetNativeSize();
            newSpriteObject.rectTransform.anchoredPosition = Vector2.Scale(position, screenSize);
            return newSpriteObject;
        }

        // TODO: change to Image[] and grab all valid results?
        Image FindActorOrSprite(string actorOrSpriteName)
        {
            if (actors.ContainsKey(actorOrSpriteName))
            {
                return actors[actorOrSpriteName].actorImage;
            }
            else
            { // or is it a generic sprite?
                foreach (var sprite in sprites)
                { // lazy sprite name search
                    if (sprite.name == actorOrSpriteName)
                    {
                        return sprite;
                    }
                }
                Debug.LogErrorFormat(this, "VN Manager couldn't find an actor or sprite with name \"{0}\", maybe it was misspelled or the sprite was hidden / destroyed already", actorOrSpriteName);
                return null;
            }
        }

        // shakes a RectTransform (usually sprites)
        IEnumerator SetShake(RectTransform thingToShake, float shakeStrength = 0.5f)
        {
            var startPos = thingToShake.anchoredPosition;
            while (shakeStrength > 0f)
            {
                shakeStrength -= Time.deltaTime;
                float shakeDistance = Mathf.Clamp(shakeStrength * 69f, 0f, 69f);
                float shakeFrequency = Mathf.Clamp(shakeStrength * 5f, 0f, 5f);
                thingToShake.anchoredPosition = startPos + shakeDistance * new Vector2(Mathf.Sin(Time.time * shakeFrequency), Mathf.Sin(Time.time * shakeFrequency + 17f) * 0.62f);
                yield return 0;
            }
            thingToShake.anchoredPosition = startPos;
        }

        // timed destroy... can't use Destroy( gameObject, timeDelay )
        // because it might get destroyed earlier via <<StopAudio>> or
        // something, and we want to remove the reference from the list too
        IEnumerator SetDestroyTime(AudioSource destroyThis, float timeDelay)
        {
            float timer = timeDelay;
            while (timer > 0f)
            {
                if (destroyThis == null) { break; } // it could've been destroyed already, so let's just make sure
                if (destroyThis.isPlaying)
                {
                    timer -= Time.deltaTime;
                }
                yield return 0;
            }
            if (destroyThis != null)
            { // it could've been destroyed already, so let's just make sure
                CleanDestroy<AudioSource>(destroyThis.gameObject);
            }
        }


        // CleanDestroy also removes any references to the gameObject from
        // sprites or sounds
        void CleanDestroy<T>(GameObject destroyThis)
        {
            if (typeof(T) == typeof(AudioSource))
            {
                sounds.Remove(destroyThis.GetComponent<AudioSource>());
            }
            else if (typeof(T) == typeof(Image))
            {
                sprites.Remove(destroyThis.GetComponent<Image>());
            }

            Destroy(destroyThis);
        }

        // utility function to convert words like "left" or "right" into
        // equivalent position numbers
        float ConvertCoordinates(string coordinate)
        {
            // first, is anyone named after this coordinate? we'll use the
            // X position
            if (actors.ContainsKey(coordinate))
            {
                return actors[coordinate].rectTransform.anchoredPosition.x / screenSize.x;
            }

            // next, let's see if they used a position keyword
            var labelCoordinate = coordinate.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");
            switch (labelCoordinate)
            {
                case "leftedge":
                case "bottomedge":
                case "loweredge":
                    return 0f;
                case "left":
                case "bottom":
                case "lower":
                    return 0.25f;
                case "center":
                case "middle":
                    return 0.5f;
                case "right":
                case "top":
                case "upper":
                    return 0.75f;
                case "rightedge":
                case "topedge":
                case "upperedge":
                    return 1f;
                case "offleft":
                    return -0.33f;
                case "offright":
                    return 1.33f;
            }

            // if none of those worked, then let's try parsing it as a
            // number
            float x;
            if (float.TryParse(coordinate, out x))
            {
                return x;
            }
            else
            {
                Debug.LogErrorFormat(this, "VN Manager couldn't convert position [{0}]... it must be an alignment (left, center, right, or top, middle, bottom) or a value (like 0.42 as 42%)", coordinate);
                return -1f;
            }

        }

        // utility function to find an asset, whether it's in \Resources\
        // or manually loaded via an array
        T FetchAsset<T>(string assetName) where T : UnityEngine.Object
        {
            // first, check to see if it's a manully loaded asset, with
            // manual array checks... it's messy but I can't think of a
            // better way to do this
            if (typeof(T) == typeof(Sprite))
            {
                foreach (var spr in loadSprites)
                {
                    if (spr.name == assetName)
                    {
                        return spr as T;
                    }
                }
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                foreach (var ac in loadAudio)
                {
                    if (ac.name == assetName)
                    {
                        return ac as T;
                    }
                }
            }

            // by default, we load all Resources assets into the asset
            // arrays already, but if you don't want that, then uncomment
            // this, etc. if ( useResourcesFolders ) {var newAsset =
            // Resources.Load<T>(assetName); if ( newAsset != null )
            // {return newAsset;
            //  }
            // }

            Debug.LogErrorFormat(this, "VN Manager can't find asset [{0}]... maybe it is misspelled, or isn't imported as {1}?", assetName, typeof(T).ToString());
            return null; // didn't find any matching asset
        }


        #endregion
    } // end class

    /// <summary>
    /// stores data for actors (sprite reference and color), can be
    /// expanded if necessary
    /// </summary>
    [System.Serializable]
    public class VNActor
    {
        public Image actorImage;
        public Color actorColor;
        public RectTransform rectTransform { get { return actorImage.rectTransform; } }
        public GameObject gameObject { get { return actorImage.gameObject; } }

        public VNActor(Image actorImage, Color actorColor)
        {
            this.actorImage = actorImage;
            this.actorColor = actorColor;
        }
    }

    // from
    // https://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes
    // by Rei Miyasaka
    class Wildcard : Regex
    {
        public Wildcard(string pattern) : base(WildcardToRegex(pattern)) { }

        public Wildcard(string pattern, RegexOptions options) : base(WildcardToRegex(pattern), options) { }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }
    }

} // end namespace


namespace Yarn.Markup
{



}


