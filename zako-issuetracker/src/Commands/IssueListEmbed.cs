using Discord;

namespace zako_issuetracker.commands;

public static class IssueListEmbed
{
    private const int PageSize = 10;
    private static int _currentPage = 1;
    public static EmbedBuilder Next()
    {
        
    }
    
    public static EmbedBuilder Previous()
    {
        
    }
    
    private static EmbedBuilder BuildIssueListEmbed(Dictionary<int, Issue.IssueContent> dict, int page)
    {
        var eb = new EmbedBuilder();
        eb.WithTitle("Issue List");
        eb.WithColor(Color.Blue);
        eb.WithFooter($"Page {page + 1} | Total Issues: {dict.Count}");
        eb.WithTimestamp(DateTimeOffset.Now);

        var AppearedIssues = dict.Skip((page -1) * PageSize).Take(PageSize);
        foreach (var ctx in AppearedIssues)
        {
            eb.AddField($"ID : {ctx.Key.ToString()}",
                $"Name : {ctx.Value.Name}\n" +
                $"Detail : {ctx.Value.Detail}\n"+
                $"Tag : {ctx.Value.Tag.ToString()}\n"+
                $"Status : {ctx.Value.Status.ToString()}\n"+
                $"User : <@{ctx.Value.UserId}>");
        }
        
        return eb;
    }
}