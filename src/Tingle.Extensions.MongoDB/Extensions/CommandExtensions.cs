﻿namespace MongoDB.Driver.Core.Events;

internal static class CommandExtensions
{
    private static readonly HashSet<string> CommandsWithCollectionNameAsValue = [
        "aggregate",
        "count",
        "distinct",
        "mapReduce",
        "geoSearch",
        "delete",
        "find",
        "killCursors",
        "findAndModify",
        "insert",
        "update",
        "create",
        "drop",
        "createIndexes",
        "listIndexes",
    ];

    public static string? GetCollectionName(this CommandStartedEvent @event)
    {
        if (@event.CommandName == "getMore")
        {
            if (@event.Command.Contains("collection"))
            {
                var collectionValue = @event.Command.GetValue("collection");
                if (collectionValue.IsString)
                {
                    return collectionValue.AsString;
                }
            }
        }
        else if (CommandsWithCollectionNameAsValue.Contains(@event.CommandName))
        {
            var commandValue = @event.Command.GetValue(@event.CommandName);
            if (commandValue != null && commandValue.IsString)
            {
                return commandValue.AsString;
            }
        }

        return null;
    }
}
