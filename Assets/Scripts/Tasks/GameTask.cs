using System.Collections;
using System.Collections.Generic;

public abstract class GameTask
{
    public ETask TaskType = ETask.Error;
    public int Index;
    public bool IsMain;

    public virtual bool IsTaskCompleted() {
        return false;
    }

    public virtual bool IsTaskFailed()
    {
        return false;
    }

    public virtual string GetTaskString() {
        return "";
    }

    public virtual string GetTaskInMenuString()
    {
        return "";
    }
}
