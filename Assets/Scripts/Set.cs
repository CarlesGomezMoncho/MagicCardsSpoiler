public class Set
{
    public int id;
    public string name;
    public string shortname;
    public string date;

    /// <summary>
    /// Contructor del set
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name">Nom complet del set</param>
    /// <param name="shortname">Nom curt (3 lletres) del set</param>
    /// <param name="date">data de eixida del set</param>
    public Set(int id, string name, string shortname, string date)
    {
        this.id = id;
        this.name = name;
        this.shortname = shortname;
        this.date = date;
    }

    override
    public string ToString()
    {
        return "id: " + id + "  name: " + name + "  shortname: " + shortname + "  date: " + date;
    }
}
