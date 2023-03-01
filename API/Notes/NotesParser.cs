using Markdig;
using Markdig.Syntax;

namespace Courses.API.Notes;

public class NotesParser
{
    public int Parse()
    {
        var document = Markdown.Parse("""
        #### TODO:
            - [ ] Item1
            - [x] Item2
            - [ ] Item3
        """);

        var lists = document.Where(b => b is ListBlock).SelectMany(l => (l as ListBlock)!.Where(child => child is ListItemBlock));
        return lists.Count();
    }
}