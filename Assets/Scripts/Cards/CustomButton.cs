using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomButton : MonoBehaviour
{
    public virtual float GetPressedTime() { 
        return 0.2f; 
    }

    public virtual void OnHover() { 
    
    }

    public virtual void OutHover() { 
    
    }

    public virtual void OnClicked(int mouseButton) { 
    
    }

    public virtual void OnHold(int mouseButton) { 
    
    }

    public virtual void OnRelease(int mouseButton) { 
    
    }
}
