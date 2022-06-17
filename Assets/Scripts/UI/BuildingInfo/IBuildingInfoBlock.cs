using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBuildingInfoBlock : MonoBehaviour
{
    public virtual void LoadBlockBeforeCanvasUpdate() {

    }

    public virtual void LoadBlockAfterCanvasUpdate()
    {

    }

    public virtual int GetHeightSize() {
        return 40;
    }

    public abstract BuildingInfoBlockType GetBuildingInfoBlockType();
}
