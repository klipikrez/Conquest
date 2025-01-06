using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTextWaveMeshEffect", menuName = "TexTeffects/Mesh/Wave")]
public class Wave : TextAnimation
{
    public AnimationCurve kurvaX;
    public AnimationCurve kurvaY;
    public float waveDensety = 10f;
    public float waveSpeed = 0.01f;
    public float waveHeight = 1f;
    public float jitterSpeed = 0.01f;
    public float JitterAngle = 10f;






    // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
    TMP_MeshInfo[] cachedMeshInfo;

    public override BOB Animate(int characterIndex)
    {
        BOB bob = new BOB();
        bob.rotation = Mathf.Sin(characterIndex + Time.unscaledTime * jitterSpeed) * JitterAngle;

        float x = kurvaX.Evaluate((Time.unscaledTime * waveSpeed + characterIndex * waveDensety) % 1) * waveHeight;
        float y = kurvaY.Evaluate((Time.unscaledTime * waveSpeed + characterIndex * waveDensety) % 1) * waveHeight;
        bob.translation = new Vector2(x, y);
        return bob;
    }

    public override void AnimateAll(int start, int lenth, TextMeshProUGUI text)
    {
        TMP_TextInfo textInfo = text.textInfo;
        cachedMeshInfo = textInfo.CopyMeshInfoVertexData();




        int characterCount = textInfo.characterCount;
        Matrix4x4 matrix;

        for (int i = start; i < /*characterCount*/ start + lenth; i++)
        {
            //int charIndex = start + ((loopCount) % lenth);

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!textInfo.characterInfo[i].isVisible)
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


            //Vector3 jitterOffset = new Vector3(/*Random.Range(-.25f, .25f)*/0, Mathf.Sin(i + loopCount * waveSpeed) /*Random.Range(-.25f, .25f)*/, 0);

            //matrix = Matrix4x4.TRS(jitterOffset * waveHeight, Quaternion.Euler(0, 0, Mathf.Sin(i + loopCount * jitterSpeed) * JitterAngle/*Random.Range(-5f, 5f) * AngleMultiplier*/), Vector3.one);


            // This is where the derived class sets translation/rotation/scale
            BOB bob = this.Animate(i);

            Vector2 translation = bob.translation;
            float rotation = bob.rotation, scale = bob.scale;
            matrix = Matrix4x4.TRS(translation, Quaternion.Euler(0f, 0f, rotation), scale * Vector3.one);

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
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
        if (text.transform.childCount != 0)
        {
            //ovo izgleda mnogo smotano, al bez njega ne radi :(((((
            GameObject child = text.transform.GetChild(0).gameObject;
            child.SetActive(false);
            child.SetActive(true);
        }
    }
    /*
        public override IEnumerator CorutineText(int start, int lenth, TextMeshProUGUI text)
        {
            // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
            // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.

            while (true)
            {
                AnimateAll(start, lenth, text);



                yield return new WaitForSeconds(0.03f);
                //yield return new WaitForEndOfFrame();
            }
        }*/
}