using System.Collections.Generic;

public static class PathFollowerManager
{
    private static List<PathFollower> allPathFollowers = new List<PathFollower>();

    public static void RegisterPathFollower(PathFollower pathFollower)
    {
        if (!allPathFollowers.Contains(pathFollower))
        {
            allPathFollowers.Add(pathFollower);
        }
    }

    public static void UnregisterPathFollower(PathFollower pathFollower)
    {
        allPathFollowers.Remove(pathFollower);
    }

    public static void RequestNewPathForAll()
    {
        foreach (PathFollower pathFollower in allPathFollowers)
        {
            if (pathFollower != null)
            {
                pathFollower.RequestNewPath();
            }
        }
    }
    
    public static void RequestPathToRedZoneForAll()
    {
        foreach (PathFollower pathFollower in allPathFollowers)
        {
            if (pathFollower != null)
            {
                pathFollower.RequestPathToRedZone();
            }
        }
    }
}