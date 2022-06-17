using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_InfectedPercent : GameTask
{
    public EResource ResourceType;
    public TaskCountComparison TaskCountComparison;
    public int TargetPecent;
    private float PercentNow;
    private string PercentStr;

    public Task_InfectedPercent(TaskCountComparison taskCountComparison, int targetPecent)
    {
        TaskType = ETask.InfectedPercent;
        TaskCountComparison = taskCountComparison;
        TargetPecent = targetPecent;
    }

    public override bool IsTaskCompleted()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return PercentNow <= TargetPecent && InfectionManager.InfectionEnemyCount <= 0;
            case TaskCountComparison.MoreOrEqualTo:
                return PercentNow > TargetPecent;
        }
        return false;
    }

    public override bool IsTaskFailed()
    {
        return false;
    }

    public void SetPercent(float percent)
    {
        PercentNow = percent;
        PercentStr = percent.ToString("0.0");
    }

    public override string GetTaskString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                if (TargetPecent <= 0)
                {
                    return $"No infected people left (now {PercentStr}%)";
                }
                return $"No more than {TargetPecent}% people left (now {PercentStr}%)";
            case TaskCountComparison.MoreOrEqualTo:
                return $"At least {TargetPecent}% of people infected (now {PercentStr}%)";
        }
        return $"No infected people left (now {PercentStr}%)";
    }

    public override string GetTaskInMenuString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                if (TargetPecent <= 0)
                {
                    return $"No infected people left";
                }
                return $"No more than {TargetPecent}% people left";
            case TaskCountComparison.MoreOrEqualTo:
                return $"At least {TargetPecent}% of people infected";
        }
        return $"No infected people left";
    }
}