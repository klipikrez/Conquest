using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorBehaviour
{
    public abstract void ChangedEditorMode(EditorManager editor);
    public abstract void EditorUpdate(EditorManager editor);


}
