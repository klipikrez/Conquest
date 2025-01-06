/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Yarn.Markup;
using System;
using TMPro;
public class DialogueView : DialogueViewBase
{
    public TMP_Text textCanvas;
    List<Coroutine> coroutines = new List<Coroutine>();
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        foreach (Coroutine c in coroutines)
        {
            StopCoroutine(c);
        }
        coroutines.Clear();

        textCanvas.text = dialogueLine.RawText;

        //markup readline
        List<MarkupAttribute> markups = dialogueLine.Text.Attributes;
        if (markups.Count != 0)
        {
            foreach (MarkupAttribute markup in markups)
            {
                if (markup.Length != 0)
                {
                    coroutines.Add(StartCoroutine(AnimateVertexColors(markup.Position, markup.Length + markup.Position)));
                }
            }

        }
        base.RunLine(dialogueLine, onDialogueLineFinished);
    }




    /// <summary>
    /// Method to animate vertex colors of a TMP Text object.
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateVertexColors(int start, int end)
    {
        // Force the text object to update right away so we can have geometry to modify right from the start.
        textCanvas.ForceMeshUpdate();

        TMP_TextInfo textInfo = textCanvas.textInfo;
        int currentCharacter = 0;


        Color32[] newVertexColors;
        Color32 c0 = textCanvas.color;

        while (true)
        {
            int charIndex = (start + currentCharacter + 1) % end;
            int characterCount = textInfo.characterCount;
            
                        // If No Characters then just yield and wait for some text to be added
                        //if (characterCount == 0)
                       // {
                       //     yield return new WaitForSeconds(0.25f);
                       //     continue;
                     //   }

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            // Only change the vertex color if the text element is visible.
            if (textInfo.characterInfo[charIndex].isVisible)
            {
                //c0 = new Color32((byte)(UnityEngine.Random.Range(-1, 20) + newVertexColors[vertexIndex + 0].r), (byte)(UnityEngine.Random.Range(-1, 20) + newVertexColors[vertexIndex + 0].g), (byte)(UnityEngine.Random.Range(-1, 20) + newVertexColors[vertexIndex + 0].b), 255);
                c0 = new Color32((byte)255, (byte)255, (byte)255, 255);
                Debug.Log("c0: " + c0);
                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                textCanvas.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
            }

            currentCharacter++;

            yield return new WaitForSeconds(0.01f);
        }
    }



}
*/

using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Markup;
using System.Collections.Generic;

/// <summary>
/// Contains coroutine methods that apply visual effects. This class is used
/// by <see cref="LineView"/> to handle animating the presentation of lines.
/// </summary>
public static class Effects
{
    /// <summary>
    /// An object that can be used to signal to a coroutine that it should
    /// terminate early.
    /// </summary>
    /// <remarks>
    /// <para>
    /// While coroutines can be stopped by calling <see
    /// cref="MonoBehaviour.StopCoroutine"/> or <see
    /// cref="MonoBehaviour.StopAllCoroutines"/>, this has the side effect
    /// of also stopping any coroutine that was waiting for the now-stopped
    /// coroutine to finish.
    /// </para>
    /// <para>
    /// Instances of this class may be passed as a parameter to a coroutine
    /// that they can periodically poll to see if they should terminate
    /// earlier than planned.
    /// </para>
    /// <para>
    /// To use this class, create an instance of it, and pass it as a
    /// parameter to your coroutine. In the coroutine, call <see
    /// cref="Start"/> to mark that the coroutine is running. During the
    /// coroutine's execution, periodically check the <see
    /// cref="WasInterrupted"/> property to determine if the coroutine
    /// should exit. If it is <see langword="true"/>, the coroutine should
    /// exit (via the <c>yield break</c> statement.) At the normal exit of
    /// your coroutine, call the <see cref="Complete"/> method to mark that the
    /// coroutine is no longer running. To make a coroutine stop, call the
    /// <see cref="Interrupt"/> method.
    /// </para>
    /// <para>
    /// You can also use the <see cref="CanInterrupt"/> property to
    /// determine if the token is in a state in which it can stop (that is,
    /// a coroutine that's using it is currently running.)
    /// </para>
    /// </remarks>
    public class CoroutineInterruptToken
    {

        /// <summary>
        /// The state that the token is in.
        /// </summary>
        enum State
        {
            NotRunning,
            Running,
            Interrupted,
        }
        private State state = State.NotRunning;

        public bool CanInterrupt => state == State.Running;
        public bool WasInterrupted => state == State.Interrupted;
        public void Start() => state = State.Running;
        public void Interrupt()
        {
            if (CanInterrupt == false)
            {
                throw new InvalidOperationException($"Cannot stop {nameof(CoroutineInterruptToken)}; state is {state} (and not {nameof(State.Running)}");
            }
            state = State.Interrupted;
        }

        public void Complete() => state = State.NotRunning;
    }

    /// <summary>
    /// A coroutine that fades a <see cref="CanvasGroup"/> object's opacity
    /// from <paramref name="from"/> to <paramref name="to"/> over the
    /// course of <see cref="fadeTime"/> seconds, and then invokes <paramref
    /// name="onComplete"/>.
    /// </summary>
    /// <param name="from">The opacity value to start fading from, ranging
    /// from 0 to 1.</param>
    /// <param name="to">The opacity value to end fading at, ranging from 0
    /// to 1.</param>
    /// <param name="stopToken">A <see cref="CoroutineInterruptToken"/> that
    /// can be used to interrupt the coroutine.</param>
    public static IEnumerator FadeAlpha(CanvasGroup canvasGroup, float from, float to, float fadeTime, CoroutineInterruptToken stopToken = null)
    {

        stopToken?.Start();

        canvasGroup.alpha = from;

        var timeElapsed = 0f;

        while (timeElapsed < fadeTime)
        {
            if (stopToken?.WasInterrupted ?? false)
            {
                yield break;
            }

            var fraction = timeElapsed / fadeTime;
            timeElapsed += Time.deltaTime;

            float a = Mathf.Lerp(from, to, fraction);

            canvasGroup.alpha = a;
            yield return null;
        }

        canvasGroup.alpha = to;

        // If our destination alpha is zero, disable interactibility,
        // because the canvas group is now invisible.
        if (to == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            // Otherwise, enable interactibility, because it's visible.
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        stopToken?.Complete();
    }

    /// <summary>
    /// A coroutine that gradually reveals the text in a <see
    /// cref="TextMeshProUGUI"/> object over time.
    /// </summary>
    /// <remarks>
    /// <para>This method works by adjusting the value of the <paramref name="text"/> parameter's <see cref="TextMeshProUGUI.maxVisibleCharacters"/> property. This means that word wrapping will not change half-way through the presentation of a word.</para>
    /// <para style="note">Depending on the value of <paramref name="lettersPerSecond"/>, <paramref name="onCharacterTyped"/> may be called multiple times per frame.</para>
    /// <para>Due to an internal implementation detail of TextMeshProUGUI, this method will always take at least one frame to execute, regardless of the length of the <paramref name="text"/> parameter's text.</para>
    /// </remarks>
    /// <param name="text">A TextMeshProUGUI object to reveal the text
    /// of.</param>
    /// <param name="lettersPerSecond">The number of letters that should be
    /// revealed per second.</param>
    /// <param name="onCharacterTyped">An <see cref="Action"/> that should be called for each character that was revealed.</param>
    /// <param name="stopToken">A <see cref="CoroutineInterruptToken"/> that
    /// can be used to interrupt the coroutine.</param>
    public static IEnumerator Typewriter(TextMeshProUGUI text, float lettersPerSecond, Action onCharacterTyped, CoroutineInterruptToken stopToken = null)
    {
        stopToken?.Start();

        // Start with everything invisible
        text.maxVisibleCharacters = 0;

        // Wait a single frame to let the text component process its
        // content, otherwise text.textInfo.characterCount won't be
        // accurate
        yield return null;

        // How many visible characters are present in the text?
        var characterCount = text.textInfo.characterCount;

        // Early out if letter speed is zero, text length is zero
        if (lettersPerSecond <= 0 || characterCount == 0)
        {
            // Show everything and return
            text.maxVisibleCharacters = characterCount;
            stopToken?.Complete();
            yield break;
        }

        // Convert 'letters per second' into its inverse
        float secondsPerLetter = 1.0f / lettersPerSecond;

        // If lettersPerSecond is larger than the average framerate, we
        // need to show more than one letter per frame, so simply
        // adding 1 letter every secondsPerLetter won't be good enough
        // (we'd cap out at 1 letter per frame, which could be slower
        // than the user requested.)
        //
        // Instead, we'll accumulate time every frame, and display as
        // many letters in that frame as we need to in order to achieve
        // the requested speed.
        var accumulator = Time.deltaTime;

        while (text.maxVisibleCharacters < characterCount)
        {
            if (stopToken?.WasInterrupted ?? false)
            {
                yield break;
            }

            // We need to show as many letters as we have accumulated
            // time for.
            while (accumulator >= secondsPerLetter)
            {
                text.maxVisibleCharacters += 1;
                onCharacterTyped?.Invoke();
                accumulator -= secondsPerLetter;
            }
            accumulator += Time.deltaTime;

            yield return null;
        }

        // We either finished displaying everything, or were
        // interrupted. Either way, display everything now.
        text.maxVisibleCharacters = characterCount;
        onCharacterTyped?.Invoke();
        stopToken?.Complete();


    }
}

/// <summary>
/// A Dialogue View that presents lines of dialogue, using Unity UI
/// elements.
/// </summary>
public class DialogueView : DialogueViewBase
{
    public TextMeshProUGUI dialogueText;
    public TextAnimation[] meshAnimationsArray;
    Dictionary<string, TextAnimation> meshAnimations = new Dictionary<string, TextAnimation>();
    List<Par> currentAnimations = new List<Par>();
    public class Par
    {
        public MarkupAttribute markup;
        public TextAnimation animation;
        public Par(TextAnimation animation, MarkupAttribute markup)
        {
            this.markup = markup;
            this.animation = animation;
        }
    }
    /// <summary>
    /// The canvas group that contains the UI elements used by this Line
    /// View.
    /// </summary>
    /// <remarks>
    /// If <see cref="useFadeEffect"/> is true, then the alpha value of this
    /// <see cref="CanvasGroup"/> will be animated during line presentation
    /// and dismissal.
    /// </remarks>
    /// <seealso cref="useFadeEffect"/>
    [SerializeField]
    internal CanvasGroup canvasGroup;

    /// <summary>
    /// Controls whether the line view should fade in when lines appear, and
    /// fade out when lines disappear.
    /// </summary>
    /// <remarks><para>If this value is <see langword="true"/>, the <see
    /// cref="canvasGroup"/> object's alpha property will animate from 0 to
    /// 1 over the course of <see cref="fadeInTime"/> seconds when lines
    /// appear, and animate from 1 to zero over the course of <see
    /// cref="fadeOutTime"/> seconds when lines disappear.</para>
    /// <para>If this value is <see langword="false"/>, the <see
    /// cref="canvasGroup"/> object will appear instantaneously.</para>
    /// </remarks>
    /// <seealso cref="canvasGroup"/>
    /// <seealso cref="fadeInTime"/>
    /// <seealso cref="fadeOutTime"/>
    [SerializeField]
    internal bool useFadeEffect = true;

    /// <summary>
    /// The time that the fade effect will take to fade lines in.
    /// </summary>
    /// <remarks>This value is only used when <see cref="useFadeEffect"/> is
    /// <see langword="true"/>.</remarks>
    /// <seealso cref="useFadeEffect"/>
    [SerializeField]
    [Min(0)]
    internal float fadeInTime = 0.25f;

    /// <summary>
    /// The time that the fade effect will take to fade lines out.
    /// </summary>
    /// <remarks>This value is only used when <see cref="useFadeEffect"/> is
    /// <see langword="true"/>.</remarks>
    /// <seealso cref="useFadeEffect"/>
    [SerializeField]
    [Min(0)]
    internal float fadeOutTime = 0.05f;

    /// <summary>
    /// The <see cref="TextMeshProUGUI"/> object that displays the text of
    /// dialogue lines.
    /// </summary>
    [SerializeField]
    internal TextMeshProUGUI lineText = null;

    /// <summary>
    /// Controls whether the <see cref="lineText"/> object will show the
    /// character name present in the line or not.
    /// </summary>
    /// <remarks>
    /// <para style="note">This value is only used if <see
    /// cref="characterNameText"/> is <see langword="null"/>.</para>
    /// <para>If this value is <see langword="true"/>, any character names
    /// present in a line will be shown in the <see cref="lineText"/>
    /// object.</para>
    /// <para>If this value is <see langword="false"/>, character names will
    /// not be shown in the <see cref="lineText"/> object.</para>
    /// </remarks>
    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("showCharacterName")]
    internal bool showCharacterNameInLineView = true;

    /// <summary>
    /// The <see cref="TextMeshProUGUI"/> object that displays the character
    /// names found in dialogue lines.
    /// </summary>
    /// <remarks>
    /// If the <see cref="LineView"/> receives a line that does not contain
    /// a character name, this object will be left blank.
    /// </remarks>
    [SerializeField]
    internal TextMeshProUGUI characterNameText = null;

    /// <summary>
    /// Controls whether the text of <see cref="lineText"/> should be
    /// gradually revealed over time.
    /// </summary>
    /// <remarks><para>If this value is <see langword="true"/>, the <see
    /// cref="lineText"/> object's <see
    /// cref="TMP_Text.maxVisibleCharacters"/> property will animate from 0
    /// to the length of the text, at a rate of <see
    /// cref="typewriterEffectSpeed"/> letters per second when the line
    /// appears. <see cref="onCharacterTyped"/> is called for every new
    /// character that is revealed.</para>
    /// <para>If this value is <see langword="false"/>, the <see
    /// cref="lineText"/> will all be revealed at the same time.</para>
    /// <para style="note">If <see cref="useFadeEffect"/> is <see
    /// langword="true"/>, the typewriter effect will run after the fade-in
    /// is complete.</para>
    /// </remarks>
    /// <seealso cref="lineText"/>
    /// <seealso cref="onCharacterTyped"/>
    /// <seealso cref="typewriterEffectSpeed"/>
    [SerializeField]
    internal bool useTypewriterEffect = false;

    /// <summary>
    /// A Unity Event that is called each time a character is revealed
    /// during a typewriter effect.
    /// </summary>
    /// <remarks>
    /// This event is only invoked when <see cref="useTypewriterEffect"/> is
    /// <see langword="true"/>.
    /// </remarks>
    /// <seealso cref="useTypewriterEffect"/>
    [SerializeField]
    internal UnityEngine.Events.UnityEvent onCharacterTyped;

    /// <summary>
    /// The number of characters per second that should appear during a
    /// typewriter effect.
    /// </summary>
    /// <seealso cref="useTypewriterEffect"/>
    [SerializeField]
    [Min(0)]
    internal float typewriterEffectSpeed = 0f;

    /// <summary>
    /// The game object that represents an on-screen button that the user
    /// can click to continue to the next piece of dialogue.
    /// </summary>
    /// <remarks>
    /// <para>This game object will be made inactive when a line begins
    /// appearing, and active when the line has finished appearing.</para>
    /// <para>
    /// This field will generally refer to an object that has a <see
    /// cref="Button"/> component on it that, when clicked, calls <see
    /// cref="OnContinueClicked"/>. However, if your game requires specific
    /// UI needs, you can provide any object you need.</para>
    /// </remarks>
    /// <seealso cref="autoAdvance"/>
    [SerializeField]
    internal GameObject continueButton = null;

    /// <summary>
    /// The amount of time to wait after any line
    /// </summary>
    [SerializeField]
    [Min(0)]
    internal float holdTime = 1f;

    /// <summary>
    /// Controls whether this Line View will wait for user input before
    /// indicating that it has finished presenting a line.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this value is true, the Line View will not report that it has
    /// finished presenting its lines. Instead, it will wait until the <see
    /// cref="UserRequestedViewAdvancement"/> method is called.
    /// </para>
    /// <para style="note"><para>The <see cref="DialogueRunner"/> will not
    /// proceed to the next piece of content (e.g. the next line, or the
    /// next options) until all Dialogue Views have reported that they have
    /// finished presenting their lines. If a <see cref="LineView"/> doesn't
    /// report that it's finished until it receives input, the <see
    /// cref="DialogueRunner"/> will end up pausing.</para>
    /// <para>
    /// This is useful for games in which you want the player to be able to
    /// read lines of dialogue at their own pace, and give them control over
    /// when to advance to the next line.</para></para>
    /// </remarks>
    [SerializeField]
    internal bool autoAdvance = false;

    /// <summary>
    /// The current <see cref="LocalizedLine"/> that this line view is
    /// displaying.
    /// </summary>
    LocalizedLine currentLine = null;

    /// <summary>
    /// A stop token that is used to interrupt the current animation.
    /// </summary>
    Effects.CoroutineInterruptToken currentStopToken = new Effects.CoroutineInterruptToken();
    private void Start()
    {
        onCharacterTyped.AddListener(UpdateAnimations);
        foreach (TextAnimation animation in meshAnimationsArray)
        {
            meshAnimations.Add(animation.animationName, animation);
        }
    }
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private void Reset()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    /// <inheritdoc/>
    public override void DismissLine(Action onDismissalComplete)
    {
        currentLine = null;

        StartCoroutine(DismissLineInternal(onDismissalComplete));
    }

    private IEnumerator DismissLineInternal(Action onDismissalComplete)
    {
        // disabling interaction temporarily while dismissing the line
        // we don't want people to interrupt a dismissal
        var interactable = canvasGroup.interactable;
        canvasGroup.interactable = false;

        // If we're using a fade effect, run it, and wait for it to finish.
        if (useFadeEffect)
        {
            yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeOutTime, currentStopToken));
            currentStopToken.Complete();
        }

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        // turning interaction back on, if it needs it
        canvasGroup.interactable = interactable;
        onDismissalComplete();
    }

    /// <inheritdoc/>
    public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
    {
        currentLine = dialogueLine;

        // Cancel all coroutines that we're currently running. This will
        // stop the RunLineInternal coroutine, if it's running.
        StopAllCoroutines();

        // for now we are going to just immediately show everything
        // later we will make it fade in
        lineText.gameObject.SetActive(true);
        canvasGroup.gameObject.SetActive(true);

        int length;

        if (characterNameText == null)
        {
            if (showCharacterNameInLineView)
            {
                lineText.text = dialogueLine.Text.Text;
                length = dialogueLine.Text.Text.Length;
            }
            else
            {
                lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                length = dialogueLine.TextWithoutCharacterName.Text.Length;
            }
        }
        else
        {
            characterNameText.text = dialogueLine.CharacterName == null ? dialogueLine.CharacterName : dialogueLine.CharacterName.Replace("_", " ");
            lineText.text = dialogueLine.TextWithoutCharacterName.Text;
            length = dialogueLine.TextWithoutCharacterName.Text.Length;
        }

        // Show the entire line's text immediately.
        lineText.maxVisibleCharacters = length;
        UpdateAnimations();
        // Make the canvas group fully visible immediately, too.
        canvasGroup.alpha = 1;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        onInterruptLineFinished();
    }
    /// <inheritdoc/>

    Coroutine animateCorutine;
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        // Stop any coroutines currently running on this line view (for
        // example, any other RunLine that might be running)
        StopAllCoroutines();
        currentAnimations.Clear();
        // Set the current line to the line we're running.
        dialogueText.ClearMesh();

        currentDialogueLine = dialogueLine;
        // Begin running the line as a coroutine.
        StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
        //markup readline
        List<MarkupAttribute> markups = dialogueLine.Text.Attributes;
        if (markups.Count != 0)
        {

            foreach (MarkupAttribute markup in markups)
            {
                if (markup.Length != 0)
                {
                    if (meshAnimations.ContainsKey(markup.Name))
                    {
                        Debug.Log(markup.Name + " " + (markup.Position - (dialogueLine.CharacterName == null ? 0 : dialogueLine.CharacterName.Length + 2)) + " " + markup.Length);
                        currentAnimations.Add(new Par(meshAnimations[markup.Name], markup));
                    }
                    /* switch (markup.Name)
                     {
                         case "wave":
                             coroutines.Add(StartCoroutine(Wave(markup.Position - dialogueLine.CharacterName.Length - 2, markup.Length,
                               markup.Properties.ContainsKey("waveSpeed") ? markup.Properties["waveSpeed"].FloatValue : 0.1f,
                               markup.Properties.ContainsKey("waveHeight") ? markup.Properties["waveHeight"].FloatValue : 10f,
                               markup.Properties.ContainsKey("jitterSpeed") ? markup.Properties["jitterSpeed"].FloatValue : 0.1f,
                               markup.Properties.ContainsKey("JitterAngle") ? markup.Properties["JitterAngle"].FloatValue : 10f)));
                             break;
                     }*/
                    /*if (markup.Name == "wave")
                    {
                        coroutines.Add(StartCoroutine(AnimateVertexColors(markup.Position - dialogueLine.CharacterName.Length - 2, markup.Length)));
                    }*/
                }
            }
            animateCorutine = StartCoroutine(Animate());

        }

    }

    IEnumerator Animate()
    {
        while (true)
        {
            UpdateAnimations();

            yield return new WaitForSeconds(0.03f);
        }
    }
    LocalizedLine currentDialogueLine;
    public void UpdateAnimations()
    {
        lineText.ForceMeshUpdate();
        if (currentAnimations.Count != 0)
            foreach (Par animation in currentAnimations)
            {
                animation.animation.AnimateAll(animation.markup.Position - (currentDialogueLine.CharacterName == null ? 0 : currentDialogueLine.CharacterName.Length + 2), animation.markup.Length, lineText);
            }
    }

    struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }
    IEnumerator Jitter(int start, int lenth, float jitterDistance = 0.016f, float JitterAngle = 10f)
    {

        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        lineText.ForceMeshUpdate();

        TMP_TextInfo textInfo = lineText.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = UnityEngine.Random.Range(10f, 25f);
            vertexAnim[i].speed = UnityEngine.Random.Range(1f, 3f);
        }

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while (true)
        {

            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible)
                    continue;

                // Retrieve the pre-computed animation data for the given character.
                VertexAnim vertAnim = vertexAnim[i];

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
                Vector3 jitterOffset = new Vector3(UnityEngine.Random.Range(-.25f, .25f), UnityEngine.Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * jitterDistance, Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f) * JitterAngle), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                vertexAnim[i] = vertAnim;
            }

            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                lineText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount += 1;

            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator Wave(int start, int lenth, float waveSpeed = 0.1f, float waveHeight = 10f, float jitterSpeed = 0.1f, float JitterAngle = 10f)
    {

        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        lineText.ForceMeshUpdate();

        TMP_TextInfo textInfo = lineText.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;
        /*
                    // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
                    VertexAnim[] vertexAnim = new VertexAnim[1024];
                    for (int i = 0; i < 1024; i++)
                    {
                        vertexAnim[i].angleRange = Random.Range(10f, 25f);
                        vertexAnim[i].speed = Random.Range(1f, 3f);
                    }
        */
        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        while (true)
        {


            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = start; i < /*characterCount*/ start + lenth; i++)
            {
                //int charIndex = start + ((loopCount) % lenth);
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible)
                    continue;

                // Retrieve the pre-computed animation data for the given character.
                //VertexAnim vertAnim = vertexAnim[i];

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                // vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
                Vector3 jitterOffset = new Vector3(/*Random.Range(-.25f, .25f)*/0, Mathf.Sin(i + loopCount * waveSpeed) /*Random.Range(-.25f, .25f)*/, 0);

                matrix = Matrix4x4.TRS(jitterOffset * waveHeight, Quaternion.Euler(0, 0, Mathf.Sin(i + loopCount * jitterSpeed) * JitterAngle/*Random.Range(-5f, 5f) * AngleMultiplier*/), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                //vertexAnim[i] = vertAnim;
            }

            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                lineText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount += 1;
            yield return new WaitForSeconds(0.01f);
            //yield return new WaitForEndOfFrame();
        }

    }
    IEnumerator AnimateVertexColors(int start, int lenth)
    {
        // Force the text object to update right away so we can have geometry to modify right from the start.
        lineText.ForceMeshUpdate();

        TMP_TextInfo textInfo = lineText.textInfo;
        int currentCharacter = 0;


        Color32[] newVertexColors;
        Color32 c0 = lineText.color;

        while (true)
        {
            int charIndex = start + ((currentCharacter) % lenth);
            //int charIndex = (currentCharacter) % textInfo.characterCount;
            int characterCount = textInfo.characterCount;
            /*
                        // If No Characters then just yield and wait for some text to be added
                        if (characterCount == 0)
                        {
                            yield return new WaitForSeconds(0.25f);
                            continue;
                        }*/
            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            // Only change the vertex color if the text element is visible.
            if (textInfo.characterInfo[charIndex].isVisible)
            {
                //c0 = new Color32((byte)(UnityEngine.Random.Range(-1, 20) + newVertexColors[vertexIndex + 0].r), (byte)(UnityEngine.Random.Range(-1, 20) + newVertexColors[vertexIndex + 0].g), (byte)(UnityEngine.Random.Range(-1, 20) + newVertexColors[vertexIndex + 0].b), 255);
                c0 = new Color32((byte)255, (byte)((int)newVertexColors[vertexIndex + 0].g + 100), (byte)255, 255);

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                lineText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
            }

            currentCharacter++;

            yield return new WaitForSeconds(0.01f);

        }
    }

    private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        IEnumerator PresentLine()
        {
            lineText.gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(true);
            // Hide the continue button until presentation is complete (if
            // we have one).
            if (continueButton != null)
            {
                continueButton.SetActive(false);
            }

            if (characterNameText != null)
            {
                // If we have a character name text view, show the character
                // name in it, and show the rest of the text in our main
                // text view.
                characterNameText.text = dialogueLine.CharacterName == null ? dialogueLine.CharacterName : dialogueLine.CharacterName.Replace("_", " ");
                lineText.text = dialogueLine.TextWithoutCharacterName.Text;
            }
            else
            {
                // We don't have a character name text view. Should we show
                // the character name in the main text view?
                if (showCharacterNameInLineView)
                {
                    // Yep! Show the entire text.
                    lineText.text = dialogueLine.Text.Text;
                }
                else
                {
                    // Nope! Show just the text without the character name.
                    lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                }
            }

            if (useTypewriterEffect)
            {
                // If we're using the typewriter effect, hide all of the
                // text before we begin any possible fade (so we don't fade
                // in on visible text).
                lineText.maxVisibleCharacters = 0;
            }
            else
            {
                // Ensure that the max visible characters is effectively
                // unlimited.
                lineText.maxVisibleCharacters = int.MaxValue;
            }

            // If we're using the fade effect, start it, and wait for it to
            // finish.
            if (useFadeEffect)
            {
                yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeInTime, currentStopToken));
                if (currentStopToken.WasInterrupted)
                {
                    // The fade effect was interrupted. Stop this entire
                    // coroutine.
                    yield break;
                }
            }

            // If we're using the typewriter effect, start it, and wait for
            // it to finish.
            if (useTypewriterEffect)
            {
                // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                yield return StartCoroutine(
                    Effects.Typewriter(
                        lineText,
                        typewriterEffectSpeed,
                        () => onCharacterTyped.Invoke(),
                        currentStopToken
                    )
                );

                if (currentStopToken.WasInterrupted)
                {
                    // The typewriter effect was interrupted. Stop this
                    // entire coroutine.
                    yield break;
                }
            }
        }
        currentLine = dialogueLine;

        // Run any presentations as a single coroutine. If this is stopped,
        // which UserRequestedViewAdvancement can do, then we will stop all
        // of the animations at once.
        yield return StartCoroutine(PresentLine());

        currentStopToken.Complete();

        // All of our text should now be visible.
        lineText.maxVisibleCharacters = int.MaxValue;
        UpdateAnimations();
        // Our view should at be at full opacity.
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Show the continue button, if we have one.
        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }

        // If we have a hold time, wait that amount of time, and then
        // continue.
        if (holdTime > 0)
        {
            yield return new WaitForSeconds(holdTime);
        }

        if (autoAdvance == false)
        {
            // The line is now fully visible, and we've been asked to not
            // auto-advance to the next line. Stop here, and don't call the
            // completion handler - we'll wait for a call to
            // UserRequestedViewAdvancement, which will interrupt this
            // coroutine.
            yield break;
        }

        // Our presentation is complete; call the completion handler.
        onDialogueLineFinished();
    }

    /// <inheritdoc/>
    public override void UserRequestedViewAdvancement()
    {
        // We received a request to advance the view. If we're in the middle of
        // an animation, skip to the end of it. If we're not current in an
        // animation, interrupt the line so we can skip to the next one.

        // we have no line, so the user just mashed randomly
        if (currentLine == null)
        {
            return;
        }

        // we may want to change this later so the interrupted
        // animation coroutine is what actually interrupts
        // for now this is fine.
        // Is an animation running that we can stop?
        if (currentStopToken.CanInterrupt)
        {
            // Stop the current animation, and skip to the end of whatever
            // started it.
            currentStopToken.Interrupt();
        }
        // No animation is now running. Signal that we want to
        // interrupt the line instead.
        requestInterrupt?.Invoke();
    }

    /// <summary>
    /// Called when the <see cref="continueButton"/> is clicked.
    /// </summary>
    public void OnContinueClicked()
    {
        // When the Continue button is clicked, we'll do the same thing as
        // if we'd received a signal from any other part of the game (for
        // example, if a DialogueAdvanceInput had signalled us.)
        UserRequestedViewAdvancement();
    }
}

