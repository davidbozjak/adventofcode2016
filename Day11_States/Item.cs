using System.Diagnostics;

[DebuggerDisplay("{Name}")]
class Item
{
    public string Name { get; }

    private readonly List<Item> forbiddenItemList = new();

    public IReadOnlyCollection<Item> ForbiddenItemList => this.forbiddenItemList.AsReadOnly();
    public Item ShieldItem { get; set; }


    public Item(string name)
    {
        this.Name = name;
    }

    public void SetForbiddenItemList(IEnumerable<Item> items)
    {
        this.forbiddenItemList.AddRange(items);
    }
}
