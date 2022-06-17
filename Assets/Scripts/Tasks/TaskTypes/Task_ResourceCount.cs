using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_ResourceCount : GameTask
{
    public EResource ResourceType;
    public TaskCountComparison TaskCountComparison;
    public int TargetCount;
    public int CountNow;

    public Task_ResourceCount(EResource resourceType, TaskCountComparison taskCountComparison, int targetCount) {
        TaskType = ETask.ResourceCount;
        ResourceType = resourceType;
        TaskCountComparison = taskCountComparison;
        TargetCount = targetCount;
    }

    public override bool IsTaskCompleted()
    {
        switch (TaskCountComparison) {
            case TaskCountComparison.LessThan:
                return CountNow < TargetCount;
            case TaskCountComparison.MoreOrEqualTo:
                return CountNow >= TargetCount;
        }
        return false;
    }

    public override bool IsTaskFailed()
    {
        return false;
    }

    public override string GetTaskString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Less than {TargetCount} {ResourceManager.GetResourceName(ResourceType)} (now {CountNow}/{TargetCount})";
            case TaskCountComparison.MoreOrEqualTo:
                return $"{TargetCount} {ResourceManager.GetResourceName(ResourceType)} (now {CountNow}/{TargetCount})";
        }
        return $"{TargetCount} {ResourceManager.GetResourceName(ResourceType)} (now {CountNow}/{TargetCount})";
    }

    public override string GetTaskInMenuString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Less than {TargetCount} {ResourceManager.GetResourceName(ResourceType)}";
            case TaskCountComparison.MoreOrEqualTo:
                return $"{TargetCount} {ResourceManager.GetResourceName(ResourceType)}";
        }
        return $"{TargetCount} {ResourceManager.GetResourceName(ResourceType)}";
    }
}
