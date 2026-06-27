namespace TaskManagement.Constants;

public static class CacheKeys
{
    public static string User(Guid id) => $"user:{id}";
    public static string Project(Guid id) => $"project:{id}";
    public static string Task(Guid id) => $"task:{id}";
    public static string ProjectsList(int page, int pageSize) => $"projects:list:{page}:{pageSize}";
    public static string TasksList(int page, int pageSize, Guid? projectId = null, string? status = null, string? priority = null, string? keyword = null)
    {
        var key = $"tasks:list:{page}:{pageSize}";
        if (projectId.HasValue) key += $":project:{projectId}";
        if (!string.IsNullOrEmpty(status)) key += $":status:{status}";
        if (!string.IsNullOrEmpty(priority)) key += $":priority:{priority}";
        if (!string.IsNullOrEmpty(keyword)) key += $":keyword:{keyword}";
        return key;
    }
}
